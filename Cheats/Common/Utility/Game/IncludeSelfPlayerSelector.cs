namespace Terraria.UtilClient.Common.Utility.Game;

public sealed class IncludeSelfPlayerSelector : PlayerSelector
{
	public override bool AllIncludesSelf => true;
}
