using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Terraria.UtilClient.UI.Elements;

public class UICollapsibleGroup : UIElement
{
	private List<UIElement> _innerList;

	private UIImageFramed _dropdownArrow;

	private UIImageFramed _dropdownArrowOutline;

	private Asset<Texture2D> _packButtons;

	private Asset<Texture2D> _packButtonsOutline;

	private UIList _displayedList;

	private bool Opened;

	public UICollapsibleGroup(string key)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		_innerList = new List<UIElement>();
		base._002Ector();
		UIElement element = UIHelpers.CreateCommonBase();
		UIPanel behindPanel = new UIPanel();
		behindPanel.Width.Set(0f, 1f);
		behindPanel.Height.Set(0f, 1f);
		behindPanel.VAlign = 1f;
		behindPanel.BorderColor = Colors.BlueBorderColor;
		behindPanel.BackgroundColor = Colors.BlueBackgroundColor;
		_packButtons = TextureAssets.TexturePackButtons;
		_dropdownArrow = new UIImageFramed(_packButtons, _packButtons.Frame(2, 2, 1, 1))
		{
			HAlign = 1f,
			MarginRight = 4f,
			IgnoresMouseInteraction = true
		};
		_packButtonsOutline = Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtonsOutline", (AssetRequestMode)1);
		_dropdownArrowOutline = new UIImageFramed(_packButtonsOutline, _packButtonsOutline.Frame(2, 2, 1, 1))
		{
			HAlign = 0.5f,
			IgnoresMouseInteraction = true
		};
		_dropdownArrow.Append(_dropdownArrowOutline);
		_displayedList = new UIList();
		_displayedList.Width.Set(0f, 1f);
		_displayedList.Height.Set(0f, 1f);
		_displayedList.VAlign = 1f;
		_displayedList.ListPadding = 4f;
		_displayedList.OverflowHidden = false;
		_displayedList.ManualSortMethod = UIHelpers.NoSorting;
		_innerList.Add(CreateHeader(key));
		UIHorizontalSeparator seperator = new UIHorizontalSeparator
		{
			HAlign = 0.5f,
			Color = Colors.BlueBorderColor * 0.8f
		};
		seperator.Height.Set(2f, 0f);
		seperator.MarginTop = -6f;
		_innerList.Add(seperator);
		UpdateInner();
		element.Append(behindPanel);
		element.Append(_dropdownArrow);
		element.Append(_displayedList);
		Append(element);
	}

	private TMLHeaderElement CreateHeader(string key)
	{
		TMLHeaderElement tMLHeaderElement = new TMLHeaderElement(key, mouseInteract: true, noLine: true);
		tMLHeaderElement.Height.Set(32f, 0f);
		tMLHeaderElement.OnLeftClick += DropdownClick;
		tMLHeaderElement.OnUpdate += UpdateDropdownArrow;
		return tMLHeaderElement;
	}

	private void UpdateDropdownArrow(UIElement affectedElement)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_dropdownArrowOutline.Color = (affectedElement.IsMouseHovering ? Main.OurFavoriteColor : Color.Transparent);
		_dropdownArrowOutline.SetFrame(_packButtonsOutline.Frame(2, 2, 1, (!Opened) ? 1 : 0));
	}

	private void DropdownClick(UIMouseEvent evt, UIElement listeningElement)
	{
		Opened = !Opened;
		SoundEngine.PlaySound(Opened ? 10 : 11);
		_dropdownArrow.SetFrame(_packButtons.Frame(2, 2, 1, (!Opened) ? 1 : 0));
		if (_innerList.Count >= 2)
		{
			UpdateInner();
		}
	}

	private void UpdateInner()
	{
		_displayedList.Clear();
		if (Opened)
		{
			for (int i = 0; i < _innerList.Count; i++)
			{
				_displayedList.Add(_innerList[i]);
			}
		}
		else
		{
			_displayedList.Add(_innerList[0]);
		}
		_innerList[1].Width.Set(_innerList[0].GetDimensions().Width - 4f, 0f);
		Height.Set(_displayedList.GetTotalHeight(), 0f);
		Recalculate();
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
	}

	public override void Update(GameTime gameTime)
	{
		UpdateInner();
		base.Update(gameTime);
	}

	public void Add(UIElement element)
	{
		if (element != null)
		{
			_innerList.Add(element);
		}
	}
}
