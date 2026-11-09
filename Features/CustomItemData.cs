using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.UtilClient.Features;

public class CustomItemData
{
	public int BaseID;

	public int? Damage;

	public int? CritChance;

	public float? Knockback;

	public float? Scale;

	public int? ManaCost;

	public int? UseTime;

	public int? AnimationTime;

	public int? UseStyle;

	public int? HoldStyle;

	public bool? AutoReusable;

	public bool? TurnWhileUsing;

	public bool? ChannelUse;

	public int? ReuseDelay;

	public int? ShotProjectile;

	public float? ProjectileSpeed;

	public int? AmmoUsed;

	public int? AmmoType;

	public bool? NeverGoesInAmmoSlot;

	public ushort? TimesToShoot;

	public int? ShootStyle;

	public bool? ShootsEveryUse;

	public int? HealedLife;

	public int? HealedMana;

	public bool? IsAPotion;

	public int? BuffAdded;

	public int? BuffDuration;

	public int? PickaxePower;

	public int? AxePower;

	public int? HammerPower;

	public int? FishingPower;

	public int? BaitPower;

	public int? Rarity;

	public bool? ExpertItem;

	public bool? OnlyUseableInExpert;

	public bool? IsQuestFish;

	public bool? UniqueItemStack;

	public bool? ConsumableStack;

	public int? MaxStackSize;

	public int? SellValue;

	public int? CreatesTile;

	public int? CreatesWall;

	public int? PlacementStyle;

	public int? TileRangeModifier;

	public int? ItemIdForWand;

	public bool? IsMelee;

	public bool? IsRanged;

	public bool? IsMagic;

	public bool? IsSummoner;

	public bool? IsSentry;

	public bool? IsDD2Sentry;

	public int? ArmorHeadSlot;

	public int? ArmorBodySlot;

	public int? ArmorLegSlot;

	public bool? IsAccessory;

	public bool? IsVanityItem;

	public sbyte? WingType;

	public int? Defense;

	public int? LifeRegenBonus;

	public int? ManaBonus;

	public bool? DoesNoMeleeDamage;

	public bool? NoUseGraphic;

	public bool? CanHitInvincibleNPCs;

	public string CustomName;

	public Color? Color;

	public bool IsValid;

	public bool BelongsToDynamicCollection;

	public string PresetName;

	public bool FromOtherClient;

	public CustomItemData(int itemID)
	{
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Item i = ContentSamples.ItemsByType[itemID];
		BaseID = itemID;
		Damage = i.damage;
		CritChance = i.crit;
		Knockback = i.knockBack;
		Scale = i.scale;
		ManaCost = i.mana;
		UseTime = i.useTime;
		AnimationTime = i.useAnimation;
		UseStyle = i.useStyle;
		HoldStyle = i.holdStyle;
		AutoReusable = i.autoReuse;
		TurnWhileUsing = i.useTurn;
		ChannelUse = i.channel;
		ReuseDelay = i.reuseDelay;
		ShotProjectile = i.shoot;
		ProjectileSpeed = i.shootSpeed;
		AmmoUsed = i.useAmmo;
		AmmoType = i.ammo;
		NeverGoesInAmmoSlot = i.notAmmo;
		TimesToShoot = (ushort)1;
		ShootsEveryUse = i.shootsEveryUse;
		HealedLife = i.healLife;
		HealedMana = i.healMana;
		BuffAdded = i.buffType;
		BuffDuration = i.buffTime;
		PickaxePower = i.pick;
		AxePower = i.axe;
		HammerPower = i.hammer;
		FishingPower = i.fishingPole;
		BaitPower = i.bait;
		Rarity = i.rare;
		ExpertItem = i.expert;
		OnlyUseableInExpert = i.expertOnly;
		IsQuestFish = i.questItem;
		UniqueItemStack = i.uniqueStack;
		ConsumableStack = i.consumable;
		MaxStackSize = i.maxStack;
		SellValue = i.value;
		CreatesTile = i.createTile;
		CreatesWall = i.createWall;
		PlacementStyle = i.placeStyle;
		TileRangeModifier = i.tileBoost;
		ItemIdForWand = i.tileWand;
		IsMelee = i.melee;
		IsRanged = i.ranged;
		IsMagic = i.magic;
		IsSummoner = i.summon;
		IsSentry = i.sentry;
		IsDD2Sentry = i.DD2Summon;
		ArmorHeadSlot = i.headSlot;
		ArmorBodySlot = i.bodySlot;
		ArmorLegSlot = i.legSlot;
		IsAccessory = i.accessory;
		IsVanityItem = i.vanity;
		WingType = i.wingSlot;
		Defense = i.defense;
		LifeRegenBonus = i.lifeRegen;
		ManaBonus = i.manaIncrease;
		DoesNoMeleeDamage = i.noMelee;
		NoUseGraphic = i.noUseGraphic;
		CanHitInvincibleNPCs = false;
		CustomName = i.NameOverride;
		Color = i.color;
	}

