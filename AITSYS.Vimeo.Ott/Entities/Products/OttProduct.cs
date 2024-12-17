// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;
using AITSYS.Vimeo.Ott.Entities.Links;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents a product.
/// </summary>
/// <typeparam name="TOttEmbedded">The type of the embedded object.</typeparam>
public sealed class OttProduct<TOttEmbedded> : OttIdObject<OttProductLinks, TOttEmbedded> where TOttEmbedded : IOttEmbedded
{
	/// <summary>
	///     Gets the product name.
	/// </summary>
	[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
	public string Name { get; internal set; }

	/// <summary>
	///     Gets the product description.
	/// </summary>
	[JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
	public string Description { get; internal set; }

	/// <summary>
	///     Gets the product thumbnail.
	/// </summary>
	[JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? Thumbnail { get; internal set; }

	/// <summary>
	///     Gets this products price configuration.
	/// </summary>
	[JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
	public OttPrice? Price { get; internal set; }

	/// <summary>
	///     Gets whether this product is active.
	/// </summary>
	[JsonProperty("is_active", NullValueHandling = NullValueHandling.Ignore)]
	public bool IsActive { get; internal set; }

	/// <summary>
	///     Gets the creation date of this product.
	/// </summary>
	[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime CreatedAt { get; internal set; }

	/// <summary>
	///     Gets the last time this product was updated.
	/// </summary>
	[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime UpdatedAt { get; internal set; }

	/// <summary>
	///     Gets the types of how it's distributed.
	///     Can be <c>purchase</c>, <c>free</c>, <c>rental</c> or <c>pwyw</c>.
	/// </summary>
	[JsonProperty("types", NullValueHandling = NullValueHandling.Ignore)]
	public IReadOnlyList<string> Types { get; internal set; } = [];

	/// <summary>
	///     Gets the sku.
	/// </summary>
	[JsonProperty("sku", NullValueHandling = NullValueHandling.Ignore)]
	public string Sku { get; internal set; }

	/// <summary>
	///     Gets the count of customer 'owning' this product.
	/// </summary>
	[JsonProperty("customer_count", NullValueHandling = NullValueHandling.Ignore)]
	public int CustomerCount { get; internal set; }

	/// <summary>
	///     Gets the count of videos in this product.
	/// </summary>
	[JsonProperty("videos_count", NullValueHandling = NullValueHandling.Ignore)]
	public int? VideosCount { get; internal set; }

	/// <summary>
	///     Gets the position of this product.
	/// </summary>
	[JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
	public int? Position { get; internal set; }

	/// <summary>
	///     Gets whether free registration is available for this product.
	/// </summary>
	[JsonProperty("free_registration", NullValueHandling = NullValueHandling.Ignore)]
	public bool FreeRegistration { get; internal set; }

	/// <summary>
	///     Gets the tier configuration. // Idk what this is..
	/// </summary>
	[JsonProperty("tier", NullValueHandling = NullValueHandling.Ignore)]
	public OttTier? Tier { get; internal set; }

	/// <summary>
	///     Gets the additional product images.
	/// </summary>
	[JsonProperty("additional_images", NullValueHandling = NullValueHandling.Ignore)]
	public OttAdditionalImages? AdditionalImages { get; internal set; }

	/// <summary>
	///     Gets the text content for ads.
	/// </summary>
	[JsonProperty("ads_txt_content", NullValueHandling = NullValueHandling.Ignore)]
	public string? AdsTxtContent { get; internal set; }

	/// <summary>
	///     Gets the advertising url.
	/// </summary>
	[JsonProperty("advertising_url", NullValueHandling = NullValueHandling.Ignore)]
	public Uri? AdvertisingUrl { get; internal set; }

	/// <summary>
	///     Gets the count of categories for this product.
	/// </summary>
	[JsonProperty("categories_count", NullValueHandling = NullValueHandling.Ignore)]
	public int CategoriesCount { get; internal set; }

	/// <summary>
	///     Gets the custom checkout url for this product.
	/// </summary>
	[JsonProperty("custom_checkout_url", NullValueHandling = NullValueHandling.Ignore)]
	public Uri? CustomCheckoutUrl { get; internal set; }

	/// <summary>
	///     Gets the custom email message customers receive upon purchasing for this product.
	/// </summary>
	[JsonProperty("custom_email_message", NullValueHandling = NullValueHandling.Ignore)]
	public string? CustomEmailMessage { get; internal set; }

	/// <summary>
	///     Gets whether free registration is enabled for this product.
	/// </summary>
	[JsonProperty("free_registration_enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool FreeRegistrationEnabled { get; internal set; }

	/// <summary>
	///     Gets the free trail configuration for this product.
	/// </summary>
	[JsonProperty("free_trial", NullValueHandling = NullValueHandling.Ignore)]
	public OttFreeTrail? FreeTrial { get; internal set; }

	/// <summary>
	///     Gets the geo-block configuration for this product.
	/// </summary>
	[JsonProperty("geoblock", NullValueHandling = NullValueHandling.Ignore)]
	public OttGeoblock? Geoblock { get; internal set; }

	/// <summary>
	///     Gets whether this product has content.
	/// </summary>
	[JsonProperty("has_content", NullValueHandling = NullValueHandling.Ignore)]
	public bool HasContent { get; internal set; }

	/// <summary>
	///     Gets whether this product manages its own checkout.
	/// </summary>
	[JsonProperty("manages_own_checkout", NullValueHandling = NullValueHandling.Ignore)]
	public bool ManagesOwnCheckout { get; internal set; }

	/// <summary>
	///     Gets the mapped product ids.
	/// </summary>
	[JsonProperty("mapped_product_ids", NullValueHandling = NullValueHandling.Ignore)]
	public IReadOnlyList<int> MappedProductIds { get; internal set; } = [];

	/// <summary>
	///     Gets the products metadata.
	/// </summary>
	[JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
	public OttProductMetadata? Metadata { get; internal set; }

	/// <summary>
	///     Gets the count of movies for this product.
	/// </summary>
	[JsonProperty("movies_count", NullValueHandling = NullValueHandling.Ignore)]
	public int MoviesCount { get; internal set; }

	/// <summary>
	///     Gets whether multi currency is enabled for this product.
	/// </summary>
	[JsonProperty("multi_currency_enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool MultiCurrencyEnabled { get; internal set; }

	/// <summary>
	///     Gets the count of playlists for this product.
	/// </summary>
	[JsonProperty("playlists_count", NullValueHandling = NullValueHandling.Ignore)]
	public int PlaylistsCount { get; internal set; }

	/// <summary>
	///     Gets the id of the trailer video for this product.
	/// </summary>
	[JsonProperty("trailer_video_id", NullValueHandling = NullValueHandling.Ignore)]
	public int? TrailerVideoId { get; internal set; }

	/// <summary>
	///     Gets the access period in <c>seconds</c> for renting.
	/// </summary>
	[JsonProperty("rental_access_period", NullValueHandling = NullValueHandling.Ignore)]
	public int? RentalAccessPeriod { get; internal set; }

	/// <summary>
	///     Gets the count of sections for this product.
	/// </summary>
	[JsonProperty("sections_count", NullValueHandling = NullValueHandling.Ignore)]
	public int SectionsCount { get; internal set; }

	/// <summary>
	///     Gets the type of sale for this product.
	///     Can be <c>sale</c> or <c>donation</c>.
	/// </summary>
	[JsonProperty("sale_type", NullValueHandling = NullValueHandling.Ignore)]
	public string? SaleType { get; internal set; }

	/// <summary>
	///     Gets the count of series for this product.
	/// </summary>
	[JsonProperty("series_count", NullValueHandling = NullValueHandling.Ignore)]
	public int SeriesCount { get; internal set; }

	/// <summary>
	///     Gets the tags for this product. // Type is unknown atm..
	/// </summary>
	[JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
	public object? Tags { get; internal set; }

	/// <summary>
	///     Gets whether this product can only be purchased on a monthly base.
	/// </summary>
	[JsonProperty("monthly_only", NullValueHandling = NullValueHandling.Ignore)]
	public bool MonthlyOnly { get; internal set; }

	/// <summary>
	///     Gets whether this product can only be purchased on a yearly base.
	/// </summary>
	[JsonProperty("annual_only", NullValueHandling = NullValueHandling.Ignore)]
	public bool AnnualOnly { get; internal set; }

	/// <summary>
	///     Gets the preorder configuration.
	/// </summary>
	[JsonProperty("preorder", NullValueHandling = NullValueHandling.Ignore)]
	public OttPreOrder? PreOrder { get; internal set; }
}
