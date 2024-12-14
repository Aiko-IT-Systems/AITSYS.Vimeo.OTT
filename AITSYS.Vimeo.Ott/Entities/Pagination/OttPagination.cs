// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;
using AITSYS.Vimeo.Ott.Entities.Links;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Pagination;

/// <summary>
///     Represents a pagination result.
/// </summary>
/// <typeparam name="TOttEmbedded">The type of the embedded object.</typeparam>
public class OttPagination<TOttEmbedded> : OttObject<OttPaginationLinks, TOttEmbedded> where TOttEmbedded : IOttEmbedded
{
	/// <summary>
	///     Gets the count of results for the current page.
	/// </summary>
	[JsonProperty("count")]
	public int Count { get; internal set; }

	/// <summary>
	///     Gets the total count of results.
	/// </summary>
	[JsonProperty("total")]
	public int Total { get; internal set; }
}