	public void SetDefaultValues()
	{
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		Damage = -1;
		CritChance = 0;
		Knockback = 0f;
		Scale = 1f;
		ManaCost = 0;
		UseTime = 20;
		AnimationTime = 20;
		UseStyle = 0;
		HoldStyle = 0;
		AutoReusable = false;
		TurnWhileUsing = false;
		ChannelUse = false;
		ReuseDelay = 0;
		ShotProjectile = 0;
		ProjectileSpeed = 0f;
		AmmoUsed = AmmoID.None;
		AmmoType = AmmoID.None;
		NeverGoesInAmmoSlot = false;
		TimesToShoot = (ushort)1;
		ShootsEveryUse = false;
		HealedLife = 0;
		HealedMana = 0;
		BuffAdded = 0;
		BuffDuration = 0;
		PickaxePower = 0;
		AxePower = 0;
		HammerPower = 0;
		FishingPower = 0;
		BaitPower = 0;
		Rarity = 0;
		ExpertItem = false;
		OnlyUseableInExpert = false;
		IsQuestFish = false;
		UniqueItemStack = false;
		ConsumableStack = false;
		MaxStackSize = 1;
		SellValue = 0;
		CreatesTile = -1;
		CreatesWall = -1;
		PlacementStyle = 0;
		TileRangeModifier = 0;
		ItemIdForWand = -1;
		IsMelee = false;
		IsRanged = false;
		IsMagic = false;
		IsSummoner = false;
		IsSentry = false;
		IsDD2Sentry = false;
		ArmorHeadSlot = -1;
		ArmorBodySlot = -1;
		ArmorLegSlot = -1;
		IsAccessory = false;
		IsVanityItem = false;
		WingType = (sbyte)(-1);
		Defense = 0;
		LifeRegenBonus = 0;
		ManaBonus = 0;
		DoesNoMeleeDamage = false;
		NoUseGraphic = false;
		CanHitInvincibleNPCs = false;
		CustomName = null;
		Color = Color.White;
	}

