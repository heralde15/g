using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.UtilClient.Net;
using Terraria.UtilClient.Net.Protocol.Packets;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class NetHelper
{
	public static void SyncLocalPlayerControls(bool hooks = true)
	{
		PlayerControlsPacket.FromPlayer(Main.LocalPlayer).Send(1, hooks ? ClientPacket.ActiveHooks.All : ClientPacket.ActiveHooks.None);
	}

	public static void SyncLocalPlayerHealth(bool hooks = true)
	{
		PlayerHealthPacket.FromPlayer(Main.LocalPlayer).Send(1, hooks ? ClientPacket.ActiveHooks.All : ClientPacket.ActiveHooks.None);
	}

	public static void SyncLocalPlayerSlot(int slot, bool hooks = true)
	{
		SyncEquipmentPacket.FromLocalPlayerInventory((short)slot).Send(1, hooks ? ClientPacket.ActiveHooks.All : ClientPacket.ActiveHooks.None);
	}

	public static void FakePlayerControlsPacket(Entity e, int? selectedItem = null)
	{
		FakePlayerControlsPacket(new Vector2(e.Center.X - 10f, e.Center.Y - 21f), selectedItem);
	}

	public static void FakePlayerControlsPacket(Vector2? pos = null, int? selectedItem = null)
	{
		PlayerControlsPacket packet = PlayerControlsPacket.FromPlayer(Main.LocalPlayer);
		if (pos.HasValue)
		{
			packet.PlayerPhysics.Position = pos.Value;
		}
		if (selectedItem.HasValue)
		{
			packet.SelectedItem = (byte)selectedItem.Value;
		}
		packet.Send();
	}

	private static void FakeItemSlotPacket(short slot, short type, short stack = 1, byte prefix = 0, bool favorite = false, bool blockedSlot = false)
	{
		new SyncEquipmentPacket
		{
			PlayerSlot = (byte)Main.myPlayer,
			InventorySlot = slot,
			Stack = stack,
			Prefix = prefix,
			ItemType = type,
			SlotData = new SyncEquipmentPacket.SlotFlags
			{
				Favorite = favorite,
				Blocked = blockedSlot
			}
		}.Send();
	}

	public static void EasyFakeSlot(Vector2 worldPos, short itemType, short selectedItem = 58)
	{
		Player p = Main.LocalPlayer;
		FakePlayerControlsPacket(new Vector2(worldPos.X - 10f, worldPos.Y - (float)p.height / 2f), selectedItem);
		FakeItemSlotPacket(selectedItem, itemType, 1, 0);
	}

	public static void EasyFakeSlot(int x, int y, short itemType, short selectedItem = 58)
	{
		Player p = Main.LocalPlayer;
		FakePlayerControlsPacket(new Vector2((float)(x * 16) - 10f, (float)(y * 16) - (float)p.height / 2f), selectedItem);
		FakeItemSlotPacket(selectedItem, itemType, 1, 0);
	}

	public static void EasyPlacement(int x, int y, short itemType, short selectedItem = 58)
	{
		Player p = Main.LocalPlayer;
		FakePlayerControlsPacket(new Vector2((float)(x * 16) - 10f, (float)(y * 16) - (float)p.height / 2f), selectedItem);
		FakeItemSlotPacket(selectedItem, itemType, 1, 0);
		Item item = ContentSamples.ItemsByType[itemType];
		TileManipulationPacket.PlaceTile((short)x, (short)y, (short)item.createTile, (byte)item.placeStyle).Send();
	}

	public static async Task<int> RequestAllSections(int sleepTime = -1, CancellationToken? cancellationToken = null)
	{
		return await RequestSections(0, 0, Main.maxSectionsX, Main.maxSectionsY, sleepTime, cancellationToken);
	}

	public static async Task<int> RequestSections(int startX, int startY, int endX, int endY, int sleepTime = -1, CancellationToken? cancellationToken = null)
	{
		CancellationToken actualToken = cancellationToken ?? CancellationToken.None;
		int count = 0;
		RequestSectionPacket packet = default(RequestSectionPacket);
		for (int i = startX; i < endX; i++)
		{
			for (int j = startY; j < endY; j++)
			{
				if (actualToken.IsCancellationRequested)
				{
					break;
				}
				if (i >= 0 && i < Main.maxSectionsX && j >= 0 && j < Main.maxSectionsY && !Main.sectionManager.SectionLoaded(i, j))
				{
					packet.SectionX = (short)i;
					packet.SectionY = (short)j;
					packet.Send();
					count++;
					if (sleepTime > 0)
					{
						await Task.Delay(sleepTime, actualToken);
					}
				}
			}
		}
		return count;
	}

	public static int DeleteAllItems()
	{
		int deleted = 0;
		for (int i = 0; i < 400; i++)
		{
			WorldItem item = Main.item[i];
			if (item.active)
			{
				if (Main.netMode != 1)
				{
					item.TurnToAirAndSync();
				}
				deleted++;
			}
		}
		if (deleted > 0 && Main.netMode == 1)
		{
			Vector2 pos = new Point(Main.maxTilesX - 24, Main.maxTilesY - 24).ToWorldCoordinates();
			WorldItem item = Main.item[Item.NewItem(null, pos, 184, 1, 0, NewItemOwnership.GrabDelayForAllPlayers, new Vector2(0f, 2f))];
			item.shimmerTime = 69696970f;
			item.enemyGrabDelayTime = 69;
			SyncItemPacket itemPacket = SyncItemPacket.FromWorldItem(item, NewItemOwnership.GrabDelayForAllPlayers);
			FakePlayerControlsPacket(pos);
			for (int j = 0; j < 400; j++)
			{
				if (item.IsAir || !item.instanced || item.playerIndexTheItemIsReservedFor != Main.myPlayer)
				{
					itemPacket.Send();
				}
			}
			for (int k = 0; k < 400; k++)
			{
				item = Main.item[k];
				if (!item.IsAir && item.instanced && item.playerIndexTheItemIsReservedFor == Main.myPlayer)
				{
					SyncItemDespawnPacket packet = SyncItemDespawnPacket.FromWorldItem(item);
					packet.Send();
					packet.Handle((byte)Main.myPlayer);
				}
			}
		}
		if (deleted > 0)
		{
			SyncLocalPlayerControls();
		}
		return deleted;
	}

	public static int KillOwnedProjectiles()
	{
		int killed = 0;
		for (int i = 0; i < Main.projectile.Length; i++)
		{
			Projectile proj = Main.projectile[i];
			if (proj.active && proj.owner == Main.myPlayer)
			{
				proj.Kill();
				killed++;
			}
		}
		return killed;
	}
}
