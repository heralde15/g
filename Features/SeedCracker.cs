using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Testing;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;

namespace Terraria.UtilClient.Features;

public static class SeedCracker
{
	private struct LCGConfig
	{
		public long A = 0L;

		public List<long> Bs = new List<long>();

		public LCGConfig()
		{
		}
	}

	private const ulong GenVersion1449 = 1198295875585uL;

	private const ulong GenVersion1456 = 1370094567425uL;

	private const ulong GenVersion1458 = 1400159338497uL;

	private const int MSEED = 161803398;

	private static readonly int[] BASE_SEEDS = new int[4] { 0, 161844078, 259416290, 259416291 };

	private static int GetCallVal(int seed, int callIndex)
	{
		int[] SeedArray = new int[56];
		int subtraction = ((seed == int.MinValue) ? int.MaxValue : Math.Abs(seed));
		int mj = (SeedArray[55] = 161803398 - subtraction);
		int mk = 1;
		for (int j = 1; j < 55; j++)
		{
			int ii = 21 * j % 55;
			SeedArray[ii] = mk;
			mk = mj - mk;
			if (mk < 0)
			{
				mk += int.MaxValue;
			}
			mj = SeedArray[ii];
		}
		for (int k = 1; k < 5; k++)
		{
			for (int i = 1; i < 56; i++)
			{
				SeedArray[i] -= SeedArray[1 + (i + 30) % 55];
				if (SeedArray[i] < 0)
				{
					SeedArray[i] += int.MaxValue;
				}
			}
		}
		uint inext = 0u;
		int retVal = 0;
		for (int l = 0; l < callIndex; l++)
		{
			uint locINext = ((inext == 55) ? 1u : (inext + 1));
			ulong locINextp = locINext + 21;
			if (locINextp > 55)
			{
				locINextp -= 55;
			}
			retVal = SeedArray[locINext] - SeedArray[locINextp];
			if (retVal == int.MaxValue)
			{
				retVal--;
			}
			retVal = (SeedArray[locINext] = retVal + ((retVal >> 31) & 0x7FFFFFFF));
			inext = locINext;
		}
		return retVal;
	}

	private static long ModInverse(long n, long mod)
	{
		long res = 1L;
		long baseVal = n % mod;
		for (long exp = mod - 2; exp > 0; exp /= 2)
		{
			if (exp % 2 == 1)
			{
				res = res * baseVal % mod;
			}
			baseVal = baseVal * baseVal % mod;
		}
		return res;
	}

	private static LCGConfig Calibrate(int callIndex)
	{
		LCGConfig cfg = new LCGConfig();
		long val0 = GetCallVal(0, callIndex);
		long val1 = GetCallVal(1, callIndex);
		cfg.A = val1 - val0;
		if (cfg.A < 0)
		{
			cfg.A += 2147483647L;
		}
		int[] bASE_SEEDS = BASE_SEEDS;
		foreach (int baseVal in bASE_SEEDS)
		{
			long B = GetCallVal(baseVal, callIndex) - cfg.A * baseVal % int.MaxValue;
			if (B < 0)
			{
				B += int.MaxValue;
			}
			cfg.Bs.Add(B);
		}
		cfg.Bs.Sort();
		cfg.Bs = cfg.Bs.Distinct().ToList();
		return cfg;
	}

	public static void TryCrackSeed()
	{
		ulong generatorVersion = Main.ActiveWorldFileData.WorldGeneratorVersion;
		if (generatorVersion < 1198295875585L)
		{
			SeedNotCracked("World is too old, seed can't be cracked.");
		}
		else if (generatorVersion == 1198295875585L)
		{
			CrackSeed1449();
		}
		else if (generatorVersion > 1198295875585L && generatorVersion <= 1400159338497L)
		{
			if (!CrackSeed1458())
			{
				SeedNotCracked("Seed cracking failed, is there something weird with the world?");
			}
		}
		else
		{
			SeedNotCracked("World is too new, seed can't be cracked.");
		}
	}

	private static bool CrackSeed1449()
	{
		SeedCracked((int)((316080314L * (long)Main.worldID + 75192552) % int.MaxValue));
		return true;
	}

	private static bool CrackSeed1458()
	{
		long worldID = Main.worldID;
		LCGConfig lCGConfig = Calibrate(18);
		long invA = ModInverse(lCGConfig.A, 2147483647L);
		foreach (long B in lCGConfig.Bs)
		{
			long diff = worldID - B;
			if (diff < 0)
			{
				diff += int.MaxValue;
			}
			int candidateSeed = (int)(diff * invA % int.MaxValue);
			if (GetCallVal(candidateSeed, 18) == worldID)
			{
				SeedCracked(candidateSeed);
				return true;
			}
		}
		return false;
	}

	private static void SeedCracked(int candidateSeed)
	{
		ManagerExtensions.get_Instance<LoggingManager>().zMod.Info((object)$"Seed of {Main.worldName} cracked: {candidateSeed}");
		if (Main.netMode == 1)
		{
			Main.ActiveWorldFileData.SetSeed(candidateSeed);
			return;
		}
		Invariant.Assert(candidateSeed == Main.ActiveWorldFileData.Seed, $"Candidate seed {candidateSeed} is does not match real seed {Main.ActiveWorldFileData.Seed}");
	}

	private static void SeedNotCracked(string errorMessage)
	{
		ManagerExtensions.get_Instance<LoggingManager>().zMod.Warn((object)errorMessage);
		Main.ActiveWorldFileData.SetSeedText(null);
	}
}
