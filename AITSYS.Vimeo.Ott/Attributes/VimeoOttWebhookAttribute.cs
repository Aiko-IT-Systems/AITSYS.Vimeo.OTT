using AITSYS.Vimeo.Ott.Entities;

namespace AITSYS.Vimeo.Ott.Attributes;

/// <summary>
///     Marks a method to handle one or more topics for incoming OTT webhooks.
/// <para>You can create a method like this:</para>
/// <code>
/// [VimeoOttWebhook("customer.product.created", "customer.product.deleted")]
/// async Task&lt;ActionResult&gt; HandleVimeoOttWebhookAsync(OttWebhook webhook)
/// {
///      // do stuff
///      return new OkResult();
/// }
/// </code>
/// <para>Important is that you define the parameter <c><see cref="OttWebhook">OttWebhook</see> webhook</c>, since we populate to this parameter.</para>
/// </summary>
/// <param name="topics">The topics to handle.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VimeoOttWebhookAttribute(params string[] topics) : Attribute
{
	/// <summary>
	///     Gets the webhooks topics.
	/// </summary>
	public IEnumerable<string> Topics { get; } = topics;
}
