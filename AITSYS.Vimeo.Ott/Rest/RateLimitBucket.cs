// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Collections.Concurrent;

namespace AITSYS.Vimeo.Ott.Rest;

/// <summary>
///     Represents a rate limit bucket.
/// </summary>
internal sealed class RateLimitBucket : IEquatable<RateLimitBucket>
{
	/// <summary>
	///     Gets the unlimited hash.
	/// </summary>
	private static readonly string s_unlimitedHash = "unlimited";

	/// <summary>
	///     Gets the Id of the ratelimit bucket.
	/// </summary>
	public volatile string? BucketId;

	/// <summary>
	///     Gets the ratelimit hash of this bucket.
	/// </summary>
	internal string HashInternal;

	/// <summary>
	///     Gets whether this bucket has it's ratelimit determined.
	///     <para>This will be <see langword="false" /> if the ratelimit is determined.</para>
	/// </summary>
	internal volatile bool IsUnlimited;

	/// <summary>
	///     If the rate limit is currently being reset.
	///     This is a int because booleans can't be accessed atomically.
	///     0 => False, all other values => True
	/// </summary>
	internal volatile int LimitResetting;

	/// <summary>
	///     Task to wait for the rate limit test to finish.
	/// </summary>
	internal volatile Task? LimitTestFinished;

	/// <summary>
	///     If the initial request for this bucket that is determining the rate limits is currently executing.
	///     This is a int because booleans can't be accessed atomically.
	///     0 => False, all other values => True
	/// </summary>
	internal volatile int LimitTesting;

	/// <summary>
	///     If the rate limits have been determined.
	/// </summary>
	internal volatile bool LimitValid;

	/// <summary>
	///     Rate limit reset in ticks, UTC on the next response after the rate limit has been reset.
	/// </summary>
	internal long NextReset;

	/// <summary>
	///     Gets or sets the remaining number of requests to the maximum when the ratelimit is reset.
	/// </summary>
	internal volatile int RemainingInternal;

	/// <summary>
	///     Initializes a new instance of the <see cref="RateLimitBucket" /> class.
	/// </summary>
	/// <param name="hash">The hash.</param>
	/// <param name="customerHref">Customer href for this bucket.</param>
	internal RateLimitBucket(string hash, string customerHref)
	{
		this.Hash = hash;
		this.CustomerHref = customerHref;
		this.HashInternal = null!;

		this.BucketId = GenerateBucketId(hash, customerHref);
		this.RouteHashes = [];
	}

	/// <summary>
	///     Gets the customer href of the bucket.
	/// </summary>
	public string CustomerHref { get; }

	/// <summary>
	///     Gets or sets the ratelimit hash of this bucket.
	/// </summary>
	public string Hash
	{
		get => Volatile.Read(ref this.HashInternal);

		internal set
		{
			this.IsUnlimited = value.Contains(s_unlimitedHash);

			if (this.BucketId is not null && !this.BucketId.StartsWith(value, StringComparison.Ordinal))
			{
				var id = GenerateBucketId(value, this.CustomerHref);
				this.BucketId = id;
				this.RouteHashes.Add(id);
			}

			Volatile.Write(ref this.HashInternal, value);
		}
	}

	/// <summary>
	///     Gets the past route hashes associated with this bucket.
	/// </summary>
	public ConcurrentBag<string> RouteHashes { get; }

	/// <summary>
	///     Gets when this bucket was last called in a request.
	/// </summary>
	public DateTimeOffset LastAttemptAt { get; internal set; }

	/// <summary>
	///     Gets the number of uses left before pre-emptive rate limit is triggered.
	/// </summary>
	public int Remaining
		=> this.RemainingInternal;

	/// <summary>
	///     Gets the maximum number of uses within a single bucket.
	/// </summary>
	public int Maximum { get; set; }

	/// <summary>
	///     Gets the timestamp at which the rate limit resets.
	/// </summary>
	public DateTimeOffset Reset { get; internal set; }

	/// <summary>
	///     Gets the time interval to wait before the rate limit resets.
	/// </summary>
	public TimeSpan? ResetAfter { get; internal set; }

	/// <summary>
	///     Gets a value indicating whether the ratelimit global.
	/// </summary>
	public bool IsGlobal { get; internal set; } = false;

