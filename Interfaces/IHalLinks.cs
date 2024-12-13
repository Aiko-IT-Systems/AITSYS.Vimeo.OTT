// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities;

namespace AITSYS.Vimeo.Ott.Interfaces;

/// <summary>
///     Represents a HAL link.
/// </summary>
public interface IHalLinks
{
	/// <summary>
	///     Gets the link to itself.
	/// </summary>
	public OttHalLink Self { get; }
}
