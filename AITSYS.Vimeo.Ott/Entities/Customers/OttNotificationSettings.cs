// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

/// <summary>
///     Represents notification settings.
/// </summary>
public sealed class OttNotificationSettings
{
	/// <summary>
	///     Whether the customer is subscribed to comment parent notifications.
	/// </summary>
	[JsonProperty("comment_parent_notification")]
	public int CommentParentNotification { get; internal set; }

	/// <summary>
	///     Whether the customer is subscribed to forum posts notifications.
	/// </summary>
	[JsonProperty("forum_post_subscriber_notification")]
	public int ForumPostSubscriberNotification { get; internal set; }

	/// <summary>
	///     Whether the customer is subscribed to product video notifications.
	/// </summary>
	[JsonProperty("product_video_customer_notification")]
	public int ProductVideoCustomerNotification { get; internal set; }

	/// <summary>
	///     Whether the customer is subscribed to forum threads notifications.
	/// </summary>
	[JsonProperty("forum_thread_subscriber_notification")]
	public int ForumThreadSubscriberNotification { get; internal set; }
}
