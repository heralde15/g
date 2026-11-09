using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerPlaceholder : UIState
{
	private UIList _uiList;

	public InnerPlaceholder()
	{
		UIElement uIElement = UIHelpers.CreateCommonBase();
		_uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = UIHelpers.CreateCommonScroller();
		_uiList.SetScrollbar(uIScrollbar);
		uIElement.Append(uIScrollbar);
		_uiList.Add(new TMLHeaderElement("Not yet implemented", mouseInteract: false, noLine: true));
		uIElement.Append(_uiList);
		Append(uIElement);
	}
}
