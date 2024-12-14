// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Entities;
using AITSYS.Vimeo.Ott.Exceptions;
using AITSYS.Vimeo.Ott.Logging;
using AITSYS.Vimeo.Ott.Regexes;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Rest;

/// <summary>
///     Represents a client used to make REST requests.
/// </summary>
internal sealed class RestClient : IDisposable
{
	/// <summary>
	///     Gets the bucket cleanup delay.
	/// </summary>
	private readonly TimeSpan _bucketCleanupDelay = TimeSpan.FromSeconds(60);

	/// <summary>
	///     Gets the global rate limit event.
	/// </summary>
	private readonly AsyncManualResetEvent _globalRateLimitEvent;

	/// <summary>
	///     Gets the hashes to buckets.
	/// </summary>
	private readonly ConcurrentDictionary<string, RateLimitBucket> _hashesToBuckets;

	/// <summary>
	///     Gets the logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	///     Gets the request queue.
	/// </summary>
	private readonly ConcurrentDictionary<string, int> _requestQueue;

	/// <summary>
	///     Gets the routes to hashes.
	/// </summary>
	private readonly ConcurrentDictionary<string, string> _routesToHashes;

	/// <summary>
	///     Gets a value indicating whether use reset after.
	/// </summary>
	private readonly bool _useResetAfter;

	/// <summary>
	///     Gets the bucket cleaner token source.
	/// </summary>
	private CancellationTokenSource? _bucketCleanerTokenSource;

	/// <summary>
	///     Gets whether the bucket cleaner is running.
	/// </summary>
	private volatile bool _cleanerRunning;

	/// <summary>
	///     Gets the cleaner task.
	/// </summary>
	private Task? _cleanerTask;

	/// <summary>
	///     Gets whether the client is disposed.
	/// </summary>
	private volatile bool _disposed;

	/// <summary>
	///     Initializes a new instance of the <see cref="RestClient" /> class.
	/// </summary>
	/// <param name="client">The client.</param>
	internal RestClient(VimeoOttClient client)
	{
		var vimeoOtt = client;

		this.Debug = vimeoOtt.Configuration.MinimumLogLevel is LogLevel.Trace;

		this._logger = client.Logger;

		var httphandler = new HttpClientHandler
		{
			UseCookies = false,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = client.Configuration.Proxy != null,
			Proxy = client.Configuration.Proxy
		};

		this.HttpClient = new(httphandler)
		{
			BaseAddress = new(Utilities.GetApiBaseUri()),
			Timeout = client.Configuration.HttpTimeout
		};

		this.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation(CommonHeaders.AUTHORIZATION, Utilities.GetFormattedToken(client));
		this.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation(CommonHeaders.USER_AGENT, Utilities.GetUserAgent(client));
		if (client.Configuration.VhxCustomer is not null)
			this.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Customer", client.Configuration.VhxCustomer);
		if (client.Configuration.VhxClientIp is not null)
			this.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Client-IP", client.Configuration.VhxCustomer);

		this._routesToHashes = new();
		this._hashesToBuckets = new();
		this._requestQueue = new();

		this._globalRateLimitEvent = new(true);
		this._useResetAfter = client.Configuration.UseRelativeRatelimit;
	}

	/// <summary>
	///     Gets the http client.
	/// </summary>
	internal HttpClient HttpClient { get; }

	/// <summary>
	///     Gets a value indicating whether debug is enabled.
	/// </summary>
	internal bool Debug { get; set; } = false;

