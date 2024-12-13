// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

namespace AITSYS.Vimeo.Ott.Interfaces;

public interface IOttObject<out TOttHalLinks, out TOttEmbedded>
	where TOttHalLinks : IHalLinks
	where TOttEmbedded : IOttEmbedded
{
	TOttHalLinks Links { get; }
	TOttEmbedded Embedded { get; }
}
