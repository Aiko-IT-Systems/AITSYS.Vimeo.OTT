// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;
using AITSYS.Vimeo.Ott.Entities.Base;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Links;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

/// <summary>
///     Represents a customer.
/// </summary>
/// <typeparam name="TOttEmbedded">The type of the embedded object.</typeparam>
public sealed class OttCustomer<TOttEmbedded> : OttIdObject<OttCustomerLinks, TOttEmbedded> where TOttEmbedded : IOttEmbedded
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
	[JsonProperty("thumbnail")]
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
	[JsonProperty("marketing_opt_in", NullValueHandling = NullValueHandling.Ignore)]
	public bool? MarketingOptIn { get; internal set; }

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
	///     The campaign associated with the customer.
	/// </summary>
	[JsonProperty("campaign", NullValueHandling = NullValueHandling.Ignore)]
	public string? Campaign { get; set; }

	/// <summary>
	///     The coupon code used by the customer.
	/// </summary>
	[JsonProperty("coupon_code", NullValueHandling = NullValueHandling.Ignore)]
	public string? CouponCode { get; set; }

	/// <summary>
	///     The first name of the customer.
	/// </summary>
	[JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
	public string FirstName { get; set; }

	/// <summary>
	///     The last name of the customer.
	/// </summary>
	[JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
	public string LastName { get; set; }

	/// <summary>
	///     The date of the last payment made by the customer.
	/// </summary>
	[JsonProperty("last_payment_date", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? LastPaymentDate { get; set; }

	/// <summary>
	///     The date of the next payment due from the customer.
	/// </summary>
	[JsonProperty("next_payment_date", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? NextPaymentDate { get; set; }

	/// <summary>
	///     The end date of the customer's subscription pause.
	/// </summary>
	[JsonProperty("pause_end_date", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? PauseEndDate { get; set; }

	/// <summary>
	///     The promotion code used by the customer.
	/// </summary>
	[JsonProperty("promotion_code", NullValueHandling = NullValueHandling.Ignore)]
	public string? PromotionCode { get; set; }

	/// <summary>
	///     The referrer of the customer.
	/// </summary>
	[JsonProperty("referrer", NullValueHandling = NullValueHandling.Ignore)]
	public string? Referrer { get; set; }

	/// <summary>
	///     The subscription frequency of the customer.
	/// </summary>
	[JsonProperty("subscription_frequency", NullValueHandling = NullValueHandling.Ignore)]
	public string? SubscriptionFrequency { get; set; }

	/// <summary>
	///     The subscription price of the customer.
	/// </summary>
	[JsonProperty("subscription_price", NullValueHandling = NullValueHandling.Ignore)]
	public int? SubscriptionPrice { get; set; }

	/// <summary>
	///     The subscription status of the customer.
	/// </summary>
	[JsonProperty("subscription_status", NullValueHandling = NullValueHandling.Ignore)]
	public string SubscriptionStatus { get; set; }

	/// <summary>
	///     The customers notification settings.
	/// </summary>
	[JsonProperty("notification_settings")]
	public OttNotificationSettings NotificationSettings { get; internal set; }

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveCustomerEventsAsync" />
	public async Task<OttPagination<OttEventsEmbeddedData<OttEventProductObjectEmbeddedData>>> RetrieveEventsAsync()
		=> await this.Client.ApiClient.RetrieveCustomerEventsAsync(this.Id);

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveCustomerProductsAsync" />
	public async Task<OttPagination<OttCustomerProductEmbeddedData>> RetrieveProductsAsync()
		=> await this.Client.ApiClient.RetrieveCustomerProductsAsync(this.Id);

	/// <inheritdoc cref="VimeoOttApiClient.UpdateCustomerAsync" />
	public async Task<OttCustomer<OttCustomerProductEmbeddedData>?> UpdateAsync(string? name = null, string? password = null)
		=> await this.Client.ApiClient.UpdateCustomerAsync(this.Id, name, password);
}