	/// <summary>
	///     Disposes the rest client.
	/// </summary>
	public void Dispose()
	{
		ObjectDisposedException.ThrowIf(this._disposed, this);

		this._disposed = true;

		this._globalRateLimitEvent.Reset();

		if (this._bucketCleanerTokenSource?.IsCancellationRequested is false)
		{
			this._bucketCleanerTokenSource?.Cancel();
			this._logger.LogDebug(LoggerEvents.RestCleaner, "Bucket cleaner task stopped.");
		}

		try
		{
			this._cleanerTask?.Dispose();
			this._bucketCleanerTokenSource?.Dispose();
			this.HttpClient.Dispose();
		}
		// ReSharper disable once EmptyGeneralCatchClause
		catch
		{ }

		this._routesToHashes.Clear();
		this._hashesToBuckets.Clear();
		this._requestQueue.Clear();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///     Gets a ratelimit bucket.
	/// </summary>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <param name="routeParams">The route parameters.</param>
	/// <param name="url">The url.</param>
	/// <returns>A ratelimit bucket.</returns>
	public RateLimitBucket GetBucket(RestRequestMethod method, string route, object routeParams, out string url)
		=> this.GetBucket(method, route, routeParams, null, out url);

	/// <summary>
	///     Gets a ratelimit bucket.
	/// </summary>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <param name="routeParams">The route parameters.</param>
	/// <param name="customerHref">The customer href.</param>
	/// <param name="url">The url.</param>
	/// <returns>A ratelimit bucket.</returns>
	public RateLimitBucket GetBucket(RestRequestMethod method, string route, object routeParams, string? customerHref, out string url)
	{
		var rparamsProps = routeParams.GetType()
			.GetTypeInfo()
			.DeclaredProperties;
		var rparams = new Dictionary<string, string>();
		foreach (var xp in rparamsProps)
		{
			var val = xp.GetValue(routeParams);
#pragma warning disable CS8601, CS8602
			rparams[xp.Name] = val switch
			{
				string xs => xs,
				DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
				DateTimeOffset dto => dto.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
				IFormattable xf => xf.ToString(null, CultureInfo.InvariantCulture),
				_ => val.ToString()
			};
#pragma warning restore CS8601, CS8602
		}

		customerHref ??= "unlimited";

		var hashKey = RateLimitBucket.GenerateHashKey(method, route);

		var hash = this._routesToHashes.GetOrAdd(hashKey, RateLimitBucket.GenerateUnlimitedHash(method, route));

		var bucketId = RateLimitBucket.GenerateBucketId(hash, customerHref);

		var bucket = this._hashesToBuckets.GetOrAdd(bucketId, new RateLimitBucket(hash, customerHref));

		bucket.LastAttemptAt = DateTimeOffset.UtcNow;

		if (!bucket.RouteHashes.Contains(bucketId))
			bucket.RouteHashes.Add(bucketId);

		_ = this._requestQueue.TryGetValue(bucketId, out var count);

		this._requestQueue[bucketId] = Interlocked.Increment(ref count);

		if (!this._cleanerRunning)
		{
			this._cleanerRunning = true;
			this._bucketCleanerTokenSource = new();
			this._cleanerTask = Task.Run(this.CleanupBucketsAsync, this._bucketCleanerTokenSource.Token);
			this._logger.LogDebug(LoggerEvents.RestCleaner, "Bucket cleaner task started.");
		}

		url = CommonRegex.HttpRouteRegex().Replace(route, xm => rparams[xm.Groups[1].Value]);
		return bucket;
	}

	/// <summary>
	///     Executes the request.
	/// </summary>
	/// <param name="request">The request to be executed.</param>
	public Task ExecuteRequestAsync(BaseRestRequest request)
		=> request is null ? throw new ArgumentNullException(nameof(request)) : this.ExecuteRequestAsync(request, null, null);

	/// <summary>
	///     Executes the form data request.
	/// </summary>
	/// <param name="request">The request to be executed.</param>
	public Task ExecuteFormRequestAsync(BaseRestRequest request)
		=> request is null ? throw new ArgumentNullException(nameof(request)) : this.ExecuteFormRequestAsync(request, null, null);

	/// <summary>
	///     Executes the form data request.
	///     This is to allow proper rescheduling of the first request from a bucket.
	/// </summary>
	/// <param name="request">The request to be executed.</param>
	/// <param name="bucket">The bucket.</param>
	/// <param name="ratelimitTcs">The ratelimit task completion source.</param>
	/// <param name="targetDebug">Enables a possible breakpoint in the rest client for debugging purposes.</param>
	private async Task ExecuteFormRequestAsync(BaseRestRequest request, RateLimitBucket? bucket, TaskCompletionSource<bool>? ratelimitTcs, bool targetDebug = false)
	{
		ObjectDisposedException.ThrowIf(this._disposed, this);

		if (targetDebug)
			Console.WriteLine("Meow");

		HttpResponseMessage? res = null;

		try
		{
			await this._globalRateLimitEvent.WaitAsync().ConfigureAwait(false);

			bucket ??= request.RateLimitBucket;

			ratelimitTcs ??= await this.WaitForInitialRateLimit(bucket).ConfigureAwait(false);

			if (ratelimitTcs is null)
			{
				var now = DateTimeOffset.UtcNow;

				await bucket.TryResetLimitAsync(now).ConfigureAwait(false);

				if (Interlocked.Decrement(ref bucket.RemainingInternal) < 0)
				{
					this._logger.LogWarning(LoggerEvents.RatelimitDiag, "Request for {bucket} is blocked. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);
					var delay = bucket.Reset - now;
					var resetDate = bucket.Reset;

					if (this._useResetAfter)
					{
						delay = bucket.ResetAfter.Value;
						resetDate = bucket.ResetAfterOffset;
					}

					if (delay < new TimeSpan(-TimeSpan.TicksPerMinute))
					{
						this._logger.LogError(LoggerEvents.RatelimitDiag, "Failed to retrieve ratelimits - giving up and allowing next request for bucket");
						bucket.RemainingInternal = 1;
					}

					if (delay < TimeSpan.Zero)
						delay = TimeSpan.FromMilliseconds(100);

					this._logger.LogWarning(LoggerEvents.RatelimitPreemptive, "Preemptive ratelimit triggered - waiting until {ResetDate:yyyy-MM-dd HH:mm:ss zzz} ({Delay:c})", resetDate, delay);
					Task.Delay(delay)
						.ContinueWith(_ => this.ExecuteFormRequestAsync(request, null, null))
						.LogTaskFault(this._logger, LogLevel.Error, LoggerEvents.RestError, "Error while executing request");

					return;
				}

				this._logger.LogDebug(LoggerEvents.RatelimitDiag, "Request for {bucket} is allowed. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);
			}
			else
				this._logger.LogDebug(LoggerEvents.RatelimitDiag, "Initial request for {bucket} is allowed. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);

			var req = BuildFormRequest(request);

			if (this.Debug && req.Content is not null)
				this._logger.Log(LogLevel.Trace, LoggerEvents.RestTx, "Rest Form Request Content:\n{Content}", await req.Content.ReadAsStringAsync());

			var response = new RestResponse();
			try
			{
				ObjectDisposedException.ThrowIf(this._disposed, this);

				res = await this.HttpClient.SendAsync(req, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ConfigureAwait(false);

				var bts = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
				var txt = Utilities.UTF8.GetString(bts, 0, bts.Length);

				if (this.Debug)
					this._logger.Log(LogLevel.Trace, LoggerEvents.RestRx, "Rest Form Response Content: {Content}", txt);

				// ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
				response.Headers = res.Headers?.ToDictionary(xh => xh.Key, xh => string.Join("\n", xh.Value), StringComparer.OrdinalIgnoreCase);
				response.Response = txt;
				response.ResponseCode = res.StatusCode;
			}
			catch (HttpRequestException httpex)
			{
				this._logger.LogError(LoggerEvents.RestError, httpex, "Request to {Url} triggered an HttpException", request.Url.AbsoluteUri);
				request.SetFaulted(httpex);
				this.FailInitialRateLimitTest(request, ratelimitTcs);
				return;
			}

			this.UpdateBucket(request, response, ratelimitTcs);

			Exception? ex = null;

			switch (response.ResponseCode)
			{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.MethodNotAllowed:
					ex = new BadRequestException(request, response);
					break;

				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.Forbidden:
					ex = new UnauthorizedException(request, response);
					break;

				case HttpStatusCode.PaymentRequired:
					ex = new PaymentRequiredException(request, response);
					break;

				case HttpStatusCode.NotFound:
					ex = new NotFoundException(request, response);
					break;

				case HttpStatusCode.NotAcceptable:
					ex = new NotAcceptableException(request, response);
					break;

				case HttpStatusCode.UnprocessableEntity:
					ex = new UnprocessableEntityException(request, response);
					break;

				case HttpStatusCode.RequestEntityTooLarge:
					ex = new RequestSizeException(request, response);
					break;

				case HttpStatusCode.TooManyRequests:
					ex = new RateLimitException(request, response);

					Handle429(response, out var wait, out var global);
					if (wait is not null)
					{
						if (global)
						{
							bucket.IsGlobal = true;
							this._logger.LogError(LoggerEvents.RatelimitHit, "Global ratelimit hit, cooling down for {uri}", request.Url.AbsoluteUri);
							try
							{
								this._globalRateLimitEvent.Reset();
								await wait.ConfigureAwait(false);
							}
							finally
							{
								_ = this._globalRateLimitEvent.SetAsync();
							}
						}
						else
						{
							this._logger.LogError(LoggerEvents.RatelimitHit, "Ratelimit hit, requeuing request to {url}", request.Url.AbsoluteUri);
							await wait.ConfigureAwait(false);
						}

						this.ExecuteRequestAsync(request, bucket, ratelimitTcs)
							.LogTaskFault(this._logger, LogLevel.Error, LoggerEvents.RestError, "Error while retrying request");

						return;
					}

					break;

				case HttpStatusCode.InternalServerError:
				case HttpStatusCode.NotImplemented:
				case HttpStatusCode.BadGateway:
				case HttpStatusCode.ServiceUnavailable:
				case HttpStatusCode.GatewayTimeout:
				case HttpStatusCode.HttpVersionNotSupported:
					ex = new ServerErrorException(request, response);
					break;
			}

			if (ex is not null)
				request.SetFaulted(ex);
			else
				request.SetCompleted(response);
		}
		catch (Exception ex)
		{
			this._logger.LogError(LoggerEvents.RestError, ex, "Request to {Url} triggered an exception", request.Url.AbsoluteUri);

			if (bucket is not null && ratelimitTcs is not null && bucket.LimitTesting is not 0)
				this.FailInitialRateLimitTest(request, ratelimitTcs);

			if (!request.TrySetFaulted(ex))
				throw;
		}
		finally
		{
			res?.Dispose();

			if (bucket?.BucketId is not null)
			{
				_ = this._requestQueue.TryGetValue(bucket.BucketId, out var count);
				this._requestQueue[bucket.BucketId] = Interlocked.Decrement(ref count);

				if (count <= 0)
					foreach (var r in bucket.RouteHashes)
						if (this._requestQueue.ContainsKey(r))
							_ = this._requestQueue.TryRemove(r, out _);
			}
		}
	}

	/// <summary>
	///     Executes the request.
	///     This is to allow proper rescheduling of the first request from a bucket.
	/// </summary>
	/// <param name="request">The request to be executed.</param>
	/// <param name="bucket">The bucket.</param>
	/// <param name="ratelimitTcs">The ratelimit task completion source.</param>
	private async Task ExecuteRequestAsync(BaseRestRequest request, RateLimitBucket? bucket, TaskCompletionSource<bool>? ratelimitTcs)
	{
		ObjectDisposedException.ThrowIf(this._disposed, this);

		HttpResponseMessage? res = null;

		try
		{
			await this._globalRateLimitEvent.WaitAsync().ConfigureAwait(false);

			bucket ??= request.RateLimitBucket;

			ratelimitTcs ??= await this.WaitForInitialRateLimit(bucket).ConfigureAwait(false);

			if (ratelimitTcs == null)
			{
				var now = DateTimeOffset.UtcNow;

				await bucket.TryResetLimitAsync(now).ConfigureAwait(false);

				if (Interlocked.Decrement(ref bucket.RemainingInternal) < 0)
				{
					this._logger.LogWarning(LoggerEvents.RatelimitDiag, "Request for {bucket} is blocked. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);
					var delay = bucket.Reset - now;
					var resetDate = bucket.Reset;

					if (this._useResetAfter)
					{
						delay = bucket.ResetAfter.Value;
						resetDate = bucket.ResetAfterOffset;
					}

					if (delay < new TimeSpan(-TimeSpan.TicksPerMinute))
					{
						this._logger.LogError(LoggerEvents.RatelimitDiag, "Failed to retrieve ratelimits - giving up and allowing next request for bucket");
						bucket.RemainingInternal = 1;
					}

					if (delay < TimeSpan.Zero)
						delay = TimeSpan.FromMilliseconds(100);

					this._logger.LogWarning(LoggerEvents.RatelimitPreemptive, "Preemptive ratelimit triggered - waiting until {0:yyyy-MM-dd HH:mm:ss zzz} ({1:c}).", resetDate, delay);
					Task.Delay(delay)
						.ContinueWith(_ => this.ExecuteRequestAsync(request, null, null))
						.LogTaskFault(this._logger, LogLevel.Error, LoggerEvents.RestError, "Error while executing request");

					return;
				}

				this._logger.LogDebug(LoggerEvents.RatelimitDiag, "Request for {bucket} is allowed. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);
			}
			else
				this._logger.LogDebug(LoggerEvents.RatelimitDiag, "Initial request for {bucket} is allowed. Url: {url}", bucket.ToString(), request.Url.AbsoluteUri);

			var req = this.BuildRequest(request);

			if (this.Debug && req.Content is not null)
				this._logger.Log(LogLevel.Trace, LoggerEvents.RestTx, "Rest Request Content:\n{Content}", await req.Content.ReadAsStringAsync());

			var response = new RestResponse();
			try
			{
				ObjectDisposedException.ThrowIf(this._disposed, this);

				res = await this.HttpClient.SendAsync(req, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ConfigureAwait(false);

				var bts = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
				var txt = Utilities.UTF8.GetString(bts, 0, bts.Length);

				if (this.Debug)
					this._logger.Log(LogLevel.Trace, LoggerEvents.RestRx, "Rest Response Content: {Content}", txt);

				// ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
				response.Headers = res.Headers?.ToDictionary(xh => xh.Key, xh => string.Join("\n", xh.Value), StringComparer.OrdinalIgnoreCase);
				response.Response = txt;
				response.ResponseCode = res.StatusCode;
			}
			catch (HttpRequestException httpex)
			{
				this._logger.LogError(LoggerEvents.RestError, httpex, "Request to {0} triggered an HttpException", request.Url.AbsoluteUri);
				request.SetFaulted(httpex);
				this.FailInitialRateLimitTest(request, ratelimitTcs);
				return;
			}

			this.UpdateBucket(request, response, ratelimitTcs);

			Exception? ex = null;
			switch (response.ResponseCode)
			{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.MethodNotAllowed:
					ex = new BadRequestException(request, response);
					break;

				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.Forbidden:
					ex = new UnauthorizedException(request, response);
					break;

				case HttpStatusCode.PaymentRequired:
					ex = new PaymentRequiredException(request, response);
					break;

				case HttpStatusCode.NotFound:
					ex = new NotFoundException(request, response);
					break;

				case HttpStatusCode.NotAcceptable:
					ex = new NotAcceptableException(request, response);
					break;

				case HttpStatusCode.UnprocessableEntity:
					ex = new UnprocessableEntityException(request, response);
					break;

				case HttpStatusCode.RequestEntityTooLarge:
					ex = new RequestSizeException(request, response);
					break;

				case HttpStatusCode.TooManyRequests:
					ex = new RateLimitException(request, response);

					Handle429(response, out var wait, out var global);
					if (wait is not null)
					{
						if (global)
						{
							bucket.IsGlobal = true;
							this._logger.LogError(LoggerEvents.RatelimitHit, "Global ratelimit hit, cooling down for {uri}", request.Url.AbsoluteUri);
							try
							{
								this._globalRateLimitEvent.Reset();
								await wait.ConfigureAwait(false);
							}
							finally
							{
								_ = this._globalRateLimitEvent.SetAsync();
							}
						}
						else
						{
							this._logger.LogError(LoggerEvents.RatelimitHit, "Ratelimit hit, requeuing request to {url}", request.Url.AbsoluteUri);
							await wait.ConfigureAwait(false);
						}

						this.ExecuteRequestAsync(request, bucket, ratelimitTcs)
							.LogTaskFault(this._logger, LogLevel.Error, LoggerEvents.RestError, "Error while retrying request");

						return;
					}

					break;

				case HttpStatusCode.InternalServerError:
				case HttpStatusCode.NotImplemented:
				case HttpStatusCode.BadGateway:
				case HttpStatusCode.ServiceUnavailable:
				case HttpStatusCode.GatewayTimeout:
				case HttpStatusCode.HttpVersionNotSupported:
					ex = new ServerErrorException(request, response);
					break;
			}

			if (ex is not null)
				request.SetFaulted(ex);
			else
				request.SetCompleted(response);
		}
		catch (Exception ex)
		{
			this._logger.LogError(LoggerEvents.RestError, ex, "Request to {0} triggered an exception", request.Url.AbsoluteUri);

			if (bucket is not null && ratelimitTcs is not null && bucket.LimitTesting is not 0)
				this.FailInitialRateLimitTest(request, ratelimitTcs);

			if (!request.TrySetFaulted(ex))
				throw;
		}
		finally
		{
			res?.Dispose();

			if (bucket?.BucketId is not null)
			{
				_ = this._requestQueue.TryGetValue(bucket.BucketId, out var count);
				this._requestQueue[bucket.BucketId] = Interlocked.Decrement(ref count);

				if (count <= 0)
					foreach (var r in bucket.RouteHashes)
						if (this._requestQueue.ContainsKey(r))
							_ = this._requestQueue.TryRemove(r, out _);
			}
		}
	}

	/// <summary>
	///     Fails the initial rate limit test.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="ratelimitTcs">The ratelimit task completion source.</param>
	/// <param name="resetToInitial">Whether to reset to initial values.</param>
	private void FailInitialRateLimitTest(BaseRestRequest request, TaskCompletionSource<bool>? ratelimitTcs, bool resetToInitial = false)
	{
		if (ratelimitTcs is null && !resetToInitial)
			return;

		var bucket = request.RateLimitBucket;

		bucket.LimitValid = false;
		bucket.LimitTestFinished = null;
		bucket.LimitTesting = 0;

		if (resetToInitial)
		{
			this.UpdateHashCaches(request, bucket);
			bucket.Maximum = 0;
			bucket.RemainingInternal = 0;
			return;
		}

		_ = Task.Run(() => ratelimitTcs?.TrySetResult(false));
	}

	/// <summary>
	///     Waits for the initial rate limit.
	/// </summary>
	/// <param name="bucket">The bucket.</param>
	private async Task<TaskCompletionSource<bool>?> WaitForInitialRateLimit(RateLimitBucket bucket)
	{
		while (!bucket.LimitValid)
		{
			if (bucket.LimitTesting is 0)
				if (Interlocked.CompareExchange(ref bucket.LimitTesting, 1, 0) is 0)
				{
					if (bucket.LimitValid)
						return null;

					var ratelimitsTcs = new TaskCompletionSource<bool>();
					bucket.LimitTestFinished = ratelimitsTcs.Task;
					return ratelimitsTcs;
				}

			Task? waitTask = null;
			while (bucket.LimitTesting is not 0 && (waitTask = bucket.LimitTestFinished) is null)
				await Task.Yield();
			if (waitTask is not null)
				await waitTask.ConfigureAwait(false);
		}

		return null;
	}

	/// <summary>
	///     Builds the form data request.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns>A http request message.</returns>
	private static HttpRequestMessage BuildFormRequest(BaseRestRequest request)
	{
		var req = new HttpRequestMessage(new(request.Method.ToString()), request.Url);
		if (request.Headers is not null && request.Headers.Any())
			foreach (var kvp in request.Headers)
				switch (kvp.Key)
				{
					case "Bearer":
						req.Headers.Authorization = new(CommonHeaders.AUTHORIZATION_BEARER, kvp.Value);
						break;
					default:
						req.Headers.Add(kvp.Key, kvp.Value);
						break;
				}

		if (request is not RestFormRequest formRequest)
			throw new InvalidOperationException();

		req.Content = new FormUrlEncodedContent(formRequest.FormData);
		req.Content.Headers.ContentType = new("application/x-www-form-urlencoded");

		return req;
	}

	/// <summary>
	///     Builds the request.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns>A http request message.</returns>
	private HttpRequestMessage BuildRequest(BaseRestRequest request)
	{
		var req = new HttpRequestMessage(new(request.Method.ToString()), request.Url);
		if (request.Headers is not null && request.Headers.Any())
			foreach (var kvp in request.Headers)
				switch (kvp.Key)
				{
					case "Bearer":
						req.Headers.Authorization = new(CommonHeaders.AUTHORIZATION_BEARER, kvp.Value);
						break;
					default:
						req.Headers.Add(kvp.Key, kvp.Value);
						break;
				}

		switch (request)
		{
			case RestRequest nmprequest when !string.IsNullOrWhiteSpace(nmprequest.Payload):
				this._logger.LogTrace(LoggerEvents.RestTx, nmprequest.Payload);

				req.Content = new StringContent(nmprequest.Payload);
				req.Content.Headers.ContentType = new("application/json");
				break;
		}

		return req;
	}

	/// <summary>
	///     Handles the HTTP 429 status.
	/// </summary>
	/// <param name="response">The response.</param>
	/// <param name="waitTask">The wait task.</param>
	/// <param name="global">If true, global.</param>
	private static void Handle429(RestResponse response, out Task? waitTask, out bool global)
	{
		waitTask = null;
		global = false;

		if (response.Headers is null)
			return;

		var hs = response.Headers;

		if (hs.TryGetValue(CommonHeaders.RETRY_AFTER, out var retryAfterRaw))
		{
			var retryAfter = TimeSpan.FromSeconds(int.Parse(retryAfterRaw, CultureInfo.InvariantCulture));
			waitTask = Task.Delay(retryAfter);
		}

		if (hs.TryGetValue(CommonHeaders.RATELIMIT_GLOBAL, out var isGlobal) && isGlobal.ToLowerInvariant() is "true")
			global = true;
	}

	/// <summary>
	///     Updates the bucket.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="response">The response.</param>
	/// <param name="ratelimitTcs">The ratelimit task completion source.</param>
	private void UpdateBucket(BaseRestRequest request, RestResponse response, TaskCompletionSource<bool>? ratelimitTcs)
	{
		var bucket = request.RateLimitBucket;

		if (response.Headers is null)
		{
			if (response.ResponseCode is not HttpStatusCode.TooManyRequests)
				this.FailInitialRateLimitTest(request, ratelimitTcs);
			return;
		}

		var hs = response.Headers;

		if (hs.TryGetValue(CommonHeaders.RATELIMIT_SCOPE, out var scope))
			bucket.Scope = scope;

		if (hs.TryGetValue(CommonHeaders.RATELIMIT_GLOBAL, out var isGlobal) && isGlobal.ToLowerInvariant() is "true")
		{
			if (response.ResponseCode is HttpStatusCode.TooManyRequests)
				return;

			bucket.IsGlobal = true;
			this.FailInitialRateLimitTest(request, ratelimitTcs);

			return;
		}

		var r1 = hs.TryGetValue(CommonHeaders.RATELIMIT_LIMIT, out var usesMax);
		var r2 = hs.TryGetValue(CommonHeaders.RATELIMIT_REMAINING, out var usesLeft);
		var r3 = hs.TryGetValue(CommonHeaders.RATELIMIT_RESET, out var reset);
		var r4 = hs.TryGetValue(CommonHeaders.RATELIMIT_RESET_AFTER, out var resetAfter);
		var r5 = hs.TryGetValue(CommonHeaders.RATELIMIT_BUCKET, out var hash);

		if (!r1 || !r2 || !r3 || !r4 || !r5)
		{
			if (response.ResponseCode is not HttpStatusCode.TooManyRequests)
				this.FailInitialRateLimitTest(request, ratelimitTcs, ratelimitTcs is null);

			return;
		}

		var clientTime = DateTimeOffset.UtcNow;
		var resetTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(double.Parse(reset!, CultureInfo.InvariantCulture));
		var serverTime = clientTime;
		if (hs.TryGetValue("Date", out var rawDate))
			serverTime = DateTimeOffset.Parse(rawDate, CultureInfo.InvariantCulture).ToUniversalTime();

		var resetDelta = resetTime - serverTime;

		if (request.RateLimitWaitOverride.HasValue)
			resetDelta = TimeSpan.FromSeconds(request.RateLimitWaitOverride.Value);
		var newReset = clientTime + resetDelta;

		if (this._useResetAfter)
		{
			bucket.ResetAfter = TimeSpan.FromSeconds(double.Parse(resetAfter!, CultureInfo.InvariantCulture));
			newReset = clientTime + bucket.ResetAfter.Value + (request.RateLimitWaitOverride.HasValue
				? resetDelta
				: TimeSpan.Zero);
			bucket.ResetAfterOffset = newReset;
		}
		else
			bucket.Reset = newReset;

		var maximum = int.Parse(usesMax!, CultureInfo.InvariantCulture);
		var remaining = int.Parse(usesLeft!, CultureInfo.InvariantCulture);

		if (ratelimitTcs is not null)
		{
			bucket.SetInitialValues(maximum, remaining, newReset);

			_ = Task.Run(() => ratelimitTcs.TrySetResult(true));
		}
		else
		{
			if (bucket.NextReset == 0)
				bucket.NextReset = newReset.UtcTicks;
		}

		this.UpdateHashCaches(request, bucket, hash);
	}

	/// <summary>
	///     Updates the hash caches.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="bucket">The bucket.</param>
	/// <param name="newHash">The new hash.</param>
	private void UpdateHashCaches(BaseRestRequest request, RateLimitBucket bucket, string? newHash = null)
	{
		var hashKey = RateLimitBucket.GenerateHashKey(request.Method, request.Route);

		if (!this._routesToHashes.TryGetValue(hashKey, out var oldHash))
			return;

		if (newHash is null)
		{
			_ = this._routesToHashes.TryRemove(hashKey, out _);
			if (bucket.BucketId is not null)
				_ = this._hashesToBuckets.TryRemove(bucket.BucketId, out _);
			return;
		}

		if (!bucket.IsUnlimited || newHash == oldHash)
			return;

		this._logger.LogDebug(LoggerEvents.RestHashMover, "Updating hash in {0}: \"{1}\" -> \"{2}\"", hashKey, oldHash, newHash);
		var bucketId = RateLimitBucket.GenerateBucketId(newHash, bucket.CustomerHref);

		_ = this._routesToHashes.AddOrUpdate(hashKey, newHash, (key, previousHash) =>
		{
			bucket.Hash = newHash;

			var oldBucketId = RateLimitBucket.GenerateBucketId(oldHash, bucket.CustomerHref);

			_ = this._hashesToBuckets.TryRemove(oldBucketId, out _);
			_ = this._hashesToBuckets.AddOrUpdate(bucketId, bucket, (_, _) => bucket);

			return newHash;
		});
	}

	/// <summary>
	///     Cleans the buckets.
	/// </summary>
	private async Task CleanupBucketsAsync()
	{
		while (!this._bucketCleanerTokenSource?.IsCancellationRequested ?? false)
		{
			try
			{
				await Task.Delay(this._bucketCleanupDelay, this._bucketCleanerTokenSource.Token).ConfigureAwait(false);
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{ }

			ObjectDisposedException.ThrowIf(this._disposed, this);

			foreach (var key in this._requestQueue.Keys)
			{
				var bucket = this._hashesToBuckets.Values.FirstOrDefault(x => x.RouteHashes.Contains(key));

				if (bucket is null || bucket.LastAttemptAt.AddSeconds(5) < DateTimeOffset.UtcNow)
					_ = this._requestQueue.TryRemove(key, out _);
			}

			var removedBuckets = 0;
			StringBuilder? bucketIdStrBuilder = null;

			foreach (var (key, value) in this._hashesToBuckets)
			{
				bucketIdStrBuilder ??= new();

				if (string.IsNullOrEmpty(value.BucketId) || (this._requestQueue.ContainsKey(value.BucketId) && !value.IsUnlimited))
					continue;

				var resetOffset = this._useResetAfter ? value.ResetAfterOffset : value.Reset;

				if (!value.IsUnlimited && (resetOffset > DateTimeOffset.UtcNow || DateTimeOffset.UtcNow - resetOffset < this._bucketCleanupDelay))
					continue;

				_ = this._hashesToBuckets.TryRemove(key, out _);
				removedBuckets++;
				bucketIdStrBuilder.Append(value.BucketId + ", ");
			}

			if (removedBuckets > 0)
				this._logger.LogDebug(LoggerEvents.RestCleaner, "Removed {0} unused bucket{1}: [{2}]", removedBuckets, removedBuckets > 1 ? "s" : string.Empty, bucketIdStrBuilder?.ToString().TrimEnd(',', ' '));

			if (this._hashesToBuckets.IsEmpty)
				break;
		}

		if (this._bucketCleanerTokenSource is not null && !this._bucketCleanerTokenSource.IsCancellationRequested)
			await this._bucketCleanerTokenSource.CancelAsync();

		this._cleanerRunning = false;
		this._logger.LogDebug(LoggerEvents.RestCleaner, "Bucket cleaner task stopped.");
	}

	/// <summary>
	///     Disposes the rest client.
	/// </summary>
	~RestClient()
	{
		this.Dispose();
	}
}
