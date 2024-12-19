// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Net;
using System.Text;
using System.Web;

using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Rest;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott;

/// <summary>
///     Represents a utility class for various operations.
/// </summary>
public static class Utilities
{
	/// <summary>
	///     Gets the utf8 encoding
	/// </summary>
	// ReSharper disable once InconsistentNaming
	internal static UTF8Encoding UTF8 { get; } = new(false);

	/// <summary>
	///     Gets the formatted token.
	/// </summary>
	/// <param name="client">The client.</param>
	/// <returns>A string.</returns>
	internal static string GetFormattedToken(VimeoOttClient client)
	{
		var byteArray = Encoding.ASCII.GetBytes($"{client.Configuration.ApiKey}:");
		return $"{CommonHeaders.AUTHORIZATION_BASIC} {Convert.ToBase64String(byteArray)}";
	}

	/// <summary>
	///     Gets the base uri.
	/// </summary>
	/// <returns>The abse uri.</returns>
	public static string GetApiBaseUri()
		=> Endpoints.BASE_URI;

	/// <summary>
	///     Gets the user agent.
	/// </summary>
	/// <param name="client">The vimeo client.</param>
	/// <returns>The user agent header</returns>
	public static string GetUserAgent(VimeoOttClient client)
		=> $"{client.LibraryName} (https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT, v{client.LibraryVersion})";

	/// <summary>
	///     Gets the base headers.
	/// </summary>
	/// <returns>A Dictionary.</returns>
	internal static Dictionary<string, string> GetBaseHeaders()
		=> [];

	/// <summary>
	///     Gets the api uri for.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <returns>An Uri.</returns>
	internal static Uri GetApiUriFor(string path)
		=> new($"{GetApiBaseUri()}{path}");

	/// <summary>
	///     Gets the api uri for.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <param name="queryString">The query string.</param>
	/// <returns>An Uri.</returns>
	internal static Uri GetApiUriFor(string path, string queryString)
		=> new($"{GetApiBaseUri()}{path}{queryString}");

	/// <summary>
	///     Gets the api uri builder for.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <returns>A QueryUriBuilder.</returns>
	internal static QueryUriBuilder GetApiUriBuilderFor(string path)
		=> new($"{GetApiBaseUri()}{path}");

	/// <summary>
	///     Adds the specified parameter to the Query String.
	/// </summary>
	/// <param name="url"></param>
	/// <param name="paramName">Name of the parameter to add.</param>
	/// <param name="paramValue">Value for the parameter to add.</param>
	/// <returns>Url with added parameter.</returns>
	public static Uri AddParameter(this Uri url, string paramName, string paramValue)
	{
		var uriBuilder = new UriBuilder(url);
		var query = HttpUtility.ParseQueryString(uriBuilder.Query);
		query[paramName] = paramValue;
		uriBuilder.Query = query.ToString();

		return uriBuilder.Uri;
	}

	/// <summary>
	///     Builds the query string.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <param name="post">Whether this query will be transmitted via POST.</param>
	internal static string BuildQueryString(this IDictionary<string, string>? values, bool post = false)
	{
		if (values == null || values.Count == 0)
			return string.Empty;

		var valsCollection = values.Select(xkvp =>
			$"{WebUtility.UrlEncode(xkvp.Key)}={WebUtility.UrlEncode(xkvp.Value)}");
		var vals = string.Join("&", valsCollection);

		return !post ? $"?{vals}" : vals;
	}

	/// <summary>
	///     Checks whether this string contains given characters.
	/// </summary>
	/// <param name="str">String to check.</param>
	/// <param name="characters">Characters to check for.</param>
	/// <returns>Whether the string contained these characters.</returns>
	public static bool Contains(this string str, params char[] characters)
		=> str.Any(characters.Contains);

	/// <summary>
	///     Logs the task fault.
	/// </summary>
	/// <param name="task">The task.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="level">The level.</param>
	/// <param name="eventId">The event id.</param>
	/// <param name="message">The message.</param>
	/// <param name="args">An object array that contains zero or more objects to format.</param>
	internal static void LogTaskFault(this Task task, ILogger? logger, LogLevel level, EventId eventId, string? message, params object?[] args)
	{
		ArgumentNullException.ThrowIfNull(task);

		if (logger == null)
			return;

		task.ContinueWith(t => logger.Log(level, eventId, t.Exception, message, args), TaskContinuationOptions.OnlyOnFaulted);
	}

	/// <summary>
	///     Deconstructs the.
	/// </summary>
	/// <param name="kvp">The kvp.</param>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	internal static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
	{
		key = kvp.Key;
		value = kvp.Value;
	}
}
