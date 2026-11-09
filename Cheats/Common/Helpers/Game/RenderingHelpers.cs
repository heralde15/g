using System.Collections.Generic;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class RenderingHelpers
{
	public static bool IsForcedFilter(IDictionary<string, VisualTweaks.ShaderControlType> filters, bool useOverrides, VisualTweaks.ShaderControlType controlType, string key)
	{
		if (useOverrides && filters.TryGetValue(key, out var value))
		{
			return value == controlType;
		}
		return false;
	}

	public static bool IsSceneFilterForced(string key)
	{
		return IsForcedFilter(TweaksCollectionExtensions.get_Current<VisualTweaks>().SceneFilters, TweaksCollectionExtensions.get_Current<VisualTweaks>().UseSceneFilterOverrides, VisualTweaks.ShaderControlType.Forced, key);
	}

	public static bool IsSkyEffectForced(string key)
	{
		return IsForcedFilter(TweaksCollectionExtensions.get_Current<VisualTweaks>().SkyFilters, TweaksCollectionExtensions.get_Current<VisualTweaks>().UseSkyFilterOverrides, VisualTweaks.ShaderControlType.Forced, key);
	}

	public static bool IsSceneFilterDisabled(string key)
	{
		return IsForcedFilter(TweaksCollectionExtensions.get_Current<VisualTweaks>().SceneFilters, TweaksCollectionExtensions.get_Current<VisualTweaks>().UseSceneFilterOverrides, VisualTweaks.ShaderControlType.Disabled, key);
	}

	public static bool IsSkyEffectDisabled(string key)
	{
		return IsForcedFilter(TweaksCollectionExtensions.get_Current<VisualTweaks>().SkyFilters, TweaksCollectionExtensions.get_Current<VisualTweaks>().UseSkyFilterOverrides, VisualTweaks.ShaderControlType.Disabled, key);
	}
}
