using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.UI;

namespace Terraria.UtilClient.UI.Elements;

public class UISpawnDescriptiveItem(Func<string> name, Func<string> description, Action onSpawn, Func<bool>? disableCondition = null, Func<string>? getValue = null) : UIDescriptiveItem(name, description, getValue, disableCondition)
{
	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		UpdateTooltip();
	}

	public override void LeftClick(UIMouseEvent evt)
	{
		if (!DisableCondition())
		{
			base.LeftClick(evt);
			SoundEngine.PlaySound(11);
			onSpawn();
		}
	}
}