	public void ApplyTo(Item item)
	{
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		if (item.type == 6196)
		{
			if (Damage.HasValue)
			{
				item.damage = Damage.Value;
			}
			if (CritChance.HasValue)
			{
				item.crit = CritChance.Value;
			}
			if (Knockback.HasValue)
			{
				item.knockBack = Knockback.Value;
			}
			if (Scale.HasValue)
			{
				item.scale = Scale.Value;
			}
			if (ManaCost.HasValue)
			{
				item.mana = ManaCost.Value;
			}
			if (UseTime.HasValue)
			{
				item.useTime = UseTime.Value;
			}
			if (AnimationTime.HasValue)
			{
				item.useAnimation = AnimationTime.Value;
			}
			if (UseStyle.HasValue)
			{
				item.useStyle = UseStyle.Value;
			}
			if (HoldStyle.HasValue)
			{
				item.holdStyle = HoldStyle.Value;
			}
			if (AutoReusable.HasValue)
			{
				item.autoReuse = AutoReusable.Value;
			}
			if (TurnWhileUsing.HasValue)
			{
				item.useTurn = TurnWhileUsing.Value;
			}
			if (ChannelUse.HasValue)
			{
				item.channel = ChannelUse.Value;
			}
			if (ReuseDelay.HasValue)
			{
				item.reuseDelay = ReuseDelay.Value;
			}
			if (ShotProjectile.HasValue)
			{
				item.shoot = ShotProjectile.Value;
			}
			if (ProjectileSpeed.HasValue)
			{
				item.shootSpeed = ProjectileSpeed.Value;
			}
			if (AmmoUsed.HasValue)
			{
				item.useAmmo = AmmoUsed.Value;
			}
			if (AmmoType.HasValue)
			{
				item.ammo = AmmoType.Value;
			}
			if (NeverGoesInAmmoSlot.HasValue)
			{
				item.notAmmo = NeverGoesInAmmoSlot.Value;
			}
			if (ShootsEveryUse.HasValue)
			{
				item.shootsEveryUse = ShootsEveryUse.Value;
			}
			if (HealedLife.HasValue)
			{
				item.healLife = HealedLife.Value;
			}
			if (HealedMana.HasValue)
			{
				item.healMana = HealedMana.Value;
			}
			if (BuffAdded.HasValue)
			{
				item.buffType = BuffAdded.Value;
			}
			if (BuffDuration.HasValue)
			{
				item.buffTime = BuffDuration.Value;
			}
			if (PickaxePower.HasValue)
			{
				item.pick = PickaxePower.Value;
			}
			if (AxePower.HasValue)
			{
				item.axe = AxePower.Value;
			}
			if (HammerPower.HasValue)
			{
				item.hammer = HammerPower.Value;
			}
			if (FishingPower.HasValue)
			{
				item.fishingPole = FishingPower.Value;
			}
			if (BaitPower.HasValue)
			{
				item.bait = BaitPower.Value;
			}
			if (Rarity.HasValue)
			{
				item.rare = Rarity.Value;
			}
			if (ExpertItem.HasValue)
			{
				item.expert = ExpertItem.Value;
			}
			if (OnlyUseableInExpert.HasValue)
			{
				item.expertOnly = OnlyUseableInExpert.Value;
			}
			if (IsQuestFish.HasValue)
			{
				item.questItem = IsQuestFish.Value;
			}
			if (UniqueItemStack.HasValue)
			{
				item.uniqueStack = UniqueItemStack.Value;
			}
			if (ConsumableStack.HasValue)
			{
				item.consumable = ConsumableStack.Value;
			}
			if (MaxStackSize.HasValue)
			{
				item.maxStack = MaxStackSize.Value;
			}
			if (SellValue.HasValue)
			{
				item.value = SellValue.Value;
			}
			if (CreatesTile.HasValue)
			{
				item.createTile = CreatesTile.Value;
			}
			if (CreatesWall.HasValue)
			{
				item.createWall = CreatesWall.Value;
			}
			if (PlacementStyle.HasValue)
			{
				item.placeStyle = PlacementStyle.Value;
			}
			if (TileRangeModifier.HasValue)
			{
				item.tileBoost = TileRangeModifier.Value;
			}
			if (ItemIdForWand.HasValue)
			{
				item.tileWand = ItemIdForWand.Value;
			}
			if (IsMelee.HasValue)
			{
				item.melee = IsMelee.Value;
			}
			if (IsRanged.HasValue)
			{
				item.ranged = IsRanged.Value;
			}
			if (IsMagic.HasValue)
			{
				item.magic = IsMagic.Value;
			}
			if (IsSummoner.HasValue)
			{
				item.summon = IsSummoner.Value;
			}
			if (IsSentry.HasValue)
			{
				item.sentry = IsSentry.Value;
			}
			if (IsDD2Sentry.HasValue)
			{
				item.DD2Summon = IsDD2Sentry.Value;
			}
			if (ArmorHeadSlot.HasValue)
			{
				item.headSlot = ArmorHeadSlot.Value;
			}
			if (ArmorBodySlot.HasValue)
			{
				item.bodySlot = ArmorBodySlot.Value;
			}
			if (ArmorLegSlot.HasValue)
			{
				item.legSlot = ArmorLegSlot.Value;
			}
			if (IsAccessory.HasValue)
			{
				item.accessory = IsAccessory.Value;
			}
			if (IsVanityItem.HasValue)
			{
				item.vanity = IsVanityItem.Value;
			}
			if (WingType.HasValue)
			{
				item.wingSlot = WingType.Value;
			}
			if (Defense.HasValue)
			{
				item.defense = Defense.Value;
			}
			if (LifeRegenBonus.HasValue)
			{
				item.lifeRegen = LifeRegenBonus.Value;
			}
			if (ManaBonus.HasValue)
			{
				item.manaIncrease = ManaBonus.Value;
			}
			if (DoesNoMeleeDamage.HasValue)
			{
				item.noMelee = DoesNoMeleeDamage.Value;
			}
			if (NoUseGraphic.HasValue)
			{
				item.noUseGraphic = NoUseGraphic.Value;
			}
			if (!string.IsNullOrWhiteSpace(CustomName))
			{
				item.SetNameOverride(Language.GetTextValue(CustomName));
			}
			if (Color.HasValue)
			{
				item.color = Color.Value;
			}
		}
	}
}
