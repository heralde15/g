using System.IO;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class BinaryReaderHelper
{
	extension(BinaryReader reader)
	{
		public void ResetPosition()
		{
			reader.BaseStream.Position = 0L;
		}
	}
}
