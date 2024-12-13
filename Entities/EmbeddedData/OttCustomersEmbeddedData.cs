// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.EmbeddedData;

public sealed class OttCustomersEmbeddedData : IOttEmbedded
{
	[JsonProperty("customers", NullValueHandling = NullValueHandling.Ignore)]
	public IReadOnlyList<OttCustomer<OttCustomerLatestEventEmbeddedData>> Customers { get; internal set; } = [];
}
