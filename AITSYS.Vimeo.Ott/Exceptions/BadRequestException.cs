// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Rest;

using Newtonsoft.Json.Linq;

namespace AITSYS.Vimeo.Ott.Exceptions;

/// <summary>
///     Represents an exception thrown when a malformed request is sent.
/// </summary>
public sealed class BadRequestException : Exception
{
	/// <summary>
	///     Initializes a new instance of the <see cref="BadRequestException" /> class.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="response">The response.</param>
	internal BadRequestException(BaseRestRequest request, RestResponse response)
		: base("Bad request: " + response.ResponseCode)
	{
		this.WebRequest = request;
		this.WebResponse = response;

		try
		{
			var j = JObject.Parse(response.Response);

			if (j["message"] is not null)
				this.JsonMessage = j["message"]!.ToString();

			if (j["documentation_url"] is not null)
				this.DocumentationUrl = j["documentation_url"]!.ToString();
		}
		catch
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
	///     Gets the JSON message received.
	/// </summary>
	public string? JsonMessage { get; internal set; } = null;

	/// <summary>
	///     Gets the documentation url.
	/// </summary>
	public string? DocumentationUrl { get; internal set; } = null;
}
