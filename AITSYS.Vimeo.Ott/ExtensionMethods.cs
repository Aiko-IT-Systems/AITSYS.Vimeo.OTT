// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Reflection;

using AITSYS.Vimeo.Ott.Attributes;
using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott;

/// <summary>
/// Represents various extension methods.
/// </summary>
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

	/// <summary>
	///     Maps methods marked with the <see cref="VimeoOttWebhookAttribute" /> to a specified pattern and applies the
	///     provided authentication scheme.
	/// </summary>
	/// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
	/// <param name="pattern">The URL pattern of the webhook endpoint.</param>
	/// <param name="authenticationScheme">The authentication scheme to apply to the endpoint.</param>
	/// <returns>The <see cref="IEndpointRouteBuilder" /> with the mapped webhook endpoint.</returns>
	public static IEndpointRouteBuilder MapVimeoOttWebhook(this IEndpointRouteBuilder endpoints, string pattern, string authenticationScheme)
	{
		var methods = Assembly.GetExecutingAssembly()
			.GetTypes()
			.SelectMany(t => t.GetMethods())
			.Where(m => m.GetCustomAttributes(typeof(VimeoOttWebhookAttribute), false).Length > 0)
			.ToArray();

		foreach (var method in methods)
		{
			var attribute = method.GetCustomAttribute<VimeoOttWebhookAttribute>();
			if (attribute != null)
				endpoints.MapPost(pattern, async context =>
					{
						var instance = Activator.CreateInstance(method.DeclaringType);
						var parameters = method.GetParameters().Select(p => context.RequestServices.GetService(p.ParameterType)).ToArray();
						await (Task)method.Invoke(instance, parameters);
					})
					.RequireAuthorization(new AuthorizeAttribute
					{
						AuthenticationSchemes = authenticationScheme
					});
		}

		return endpoints;
	}
}
