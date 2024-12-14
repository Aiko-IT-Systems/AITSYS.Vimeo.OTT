// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Rest;

using Newtonsoft.Json.Linq;

namespace AITSYS.Vimeo.Ott.Exceptions;

/// <summary>
///     Represents an exception thrown when the request sent to vimeo is too large.
/// </summary>
public sealed class RequestSizeException : Exception
{
	/// <summary>
	///     Initializes a new instance of the <see cref="RequestSizeException" /> class.
	/// </summary>
	/// <param name="request">The request.</param>
	/// <param name="response">The response.</param>
	internal RequestSizeException(BaseRestRequest request, RestResponse response)
		: base($"Request entity too large: {response.ResponseCode}. Make sure the data sent is within Discord's upload limit.")
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
