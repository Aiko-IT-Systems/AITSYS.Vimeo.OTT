namespace AITSYS.Vimeo.Ott.Attributes;

/// <summary>
///     Marks a method to handle one or more topics for incoming OTT webhooks.
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
