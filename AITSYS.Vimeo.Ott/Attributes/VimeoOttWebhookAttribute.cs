namespace AITSYS.Vimeo.Ott.Attributes;

/// <summary>
/// Marks a method to handle a topic for incoming ott webhooks.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VimeoOttWebhookAttribute : Attribute
{
	/// <summary>
	/// Marks a method to handle a topic for incoming ott webhooks.
	/// </summary>
	/// <param name="topic">The topic to handle.</param>
	public VimeoOttWebhookAttribute(string topic)
	{
		this.Topic = topic;
	}

	/// <summary>
	/// Gets the webhooks topic.
	/// </summary>
	public string Topic { get; }
}
