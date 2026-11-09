using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

namespace Terraria.UtilClient.Common.Utility;

public sealed class ServerEndPoint : DnsEndPoint, IEquatable<ServerEndPoint>, IParsable<ServerEndPoint>
{
	public ServerEndPoint(string host, int port)
		: base(host, port)
	{
	}

	public ServerEndPoint(IPAddress ipAddress, int port)
		: this(ipAddress.ToString(), port)
	{
	}

	private static bool TryParse(ReadOnlySpan<char> input, [NotNullWhen(true)] out ServerEndPoint? result)
	{
		result = null;
		int div = input.LastIndexOf(':');
		if (div <= 0)
		{
			return false;
		}
		ReadOnlySpan<char> host = input.Slice(0, div);
		if (!ushort.TryParse(input.Slice(div + 1), NumberStyles.None, CultureInfo.InvariantCulture, out var port))
		{
			return false;
		}
		result = new ServerEndPoint(host.ToString(), port);
		return true;
	}

	public static bool TryParse(string input, [NotNullWhen(true)] out ServerEndPoint? result)
	{
		ArgumentException.ThrowIfNullOrEmpty(input, "input");
		return TryParse(input.AsSpan(), out result);
	}

	public static ServerEndPoint Parse(string s, IFormatProvider? provider)
	{
		ThrowIfProvided(provider);
		if (TryParse(s, out ServerEndPoint result))
		{
			return result;
		}
		throw new FormatException("Invalid server endpoint: '" + s + "'.");
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ServerEndPoint result)
	{
		ThrowIfProvided(provider);
		if (!string.IsNullOrEmpty(s))
		{
			return TryParse(s, out result);
		}
		result = null;
		return false;
	}

	public bool Equals(ServerEndPoint? other)
	{
		if (other == null)
		{
			return false;
		}
		if (string.Equals(base.Host, other.Host, StringComparison.OrdinalIgnoreCase))
		{
			return base.Port == other.Port;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.Host}:{base.Port}";
	}

	private static void ThrowIfProvided(IFormatProvider? provider)
	{
		if (provider == null)
		{
			return;
		}
		throw new NotSupportedException("Format providers are not supported.");
	}
}
