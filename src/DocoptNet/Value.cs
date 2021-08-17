#nullable enable

namespace DocoptNet
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    enum ValueKind { None, Boolean, Integer, String, StringList }

    [DebuggerDisplay("{" + nameof(DebugDisplay) + "(),nq}")]
    readonly struct Value
    {
        readonly int _int;     // stores: bool, int
        readonly object? _ref; // stores: string, StringList

        public static readonly Value None  = new(ValueKind.None, null);
        public static readonly Value True  = new(ValueKind.Boolean, 1);
        public static readonly Value False = new(ValueKind.Boolean, 0);

        Value(ValueKind kind, object? value) : this(kind, 0, value) { }
        Value(ValueKind kind, int @int, object? @ref = null) => (Kind, _int, _ref) = (kind, @int, @ref);

        public ValueKind Kind { get; }

        public bool IsNone       => Kind == ValueKind.None;
        public bool IsBoolean    => Kind == ValueKind.Boolean;
        public bool IsInteger    => Kind == ValueKind.Integer;
        public bool IsString     => Kind == ValueKind.String;
        public bool IsStringList => Kind == ValueKind.StringList;

        public bool IsTrue  => IsBoolean && !IsFalse;
        public bool IsFalse => IsBoolean && _int == 0;

        public object? Box =>
            Kind switch
            {
                ValueKind.None       => null,
                ValueKind.Boolean    => (bool)this ? Boxed.True : Boxed.False,
                ValueKind.Integer    => Boxed.Integer((int)this),
                ValueKind.String     => (string)this,
                ValueKind.StringList => (StringList)this,
                _                    => throw new InvalidOperationException()
            };

        string DebugDisplay() => $"{Kind}: {this}";

        public override string ToString() => ValueObject.Format(Box);

        public static implicit operator Value(bool value) => value ? True : False;
        public static implicit operator Value(int value) => new(ValueKind.Integer, value);
        public static implicit operator Value(string value) => new(ValueKind.String, value);
        public static implicit operator Value(StringList value) => new(ValueKind.StringList, value);

        public bool TryAsBoolean(out bool value) { value = IsBoolean && _int != 0; return IsBoolean; }
        public bool TryAsInteger(out int value) { value = IsInteger ? _int : default; return IsInteger; }
        public bool TryAsString([NotNullWhen(true)]out string? value) { value = _ref is string s ? s : default; return IsString; }
        public bool TryAsStringList([NotNullWhen(true)]out StringList? value) { value = _ref is StringList list ? list : default; return IsStringList; }

        public static explicit operator bool(Value value) => value.TryAsBoolean(out var f) ? f : throw new InvalidCastException();
        public static explicit operator int(Value value) => value.TryAsInteger(out var n) ? n : throw new InvalidCastException();
        public static explicit operator string(Value value) => value.TryAsString(out var s) ? s : throw new InvalidCastException();
        public static explicit operator StringList(Value value) => value.TryAsStringList(out var list) ? list : throw new InvalidCastException();

        static class Boxed
        {
            public static readonly object True = true;
            public static readonly object False = false;

            static readonly object[] Integers;

            public static object Integer(int n) => n >= 0 && n < Integers.Length ? Integers[n] : n;

            static Boxed()
            {
                var ints = new object[5];
                for (var i = 0; i < ints.Length; i++)
                    ints[i] = i;
                Integers = ints;
            }
        }

        internal ValueObject? ToValueObject() => IsNone ? null : new ValueObject(Box);
    }
}
