// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Rest;

using Newtonsoft.Json.Linq;

namespace AITSYS.Vimeo.Ott.Exceptions;

/// <summary>
///     Represents an exception thrown when too many requests are sent.
/// </summary>
public sealed class RateLimitException : Exception
{
	/// <summary>
	///     Initializes a new instance of the <see cref="RateLimitException" /> class.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="response">The response.</param>
	internal RateLimitException(BaseRestRequest request, RestResponse response)
		: base("Rate limited: " + response.ResponseCode)
	{
		this.WebRequest = request;
		this.WebResponse = response;

		try
		{
			var j = JObject.Parse(response.Response);

			if (j["message"] != null)
				this.JsonMessage = j["message"]!.ToString();
		}
		catch (Exception)
		{ }
	}

	/// <summary>
	///     Gets the request that caused the exception.
	/// </summary>
	public BaseRestRequest WebRequest { get; internal set; }

	/// <summary>
	///     Gets the response to the request.
	/// </summary>
	public RestResponse WebResponse { get; internal set; }

	/// <summary>
	///     Gets the JSON received.
	/// </summary>
	public string? JsonMessage { get; internal set; }
}
