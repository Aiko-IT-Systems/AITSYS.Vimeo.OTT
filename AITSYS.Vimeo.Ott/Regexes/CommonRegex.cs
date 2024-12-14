// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Text.RegularExpressions;

namespace AITSYS.Vimeo.Ott.Regexes;

internal partial class CommonRegex
{
	[GeneratedRegex(":([a-z_]+)", RegexOptions.Compiled | RegexOptions.ECMAScript)]
	public static partial Regex HttpRouteRegex();
}
