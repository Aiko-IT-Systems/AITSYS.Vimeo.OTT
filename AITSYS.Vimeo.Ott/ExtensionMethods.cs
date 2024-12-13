// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Logging;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott;

public static class ExtensionMethods
{
	/// <summary>
	///     Gets an authenticated api client for usage specific to a customer.
	///     <para>Note that this restricts the api endpoints heavily to the specific user.</para>
	///     <para>
	///         Vimeo's ratelimit respects the <paramref name="customerHref" /> and <paramref name="clientIp" /> for
	///         ratelimit.
	///     </para>
	/// </summary>
	/// <param name="baseClient">The base client to use.</param>
	/// <param name="customerHref">The customer href.</param>
	/// <param name="clientIp">The optional client ip.</param>
	/// <returns>A seperate authenticated api client bound to <paramref name="customerHref" />.</returns>
	public static VimeoOttClient GetClientForCustomer(this VimeoOttClient baseClient, string customerHref, string? clientIp = null)
	{
		var shadowConfig = baseClient.Configuration;
		shadowConfig.VhxCustomer = customerHref;
		shadowConfig.VhxClientIp = clientIp;
		baseClient.Logger.LogDebug(LoggerEvents.Library, "Creating a new client bound to {customer}", customerHref);
		return new(shadowConfig);
	}
}
