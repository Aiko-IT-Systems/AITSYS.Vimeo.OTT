// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;

namespace AITSYS.Vimeo.Ott.Rest;

/// <summary>
///     Represents a form data HTTP request.
/// </summary>
internal sealed class RestFormRequest : BaseRestRequest
{
	/// <summary>
	///     Initializes a new instance of the <see cref="RestRequest" /> class.
	/// </summary>
	/// <param name="client">The client.</param>
	/// <param name="bucket">The bucket.</param>
	/// <param name="url">The url.</param>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <param name="formData">The form data.</param>
	/// <param name="headers">The headers.</param>
	/// <param name="ratelimitWaitOverride">The ratelimit wait override.</param>
	internal RestFormRequest(VimeoOttClient client, RateLimitBucket bucket, Uri url, RestRequestMethod method, string route, Dictionary<string, string> formData, IReadOnlyDictionary<string, string>? headers = null, double? ratelimitWaitOverride = null)
		: base(client, bucket, url, method, route, headers, ratelimitWaitOverride)
	{
		this.FormData = formData;
	}

	/// <summary>
	///     Gets the form data sent with this request.
	/// </summary>
	public Dictionary<string, string> FormData { get; }
}
