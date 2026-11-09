using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Terraria.IO;
using Terraria.UtilClient.Config.JsonConverters;

namespace Terraria.UtilClient.Cheats.Settings;

public abstract class USettings
{
	public static readonly JsonSerializerSettings SerializerSettings;

	private const string KeyAdd = "Settings";

	public virtual void Save(Preferences prefs, string name)
	{
		string key = name + "Settings";
		prefs.UtilClientPut(key, this);
	}

	public virtual void Load(Preferences prefs, string name)
	{
		string key = name + "Settings";
		object obj = prefs.UtilClientGet<object>(key, null);
		JToken token = (JToken)((obj is JToken) ? obj : null);
		if (token != null)
		{
			JsonConvert.PopulateObject(((object)token).ToString(), (object)this, SerializerSettings);
		}
	}

	public static void Reset(USettings settings)
	{
		Type type = settings.GetType();
		object def = Activator.CreateInstance(type);
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo field in fields)
		{
			if (!field.IsLiteral || field.IsInitOnly)
			{
				field.SetValue(settings, field.GetValue(def));
			}
		}
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (PropertyInfo property in properties)
		{
			if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
			{
				property.SetValue(settings, property.GetValue(def));
			}
		}
	}

	static USettings()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		JsonSerializerSettings val = new JsonSerializerSettings();
		int num = 3;
		List<JsonConverter> list = new List<JsonConverter>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<JsonConverter> span = CollectionsMarshal.AsSpan(list);
		span[0] = (JsonConverter)new StringEnumConverter();
		span[1] = (JsonConverter)(object)new ColorConverter();
		span[2] = (JsonConverter)(object)new ColorAlphaConverter();
		val.Converters = list;
		val.NullValueHandling = (NullValueHandling)1;
		val.FloatFormatHandling = (FloatFormatHandling)0;
		val.FloatParseHandling = (FloatParseHandling)0;
		val.ObjectCreationHandling = (ObjectCreationHandling)2;
		SerializerSettings = val;
	}
}
