// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.OTT.Entities;

namespace AITSYS.Vimeo.Ott;

public sealed class VimeoOttClient(VimeoOttConfiguration configuration)
{
	internal VimeoOttConfiguration Configuration { get; set; } = configuration;
}
