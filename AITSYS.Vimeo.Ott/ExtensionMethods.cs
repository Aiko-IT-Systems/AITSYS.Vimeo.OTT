using System.Reflection;

using AITSYS.Vimeo.Ott.Attributes;
using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Entities;
using AITSYS.Vimeo.Ott.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott;

/// <summary>
///     Represents various extension methods.
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
	/// <param name="client">The <see cref="VimeoOttClient" /> to use for logging.</param>
	/// <param name="pattern">The URL pattern of the webhook endpoint.</param>
	/// <param name="authenticationScheme">The authentication scheme to apply to the endpoint.</param>
	/// <returns>The <see cref="IEndpointRouteBuilder" /> with the mapped webhook endpoint.</returns>
	public static IEndpointRouteBuilder MapVimeoOttWebhook(this IEndpointRouteBuilder endpoints, VimeoOttClient client, string pattern, string authenticationScheme)
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();

		var methods = assemblies
			.SelectMany(assembly => assembly.GetTypes())
			.SelectMany(type => type.GetMethods())
			.Where(method => method.GetCustomAttributes(typeof(VimeoOttWebhookAttribute), false).Length > 0)
			.ToArray();

		client.Logger.LogDebug("Found {count} methods with VimeoOttWebhookAttribute.", methods.Length);

		foreach (var method in methods)
		{
			var attribute = method.GetCustomAttribute<VimeoOttWebhookAttribute>();
			if (attribute is null)
				continue;

			client.Logger.LogInformation("Registering method '{name}' for {topics}", method.Name, string.Join(", ", attribute.Topics));
			endpoints.MapPost(pattern, async context =>
				{
					var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
					var needsReDeserialization = context.Request.ContentType != "application/json" && context.Request.ContentType != "application/json; charset=utf-8";
					var webhookData = needsReDeserialization ? JsonConvert.DeserializeObject<string>(body) : body;
					var webhook = JsonConvert.DeserializeObject<OttWebhook?>(webhookData!);
					if (webhook is not null && attribute.Topics.Contains(webhook.Topic))
					{
						var instance = context.RequestServices.GetService(method.DeclaringType!) ?? Activator.CreateInstance(method.DeclaringType!);
						var parameters = method.GetParameters().Select(p => p.ParameterType == typeof(OttWebhook) ? webhook : context.RequestServices.GetService(p.ParameterType)).ToArray();
						if (instance is not null)
						{
							client.Logger.LogTrace("Handling incoming VHX webhook with topic '{topic}'", webhook.Topic);
							client.Logger.LogTrace("Webhook data: {data}", webhookData ?? "null");
							await (Task)method.Invoke(instance, parameters)!;
						}
						else
						{
							client.Logger.LogError("Failed to resolve an instance of the method's declaring type while handling incoming VHX webhook with topic '{topic}'", webhook.Topic);
							context.Response.StatusCode = StatusCodes.Status500InternalServerError;
						}
					}
					else
						context.Response.StatusCode = StatusCodes.Status400BadRequest;
				})
				.RequireAuthorization(new AuthorizeAttribute
				{
					AuthenticationSchemes = authenticationScheme
				});
		}

		return endpoints;
	}
}
