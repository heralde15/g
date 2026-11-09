using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Terraria.UtilClient.Common.Utility;

public abstract class ClassTypeRegistry<T> where T : class
{
	private readonly FrozenDictionary<Type, T> _items;

	public IReadOnlyDictionary<Type, T> Items => _items;

	protected virtual Assembly Assembly => typeof(T).Assembly;

	protected virtual string RootNamespaceRestriction => "";

	protected ClassTypeRegistry()
	{
		_items = Collect();
	}

	public TItem Get<TItem>() where TItem : class, T
	{
		return (TItem)_items[typeof(TItem)];
	}

	public T Get(Type type)
	{
		return _items[type];
	}

	protected virtual bool IsValidType(Type type)
	{
		return true;
	}

	protected abstract T Create(Type type);

	private bool IsValidTypeInternal(Type type)
	{
		if (type != typeof(T) && (object)type != null && type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(T)) && IsValidNamespace(type))
		{
			return IsValidType(type);
		}
		return false;
	}

	private bool IsValidNamespace(Type type)
	{
		if (string.IsNullOrWhiteSpace(RootNamespaceRestriction))
		{
			return true;
		}
		string @namespace = type.Namespace;
		if (@namespace == null || !@namespace.StartsWith(RootNamespaceRestriction, StringComparison.Ordinal))
		{
			return false;
		}
		int namespaceLength = @namespace.Length;
		int rootNamespaceLength = RootNamespaceRestriction.Length;
		if (namespaceLength != rootNamespaceLength)
		{
			if (namespaceLength > rootNamespaceLength)
			{
				return @namespace[rootNamespaceLength] == '.';
			}
			return false;
		}
		return true;
	}

	private FrozenDictionary<Type, T> Collect()
	{
		return Assembly.GetTypes().Where(IsValidTypeInternal).ToDictionary((Type type) => type, Create)
			.ToFrozenDictionary();
	}
}
