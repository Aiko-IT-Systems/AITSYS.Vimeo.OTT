// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Links;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

/// <summary>
///     Represents a customer.
/// </summary>
public sealed class OttCustomer : OttIdObject<OttCustomerLinks>
{
	/// <summary>
	///     The customer name.
	/// </summary>
	[JsonProperty("name")]
	public string Name { get; internal set; }

	/// <summary>
	///     The customer email.
	/// </summary>
	[JsonProperty("email")]
	public string Email { get; internal set; }

	/// <summary>
	///     The customer thumbnail.
	/// </summary>
	[JsonProperty("email")]
	public Uri? Thumbnail { get; internal set; }

	/// <summary>
	///     The customer location.
	/// </summary>
	[JsonProperty("location")]
	public OttLocation? Location { get; internal set; }

	/// <summary>
	///     Datetime when the customer was created.
	/// </summary>
	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; internal set; }

	/// <summary>
	///     Datetime when the customer was updated.
	/// </summary>
	[JsonProperty("updated_at")]
	public DateTime UpdatedAt { get; internal set; }

	/// <summary>
	///     The customers current plan.
	/// </summary>
	[JsonProperty("plan")]
	public string Plan { get; internal set; }

	/// <summary>
	///     Platform where the customer was created.
	/// </summary>
	[JsonProperty("platform")]
	public string Platform { get; internal set; }

	/// <summary>
	///     Whether the customer is opt-in to marketing emails.
	/// </summary>
	[JsonProperty("marketing_opt_in")]
	public bool MarketingOptIn { get; internal set; }

	/// <summary>
	///     Whether the customer is registered to the site.
	/// </summary>
	[JsonProperty("registered_to_site")]
	public bool RegisteredToSite { get; internal set; }

	/// <summary>
	///     Whether the customer is subscribed to the site.
	///     <c>
	///         This is from a concept that pre-dates FVOD product availability and should not be used to determine whether a
	///         user is subscribed.
	///     </c>
	/// </summary>
	[JsonProperty("subscribed_to_site")]
	public bool SubscribedToSite { get; internal set; }

	/// <summary>
	///     The customers notification settings.
	/// </summary>
	[JsonProperty("notification_settings")]
	public OttNotificationSettings NotificationSettings { get; internal set; }
}
