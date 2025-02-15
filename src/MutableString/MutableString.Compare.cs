using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace NiTiS;

public partial class MutableString : IComparable<MutableString>, IComparable<string>, IComparable
{
	/// <summary>
	/// Compares this <see cref="MutableString"/> to another <see cref="String"/> or <see cref="MutableString"/> (cast as object), returning an integer that
	/// indicates the relationship. This method returns a value less than 0 if this is less than value, 0
	/// if this is equal to value, or a value greater than 0 if this is greater than value.
	/// </summary>
	/// <param name="value">An object that evaluates to a <see cref="String"/> or <see cref="MutableString"/>.</param>
	/// <returns>
	/// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="value"/> parameter.
	/// <list type="table">
	/// <listheader><term> Value</term><description> Meaning</description></listheader>
	///		<item><term> Less than zero</term><description> This instance precedes <paramref name="value" /> in the sort order.</description></item>
	///		<item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="value" />.</description></item>
	///		<item><term> Greater than zero</term><description> This instance follows <paramref name="value" /> in the sort order. -or- the <paramref name="value"/> is <see langword="null"/>.</description></item>
	/// </list>
	/// </returns>
	public int CompareTo(object? value)
	{
		if (value is null) return 1;

		if (value is string str)
		{
			return CompareTo(str);
		}

		if (value is MutableString mutableString)
		{
			return CompareTo(mutableString);
		}

		ThrowHelper.ThrowArgumentException(nameof(value), "Argument must be string type.");
		return 0;
	}

	/// <summary>
	/// Compares this <see cref="MutableString"/> to another <see cref="MutableString"/>, returning an integer that
	/// indicates the relationship. This method returns a value less than 0 if this is less than value, 0
	/// if this is equal to value, or a value greater than 0 if this is greater than value.
	/// </summary>
	/// <param name="value">The string to compare with this instance.</param>
	/// <returns>
	/// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="value"/> parameter.
	/// <list type="table">
	/// <listheader><term> Value</term><description> Meaning</description></listheader>
	///		<item><term> Less than zero</term><description> This instance precedes <paramref name="value" /> in the sort order.</description></item>
	///		<item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="value" />.</description></item>
	///		<item><term> Greater than zero</term><description> This instance follows <paramref name="value" /> in the sort order. -or- the <paramref name="value"/> is <see langword="null"/>.</description></item>
	/// </list>
	/// </returns>
	public int CompareTo(MutableString? value)
	{
		if (value is null) return 1;

		return Compare(this.AsSpan(), value.AsSpan(), StringComparison.CurrentCulture);
	}

	/// <summary>
	/// Compares this <see cref="MutableString"/> to another <see cref="string"/>, returning an integer that
	/// indicates the relationship. This method returns a value less than 0 if this is less than value, 0
	/// if this is equal to value, or a value greater than 0 if this is greater than value.
	/// </summary>
	/// <param name="value">The string to compare with this instance.</param>
	/// <returns>
	/// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="value"/> parameter.
	/// <list type="table">
	/// <listheader><term> Value</term><description> Meaning</description></listheader>
	///		<item><term> Less than zero</term><description> This instance precedes <paramref name="value" /> in the sort order.</description></item>
	///		<item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="value" />.</description></item>
	///		<item><term> Greater than zero</term><description> This instance follows <paramref name="value" /> in the sort order. -or- the <paramref name="value"/> is <see langword="null"/>.</description></item>
	/// </list>
	/// </returns>
	public int CompareTo(string? value)
	{
		if (value is null) return 1;

		return Compare(this.AsSpan(), value.AsSpan(), StringComparison.Ordinal);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Compare(ReadOnlySpan<char> value1, ReadOnlySpan<char> value2, StringComparison comparisonType)
	{
		return value1.CompareTo(value2, comparisonType);
	}
}