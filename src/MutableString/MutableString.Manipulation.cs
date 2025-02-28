using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NiTiS;

public partial class MutableString
{
	/// <summary>
	/// Fill whole string content with <paramref name="filler"/>.
	/// </summary>
	/// <param name="filler">Character to fill whole string.</param>
	/// <returns>A reference to this instance after the fill operation is completed.</returns>
	public MutableString Fill(char filler)
	{
		AsSpan().Fill(filler);
		return this;
	}

	/// <summary>
	/// Fill specified string region by specified filler.
	/// </summary>
	/// <param name="range">Range to fill.</param>
	/// <param name="filler">Character to fill within the range.</param>
	/// <returns>A reference to this instance after the fill operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="range"/> present invalid span of string.</exception>
	public MutableString Fill(Range range, char filler)
	{
		int start = range.Start.GetOffset(this.length);
		int length = range.End.GetOffset(this.length) - start;

		if (length < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(nameof(range));
		}

		AsSpan().Slice(start, length).Fill(filler);
		return this;
	}

	/// <summary>
	/// Appends the string representation of a specified <see cref="char"/> object to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value">Character to append to this string.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(char value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Appends the string representation of a specified read-only character span to this <see cref="MutableString"/> instance.
	/// </summary>
	/// <param name="value">String span to append to this string.</param>
	/// <returns>A reference to this instance after the append operation is completed.</returns>
	public MutableString Append(ReadOnlySpan<char> value)
	{
		return Insert(length, value);
	}

	/// <summary>
	/// Remove specified region in <see cref="MutableString"/>.
	/// </summary>
	/// <param name="index">Index to remove characters.</param>
	/// <param name="length">Length of removed characters.</param>
	/// <returns>A reference to this instance after the remove operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="index"/> is negative.
	/// -or-
	/// <paramref name="length"/> is negative or zero.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// <paramref name="length"/> is greater than the number of elements from <paramref name="index"/> to the end of this string.
	/// </exception>
	public MutableString Remove(int index, int length)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
#else
		Guard.IsGreaterThanOrEqualTo(index, 0);
		Guard.IsGreaterThan(length, 0);
#endif
		int lastRemoveIndex = index + length;

		if (lastRemoveIndex > this.length)
		{
			ThrowHelper.ThrowArgumentException(nameof(length), "length and index do not present valid region within string.");
		}

		if (lastRemoveIndex != this.length) // Remove region is in between or at begin
		{
			Array.Copy(this.buffer, lastRemoveIndex, this.buffer, index, this.length - lastRemoveIndex);
		}

		this.length -= length;

		return this;
	}

	/// <summary>
	/// Replace all occurrences of a specified Unicode character in the instance to another specified Unicode character.
	/// </summary>
	/// <param name="oldChar">The Unicode character to be replaced.</param>
	/// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChar"/>.</param>
	/// <returns>A reference to this instance after the replace operation is completed.</returns>
	public MutableString Replace(char oldChar, char newChar)
	{
		if (oldChar == newChar) return this;

		ref char buffer = ref MemoryMarshal.GetArrayDataReference(this.buffer);

		for (int i = 0; i < length; i++)
		{
			if (Unsafe.Add(ref buffer, i) == oldChar)
			{
				Unsafe.Add(ref buffer, i) = newChar;
			}
		}

		return this;
	}

	/// <summary>
	/// Replace all occurrences of a specified Unicode character sequence in the instance to another specified Unicode character sequence.
	/// </summary>
	/// <param name="oldValue">The string span to be replaced.</param>
	/// <param name="newValue">The string span to replace all occurrences of <paramref name="oldValue"/>.</param>
	/// <returns>A reference to this instance after the replace operation is completed.</returns>
	public MutableString Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
	{
		if (oldValue.SequenceEqual(newValue)) return this;
		if (oldValue.IsEmpty) return this;

		int index = 0;
		while (index < this.length)
		{
			index = IndexOf(oldValue, index);

			if (index < 0) break;


			if (oldValue.Length > newValue.Length)
			{
				int diff = oldValue.Length - newValue.Length;
				Array.Copy(this.buffer, index + diff, this.buffer, index, this.length - index);
				this.length -= diff;
			}
			else if (oldValue.Length < newValue.Length)
			{
				int diff = newValue.Length - oldValue.Length;
				if (this.buffer.Length < this.length + diff)
				{
					GrowForInsertion(index + oldValue.Length, diff); // Extending buffer
				}
				else
				{
					Array.Copy(this.buffer, index + oldValue.Length, this.buffer, index + newValue.Length, this.Length - index - oldValue.Length);
				}
				this.length += diff;
			}

			newValue.CopyTo(this.buffer.AsSpan()[index..]);
			index += newValue.Length;
		}

		return this;
	}

	/// <summary>
	/// Insert character in <see cref="MutableString"/> at specified <paramref name="index"/>.
	/// If required, the string capacity may be increased.
	/// </summary>
	/// <param name="index">Index to insert character.</param>
	/// <param name="value">Character to insert.</param>
	/// <returns>A reference to this instance after the insert operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> are out of string bounds.</exception>
	public MutableString Insert(int index, char value)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)length);
#else
		Guard.IsLessThanOrEqualTo((uint)index, (uint)length);
#endif

		if (length == Capacity)
		{
			GrowForInsertion(index, 1);
		}
		else if (index < length)
		{
			Array.Copy(buffer, index, buffer, index + 1, length - index);
		}

		buffer[index] = value;
		length++;

		return this;
	}

	/// <summary>
	/// Insert string in <see cref="MutableString"/> at <paramref name="index"/>.
	/// If required, the string capacity may be increased.
	/// </summary>
	/// <param name="index">Index to insert string.</param>
	/// <param name="value">String to insert.</param>
	/// <returns>A reference to this instance after the insert operation is completed.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> are out of string bounds.</exception>
	public MutableString Insert(int index, ReadOnlySpan<char> value)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)length);
#else
		Guard.IsLessThanOrEqualTo((uint)index, (uint)length);
#endif

		int count = value.Length;

		if (count == 0) return this;

		if (Capacity - length < count)
		{
			GrowForInsertion(index, count);
		}
		else if (index < length)
		{
			Array.Copy(buffer, index, buffer, index + count, length - index);
		}

		value.CopyTo(buffer.AsSpan()[index..]);

		length += count;

		return this;
	}

	/// <summary>
	/// Reverse character order of this string.
	/// </summary>
	/// <returns>A reference to this instance after the reverse operation is completed.</returns>
	public MutableString Reverse()
	{
		Array.Reverse(buffer, 0, length);

		return this;
	}

	/// <summary>
	/// Set string length to 0 (zero).
	/// </summary>
	/// <returns>A reference to this instance after the clear operation is completed.</returns>
	public MutableString Clear()
	{
		length = 0;
		return this;
	}
}