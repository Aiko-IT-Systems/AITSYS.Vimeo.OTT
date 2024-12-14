// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Net;

namespace AITSYS.Vimeo.Ott.Rest;

/// <summary>
///     Represents a response sent by the remote HTTP party.
/// </summary>
public sealed class RestResponse
{
	/// <summary>
	///     Gets the response code sent by the remote party.
	/// </summary>
	public HttpStatusCode ResponseCode { get; internal set; }

	/// <summary>
	///     Gets the headers sent by the remote party.
	/// </summary>
	public IReadOnlyDictionary<string, string>? Headers { get; internal set; }

	/// <summary>
	///     Gets the contents of the response sent by the remote party.
	/// </summary>
	public string Response { get; internal set; }
}
