// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

public sealed class OttProductMetadata
{
	[JsonProperty("bank_account_connected")]
	public bool BankAccountConnected { get; internal set; }

	[JsonProperty("has_branded_apps")]
	public bool HasBrandedApps { get; internal set; }

	[JsonProperty("is_multi_currency_enabled")]
	public bool IsMultiCurrencyEnabled { get; internal set; }

	[JsonProperty("is_whitelisted")]
	public bool IsWhitelisted { get; internal set; }

	[JsonProperty("pre_launch_preview_enabled")]
	public bool PreLaunchPreviewEnabled { get; internal set; }
}
