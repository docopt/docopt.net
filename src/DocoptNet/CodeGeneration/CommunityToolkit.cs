#region Copyright .NET Foundation and Contributors. All rights reserved.
/*
 **************************************************************
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************
 */
#endregion

// Source: https://github.com/CommunityToolkit/dotnet/tree/v8.1.0/src/CommunityToolkit.Mvvm.SourceGenerators

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    partial class Extensions
    {
        /// <summary>
        /// Checks whether a given <see cref="TypeDeclarationSyntax"/> has or could possibly have any attributes, using only syntax.
        /// </summary>
        /// <param name="typeDeclaration">The input <see cref="TypeDeclarationSyntax"/> instance to check.</param>
        /// <returns>Whether <paramref name="typeDeclaration"/> has or could possibly have any attributes.</returns>

        public static bool HasOrPotentiallyHasAttributes(this TypeDeclarationSyntax typeDeclaration) =>
            //
            // If the type has any attributes lists, then clearly it can have attributes
            //
            typeDeclaration.AttributeLists.Count > 0
            //
            // If the declaration has no attribute lists, check if the type is partial. If it is, it means
            // that there could be another partial declaration with some attribute lists over them.
            //
            || typeDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    }

    /// <summary>
    /// A model for a serializeable diagnostic info.
    /// </summary>
    /// <param name="Descriptor">The wrapped <see cref="DiagnosticDescriptor"/> instance.</param>
    /// <param name="SyntaxTree">The tree to use as location for the diagnostic, if available.</param>
    /// <param name="TextSpan">The span to use as location for the diagnostic.</param>
    /// <param name="Arguments">The diagnostic arguments.</param>

    sealed record DiagnosticInfo(
        DiagnosticDescriptor Descriptor,
        SyntaxTree? SyntaxTree,
        TextSpan TextSpan,
        EquatableArray<string> Arguments)
    {
        /// <summary>
        /// Creates a new <see cref="Diagnostic"/> instance with the state from this model.
        /// </summary>
        /// <returns>A new <see cref="Diagnostic"/> instance with the state from this model.</returns>

        public Diagnostic ToDiagnostic()
        {
            return Diagnostic.Create(Descriptor, SyntaxTree is { } tree ? Location.Create(tree, TextSpan) : null,
                                     Arguments.ToArray());
        }

        /// <summary>
        /// Creates a new <see cref="DiagnosticInfo"/> instance with the specified parameters.
        /// </summary>
        /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
        /// <param name="symbol">The source <see cref="ISymbol"/> to attach the diagnostics to.</param>
        /// <param name="args">The optional arguments for the formatted message to include.</param>
        /// <returns>A new <see cref="DiagnosticInfo"/> instance with the specified parameters.</returns>

        public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, ISymbol symbol, params object[] args)
        {
            var location = symbol.Locations.First();
            return new(descriptor, location.SourceTree, location.SourceSpan, args.Select(static arg => arg.ToString()).ToImmutableArray());
        }

        /// <summary>
        /// Creates a new <see cref="DiagnosticInfo"/> instance with the specified parameters.
        /// </summary>
        /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
        /// <param name="node">The source <see cref="SyntaxNode"/> to attach the diagnostics to.</param>
        /// <param name="args">The optional arguments for the formatted message to include.</param>
        /// <returns>A new <see cref="DiagnosticInfo"/> instance with the specified parameters.</returns>

        public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, SyntaxNode node, params object[] args)
        {
            var location = node.GetLocation();
            return new(descriptor, location.SourceTree, location.SourceSpan, args.Select(static arg => arg.ToString()).ToImmutableArray());
        }
    }

    /// <summary>
    /// A model representing a value and an associated set of diagnostic errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the wrapped value.</typeparam>
    /// <param name="Value">The wrapped value for the current result.</param>
    /// <param name="Errors">The associated diagnostic errors, if any.</param>

    sealed record Result<TValue>(TValue Value, EquatableArray<DiagnosticInfo> Errors)
        where TValue : IEquatable<TValue>?;

    /// <summary>
    /// Extensions for <see cref="EquatableArray{T}"/>.
    /// </summary>

    static class EquatableArray
    {
        /// <summary>
        /// Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the input array.</typeparam>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>

        public static EquatableArray<T> AsEquatableArray<T>(this ImmutableArray<T> array)
            where T : IEquatable<T>
        {
            return new(array);
        }
    }

    /// <summary>
    /// An immutable, equatable array. This is equivalent to <see cref="ImmutableArray{T}"/> but with value equality support.
    /// </summary>
    /// <typeparam name="T">The type of values in the array.</typeparam>
    readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The underlying <typeparamref name="T"/> array.
        /// </summary>

        readonly T[]? array;

        /// <summary>
        /// Creates a new <see cref="EquatableArray{T}"/> instance.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> to wrap.</param>

        public EquatableArray(ImmutableArray<T> array)
        {
            this.array = Unsafe.As<ImmutableArray<T>, T[]?>(ref array);
        }

        /// <summary>
        /// Gets a reference to an item at a specified position within the array.
        /// </summary>
        /// <param name="index">The index of the item to retrieve a reference to.</param>
        /// <returns>A reference to an item at a specified position within the array.</returns>

        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsImmutableArray().ItemRef(index);
        }

        /// <summary>
        /// Gets a value indicating whether the current array is empty.
        /// </summary>

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsImmutableArray().IsEmpty;
        }

        /// <sinheritdoc/>

        public bool Equals(EquatableArray<T> array)
        {
            return AsSpan().SequenceEqual(array.AsSpan());
        }

        /// <sinheritdoc/>

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is EquatableArray<T> array && Equals(this, array);
        }

        /// <sinheritdoc/>

        public override int GetHashCode()
        {
            if (this.array is not T[] array)
            {
                return 0;
            }

            HashCode hashCode = default;

            foreach (var item in array)
            {
                hashCode.Add(item);
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}"/> instance from the current <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>The <see cref="ImmutableArray{T}"/> from the current <see cref="EquatableArray{T}"/>.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<T> AsImmutableArray()
        {
            return Unsafe.As<T[]?, ImmutableArray<T>>(ref Unsafe.AsRef(in this.array));
        }

        /// <summary>
        /// Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>

        public static EquatableArray<T> FromImmutableArray(ImmutableArray<T> array)
        {
            return new(array);
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
        /// </summary>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>

        public ReadOnlySpan<T> AsSpan()
        {
            return AsImmutableArray().AsSpan();
        }

        /// <summary>
        /// Copies the contents of this <see cref="EquatableArray{T}"/> instance. to a mutable array.
        /// </summary>
        /// <returns>The newly instantiated array.</returns>

        public T[] ToArray()
        {
            return AsImmutableArray().ToArray();
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.</returns>

        public ImmutableArray<T>.Enumerator GetEnumerator()
        {
            return AsImmutableArray().GetEnumerator();
        }

        /// <sinheritdoc/>

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();
        }

        /// <sinheritdoc/>

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)AsImmutableArray()).GetEnumerator();
        }

        /// <summary>
        /// Implicitly converts an <see cref="ImmutableArray{T}"/> to <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>

        public static implicit operator EquatableArray<T>(ImmutableArray<T> array)
        {
            return FromImmutableArray(array);
        }

        /// <summary>
        /// Implicitly converts an <see cref="EquatableArray{T}"/> to <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}"/> instance from a given <see cref="EquatableArray{T}"/>.</returns>

        public static implicit operator ImmutableArray<T>(EquatableArray<T> array)
        {
            return array.AsImmutableArray();
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>

        public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>

        public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// A polyfill type that mirrors some methods from <see cref="HashCode"/> on .NET 6.
    /// </summary>
    struct HashCode
    {
        const uint Prime1 = 2654435761U;
        const uint Prime2 = 2246822519U;
        const uint Prime3 = 3266489917U;
        const uint Prime4 = 668265263U;
        const uint Prime5 = 374761393U;

        static readonly uint seed = GenerateGlobalSeed();

        uint v1, v2, v3, v4;
        uint queue1, queue2, queue3;
        uint length;

        /// <summary>
        /// Initializes the default seed.
        /// </summary>
        /// <returns>A random seed.</returns>

        static uint GenerateGlobalSeed()
        {
            var bytes = new byte[4];

            RandomNumberGenerator.Create().GetBytes(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Adds a single value to the current hash.
        /// </summary>
        /// <typeparam name="T">The type of the value to add into the hash code.</typeparam>
        /// <param name="value">The value to add into the hash code.</param>

        public void Add<T>(T value)
        {
            Add(value?.GetHashCode() ?? 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
        {
            v1 = seed + Prime1 + Prime2;
            v2 = seed + Prime2;
            v3 = seed;
            v4 = seed - Prime1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Round(uint hash, uint input)
        {
            return RotateLeft(hash + input * Prime2, 13) * Prime1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint QueueRound(uint hash, uint queuedValue)
        {
            return RotateLeft(hash + queuedValue * Prime3, 17) * Prime4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint MixState(uint v1, uint v2, uint v3, uint v4)
        {
            return RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint MixEmptyState()
        {
            return seed + Prime5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint MixFinal(uint hash)
        {
            hash ^= hash >> 15;
            hash *= Prime2;
            hash ^= hash >> 13;
            hash *= Prime3;
            hash ^= hash >> 16;

            return hash;
        }

        void Add(int value)
        {
            var val = (uint)value;
            var previousLength = length++;
            var position = previousLength % 4;

            switch (position)
            {
                case 0: queue1 = val; break;
                case 1: queue2 = val; break;
                case 2: queue3 = val; break;
                default:
                {
                    if (previousLength == 3)
                        Initialize(out v1, out v2, out v3, out v4);

                    v1 = Round(v1, queue1);
                    v2 = Round(v2, queue2);
                    v3 = Round(v3, queue3);
                    v4 = Round(v4, val);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the resulting hashcode from the current instance.
        /// </summary>
        /// <returns>The resulting hashcode from the current instance.</returns>

        public int ToHashCode()
        {
            var length = this.length;
            var position = length % 4;
            var hash = length < 4 ? MixEmptyState() : MixState(v1, v2, v3, v4);

            hash += length * 4;

            if (position > 0)
            {
                hash = QueueRound(hash, queue1);

                if (position > 1)
                {
                    hash = QueueRound(hash, queue2);

                    if (position > 2)
                    {
                        hash = QueueRound(hash, queue3);
                    }
                }
            }

            hash = MixFinal(hash);

            return (int)hash;
        }

        /// <inheritdoc/>

        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => throw new NotSupportedException();

        /// <inheritdoc/>

        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => throw new NotSupportedException();

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
        /// <returns>The rotated value.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint RotateLeft(uint value, int offset)
        {
            return (value << offset) | (value >> (32 - offset));
        }
    }
}
