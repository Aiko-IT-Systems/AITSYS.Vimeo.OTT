// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents product metadata.
/// </summary>
public sealed class OttProductMetadata
{
	/// <summary>
	///     Gets whether the site has a bank account connected.
	/// </summary>
	[JsonProperty("bank_account_connected")]
	public bool BankAccountConnected { get; internal set; }

	/// <summary>
	///     Gets whether the site has branded apps.
	/// </summary>
	[JsonProperty("has_branded_apps")]
	public bool HasBrandedApps { get; internal set; }

	/// <summary>
	///     Gets whether the site allows products to be sold in multiple currencies.
	/// </summary>
	[JsonProperty("is_multi_currency_enabled")]
	public bool IsMultiCurrencyEnabled { get; internal set; }

	/// <summary>
	///     Gets whether the site is whitelisted-only? // Actually unsure about this..
	/// </summary>
	[JsonProperty("is_whitelisted")]
	public bool IsWhitelisted { get; internal set; }

	/// <summary>
	///     Gets whether this products pre launch preview is enabled.
	/// </summary>
	[JsonProperty("pre_launch_preview_enabled")]
	public bool PreLaunchPreviewEnabled { get; internal set; }
}