	/// <summary>
	///     Gets the ratelimit scope.
	/// </summary>
	public string Scope { get; internal set; } = "user";

	/// <summary>
	///     Gets the time interval to wait before the rate limit resets as offset.
	/// </summary>
	internal DateTimeOffset ResetAfterOffset { get; set; }

	/// <summary>
	///     Checks whether this <see cref="RateLimitBucket" /> is equal to another <see cref="RateLimitBucket" />.
	/// </summary>
	/// <param name="e"><see cref="RateLimitBucket" /> to compare to.</param>
	/// <returns>Whether the <see cref="RateLimitBucket" /> is equal to this <see cref="RateLimitBucket" />.</returns>
	public bool Equals(RateLimitBucket? e) => e is not null && (ReferenceEquals(this, e) || this.BucketId == e.BucketId);

	/// <summary>
	///     Generates an ID for this request bucket.
	/// </summary>
	/// <param name="hash">Hash for this bucket.</param>
	/// <param name="customerHref">Customer href for this bucket.</param>
	/// <returns>Bucket Id.</returns>
	public static string GenerateBucketId(string hash, string customerHref) // , string customerId, string videoId, string siteId, string productId, string collectionId, string commentId)
		=> $"{hash}:{customerHref}";

	/// <summary>
	///     Generates the hash key.
	/// </summary>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <returns>A string.</returns>
	public static string GenerateHashKey(RestRequestMethod method, string route)
		=> $"{method}:{route}";

	/// <summary>
	///     Generates the unlimited hash.
	/// </summary>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <returns>A string.</returns>
	public static string GenerateUnlimitedHash(RestRequestMethod method, string route)
		=> $"{GenerateHashKey(method, route)}:{s_unlimitedHash}";

	/// <summary>
	///     Returns a string representation of this bucket.
	/// </summary>
	/// <returns>String representation of this bucket.</returns>
	public override string ToString()
	{
		var customerHref = this.CustomerHref != string.Empty ? this.CustomerHref : "customer_href";

		return $"{this.Scope} rate limit bucket [{this.Hash}:{customerHref}] [{this.Remaining}/{this.Maximum}] {(this.ResetAfter.HasValue ? this.ResetAfterOffset : this.Reset)}";
	}

	/// <summary>
	///     Checks whether this <see cref="RateLimitBucket" /> is equal to another object.
	/// </summary>
	/// <param name="obj">Object to compare to.</param>
	/// <returns>Whether the object is equal to this <see cref="RateLimitBucket" />.</returns>
	public override bool Equals(object? obj)
		=> this.Equals(obj as RateLimitBucket);

	/// <summary>
	///     Gets the hash code for this <see cref="RateLimitBucket" />.
	/// </summary>
	/// <returns>The hash code for this <see cref="RateLimitBucket" />.</returns>
	public override int GetHashCode()
		=> this.BucketId?.GetHashCode() ?? -1;

	/// <summary>
	///     Sets remaining number of requests to the maximum when the ratelimit is reset
	/// </summary>
	/// <param name="now">The datetime offset.</param>
	internal async Task TryResetLimitAsync(DateTimeOffset now)
	{
		if (this.ResetAfter.HasValue)
			this.ResetAfter = this.ResetAfterOffset - now;

		if (this.NextReset is 0)
			return;

		if (this.NextReset > now.UtcTicks)
			return;

		while (Interlocked.CompareExchange(ref this.LimitResetting, 1, 0) != 0)
			await Task.Yield();

		if (this.NextReset is not 0)
		{
			this.RemainingInternal = this.Maximum;
			this.NextReset = 0;
		}

		this.LimitResetting = 0;
	}

	/// <summary>
	///     Sets the initial values.
	/// </summary>
	/// <param name="max">The max.</param>
	/// <param name="usesLeft">The uses left.</param>
	/// <param name="newReset">The new reset.</param>
	internal void SetInitialValues(int max, int usesLeft, DateTimeOffset newReset)
	{
		this.Maximum = max;
		this.RemainingInternal = usesLeft;
		this.NextReset = newReset.UtcTicks;

		this.LimitValid = true;
		this.LimitTestFinished = null;
		this.LimitTesting = 0;
	}
}
