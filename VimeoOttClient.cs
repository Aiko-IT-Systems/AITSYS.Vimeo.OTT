using AITSYS.Vimeo.OTT.Entities;

namespace AITSYS.Vimeo.Ott;

public sealed class VimeoOttClient(VimeoOttConfiguration configuration)
{
	internal VimeoOttConfiguration Configuration { get; set; } = configuration;
}
