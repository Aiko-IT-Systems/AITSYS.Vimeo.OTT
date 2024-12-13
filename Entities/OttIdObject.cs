// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities;

/// <summary>
///     Represents an api object within ott with a unique id.
/// </summary>
/// <typeparam name="TOttHalLinks">The type of the HAL links.</typeparam>
public class OttIdObject<TOttHalLinks> : OttObject<TOttHalLinks> where TOttHalLinks : IHalLinks
{
	/// <summary>
	///     Gets the objects id.
	/// </summary>
	[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
	public int Id { get; internal set; }
}
