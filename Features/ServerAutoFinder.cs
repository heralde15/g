using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Terraria.UtilClient.Config.Types.Structures;
using Terraria.UtilClient.UI.States;

namespace Terraria.UtilClient.Features;

public static class ServerAutoFinder
{
	public struct ServerSearchResult(string worldN, IPAddress serverIp, int port, ushort tilesX, int numPlayers, int maxPlayers, short difficulty, bool corrupt, bool hard, string host)
	{
		public string WorldName = worldN;

		public IPAddress ServerAddress = serverIp;

		public int ServerPort = port;

		public DateTime TimeSinceLastRecieved = DateTime.UtcNow;

		public ushort WorldXSize = tilesX;

		public int NumberOfPlayers = numPlayers;

		public int MaxPlayers = maxPlayers;

		public short GameMode = difficulty;

		public bool Corruption = corrupt;

		public bool HardMode = hard;

		public string HostName = host;

		public string FormatName => "(" + HostName + ") " + WorldName;
	}

	public static UdpClient broadcastPort;

	public static Thread broadcastThread;

	public static List<ServerSearchResult> TempResults = new List<ServerSearchResult>();

	public static Stopwatch TimeSinceLastListUpdate = new Stopwatch();

	public static List<ServerSearchResult> Results = new List<ServerSearchResult>();

	public static bool abortBroadcastThread;

	public static List<SavedServerData> ResultsToData
	{
		get
		{
			List<SavedServerData> data = new List<SavedServerData>();
			for (int i = 0; i < Results.Count; i++)
			{
				data.Add(new SavedServerData(Results[i]));
			}
			return data;
		}
	}

	public static void BroadcastRecieveThread()
	{
		abortBroadcastThread = false;
		if (TempResults.Count > 0)
		{
			TempResults.Clear();
		}
		while (true)
		{
			try
			{
				if (abortBroadcastThread)
				{
					abortBroadcastThread = false;
					break;
				}
				if (!(Main.MenuUI.CurrentState is UIServerList))
				{
					break;
				}
				while (broadcastPort == null)
				{
					broadcastPort = new UdpClient(8888);
				}
				UdpReceiveResult result = broadcastPort.ReceiveAsync().Result;
				BinaryReader reader = new BinaryReader(new MemoryStream(result.Buffer));
				if (reader.ReadInt32() == 1010)
				{
					IPAddress address = result.RemoteEndPoint.Address;
					int port = reader.ReadInt32();
					string worldName = reader.ReadString();
					string hostName = reader.ReadString();
					ushort tilesX = reader.ReadUInt16();
					bool crimson = reader.ReadBoolean();
					short gameMode = (short)reader.ReadInt32();
					byte maxPlayers = reader.ReadByte();
					byte numPlayers = reader.ReadByte();
					bool hardmode = reader.ReadBoolean();
					lock (TempResults)
					{
						bool shouldAdd = true;
						ServerSearchResult searchResult = new ServerSearchResult(worldName, address, port, tilesX, numPlayers, maxPlayers, gameMode, !crimson, hardmode, hostName);
						for (int i = 0; i < TempResults.Count; i++)
						{
							ServerSearchResult res = TempResults[i];
							if (res.ServerAddress.Equals(address))
							{
								res.TimeSinceLastRecieved = DateTime.UtcNow;
								TempResults[i] = res;
								shouldAdd = false;
								break;
							}
						}
						if (shouldAdd)
						{
							TempResults.Add(searchResult);
						}
					}
				}
			}
			catch
			{
			}
			Thread.Sleep(1000);
		}
	}

	public static void StartSearching()
	{
		if (broadcastThread != null)
		{
			StopSearching();
		}
		broadcastThread = new Thread(BroadcastRecieveThread);
		broadcastThread.Name = "LAN Game Finder Thread";
		broadcastThread.IsBackground = true;
		broadcastThread.Start();
	}

	public static void StopSearching()
	{
		if (broadcastThread != null)
		{
			abortBroadcastThread = true;
		}
		if (broadcastPort != null)
		{
			broadcastPort.Close();
			broadcastPort = null;
		}
		TempResults.Clear();
		Results.Clear();
		TimeSinceLastListUpdate.Stop();
	}

	public static void UpdateResults()
	{
		if (!TimeSinceLastListUpdate.IsRunning)
		{
			TimeSinceLastListUpdate.Restart();
		}
		if (!(TimeSinceLastListUpdate.Elapsed.TotalSeconds > 0.25))
		{
			return;
		}
		lock (TempResults)
		{
			Results.Clear();
			for (int i = TempResults.Count - 1; i >= 0; i--)
			{
				ServerSearchResult result = TempResults[i];
				if ((DateTime.UtcNow - result.TimeSinceLastRecieved).TotalSeconds <= 10.0)
				{
					Results.Add(result);
				}
				else
				{
					TempResults.RemoveAt(i);
				}
			}
		}
		TimeSinceLastListUpdate.Restart();
	}
}
