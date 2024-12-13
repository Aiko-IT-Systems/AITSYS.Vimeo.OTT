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
	[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
	public string Name { get; internal set; }

	[JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
	public string Description { get; internal set; }

	[JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? Thumbnail { get; internal set; }

	[JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
	public OttPrice? Price { get; internal set; }

	[JsonProperty("is_active", NullValueHandling = NullValueHandling.Ignore)]
	public bool IsActive { get; internal set; }

	[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime CreatedAt { get; internal set; }

	[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime UpdatedAt { get; internal set; }

	[JsonProperty("types", NullValueHandling = NullValueHandling.Ignore)]
	public IReadOnlyList<string> Types { get; internal set; } = [];

	[JsonProperty("sku", NullValueHandling = NullValueHandling.Ignore)]
	public string Sku { get; internal set; }

	[JsonProperty("customer_count", NullValueHandling = NullValueHandling.Ignore)]
	public int CustomerCount { get; internal set; }

	[JsonProperty("videos_count", NullValueHandling = NullValueHandling.Ignore)]
	public int? VideosCount { get; internal set; }

	[JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
	public int? Position { get; internal set; }

	[JsonProperty("free_registration", NullValueHandling = NullValueHandling.Ignore)]
	public bool FreeRegistration { get; internal set; }

	[JsonProperty("tier", NullValueHandling = NullValueHandling.Ignore)]
	public OttTier? Tier { get; internal set; }

	[JsonProperty("additional_images", NullValueHandling = NullValueHandling.Ignore)]
	public OttAdditionalImages? AdditionalImages { get; internal set; }

	[JsonProperty("ads_txt_content", NullValueHandling = NullValueHandling.Ignore)]
	public string? AdsTxtContent { get; internal set; }

	[JsonProperty("advertising_url", NullValueHandling = NullValueHandling.Ignore)]
	public Uri? AdvertisingUrl { get; internal set; }

	[JsonProperty("categories_count", NullValueHandling = NullValueHandling.Ignore)]
	public int CategoriesCount { get; internal set; }

	[JsonProperty("custom_checkout_url", NullValueHandling = NullValueHandling.Ignore)]
	public Uri? CustomCheckoutUrl { get; internal set; }

	[JsonProperty("custom_email_message", NullValueHandling = NullValueHandling.Ignore)]
	public string? CustomEmailMessage { get; internal set; }

	[JsonProperty("free_registration_enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool FreeRegistrationEnabled { get; internal set; }

	[JsonProperty("free_trial", NullValueHandling = NullValueHandling.Ignore)]
	public OttFreeTrail? FreeTrial { get; internal set; }

	[JsonProperty("geoblock", NullValueHandling = NullValueHandling.Ignore)]
	public OttGeoblock? Geoblock { get; internal set; }

	[JsonProperty("has_content", NullValueHandling = NullValueHandling.Ignore)]
	public bool HasContent { get; internal set; }

	[JsonProperty("manages_own_checkout", NullValueHandling = NullValueHandling.Ignore)]
	public bool ManagesOwnCheckout { get; internal set; }

	[JsonProperty("mapped_product_ids", NullValueHandling = NullValueHandling.Ignore)]
	public IReadOnlyList<int> MappedProductIds { get; internal set; } = [];

	[JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
	public OttProductMetadata? Metadata { get; internal set; }

	[JsonProperty("movies_count", NullValueHandling = NullValueHandling.Ignore)]
	public int MoviesCount { get; internal set; }

	[JsonProperty("multi_currency_enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool MultiCurrencyEnabled { get; internal set; }

	[JsonProperty("playlists_count", NullValueHandling = NullValueHandling.Ignore)]
	public int PlaylistsCount { get; internal set; }

	[JsonProperty("trailer_video_id", NullValueHandling = NullValueHandling.Ignore)]
	public int? TrailerVideoId { get; internal set; }

	// Access period is in seconds
	[JsonProperty("rental_access_period", NullValueHandling = NullValueHandling.Ignore)]
	public int? RentalAccessPeriod { get; internal set; }

	[JsonProperty("sections_count", NullValueHandling = NullValueHandling.Ignore)]
	public int SectionsCount { get; internal set; }

	[JsonProperty("sale_type", NullValueHandling = NullValueHandling.Ignore)]
	public string? SaleType { get; internal set; }

	[JsonProperty("series_count", NullValueHandling = NullValueHandling.Ignore)]
	public int SeriesCount { get; internal set; }

	[JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
	public object? Tags { get; internal set; }

	[JsonProperty("monthly_only", NullValueHandling = NullValueHandling.Ignore)]
	public bool MonthlyOnly { get; internal set; }

	[JsonProperty("annual_only", NullValueHandling = NullValueHandling.Ignore)]
	public bool AnnualOnly { get; internal set; }
	
	// TODO: add preorder {enabled: bool}
}
