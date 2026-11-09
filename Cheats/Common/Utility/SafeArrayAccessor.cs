using System;
using System.Collections;
using System.Collections.Generic;

namespace Terraria.UtilClient.Common.Utility;

public sealed class SafeArrayAccessor<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : notnull
{
	private readonly T[] _array;

	private readonly T _defaultValue;

	public int Length => _array.Length;

	public int Count => Length;

	public bool IsReadOnly => false;

	public T this[int index]
	{
		get
		{
			if ((uint)index >= (uint)_array.Length)
			{
				return _defaultValue;
			}
			return _array[index];
		}
		set
		{
			if ((uint)index < (uint)_array.Length)
			{
				_array[index] = value;
			}
		}
	}

	public SafeArrayAccessor(T[] array, T defaultValue)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentNullException.ThrowIfNull(defaultValue, "defaultValue");
		_array = array;
		_defaultValue = defaultValue;
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(_array, item);
	}

	public bool Contains(T item)
	{
		return Array.IndexOf(_array, item) >= 0;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_array.CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)_array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
