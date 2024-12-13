// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

public sealed class OttEventData
{
	public string Action { get; internal set; }

	public string? ActionType { get; internal set; }

	public string? Status { get; internal set; }
	public string? Frequency { get; internal set; }
	public OttPrice? Price { get; internal set; }

	public string? Platform { get; internal set; }
}
