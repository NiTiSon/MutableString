using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NiTiS;

[DebuggerDisplay("{ToString()}")]
public partial class MutableString : IEnumerable<char>, IEnumerable, IEquatable<MutableString>
{
	private const int DefaultCapacity = 32;

	private char[] buffer;
	private int length;

	public int Capacity => buffer.Length;

	public int Length => length;

	public MutableString()
	{
		buffer = new char[DefaultCapacity];
		length = 0;
	}

	public MutableString(ReadOnlySpan<char> value)
	{
		buffer = new char[length = value.Length];
		value.CopyTo(buffer);
	}

	public MutableString(char[] value)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(value);
#else
		Guard.IsNotNull(value);
#endif

		buffer = new char[length = value.Length];

		value.CopyTo(buffer, length);
	}

	public MutableString(char[] value, int startIndex, int length)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(value);
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
#else
		Guard.IsNotNull(value);
		Guard.IsGreaterThanOrEqualTo(length, 0);
		Guard.IsGreaterThanOrEqualTo(startIndex, 0);
#endif

		if (startIndex + length > value.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
		}

		this.length = length;
		this.buffer = new char[length];
		value.CopyTo(this.buffer, startIndex);
	}

	public MutableString Duplicate()
	{
		return new MutableString(this.buffer, 0, this.length);
	}

	#region Standard methods
	public override bool Equals(object? obj)
	{
		return obj is MutableString other && this.Equals(other);
	}

	public bool Equals(MutableString? other)
	{
#if NET8_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(other);
#else
		Guard.IsNotNull(other);
#endif

		if (ReferenceEquals(this, other)) return true;
		if (this.length != other!.length) return false;

		char[] first = other.buffer;
		char[] second = other.buffer;

		for (int i = 0; i < this.length; i++)
		{
			if (first[i] != second[i]) return false;
		}

		return true;
	}

	public IEnumerator<char> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public override string ToString()
	{
		return new string(buffer, 0, length);
	}

	public override int GetHashCode()
	{
		return this.ToString().GetHashCode();
	}

#endregion
}
