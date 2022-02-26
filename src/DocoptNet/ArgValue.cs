#nullable enable

namespace DocoptNet
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    #if DOCOPTNET_PUBLIC
    public // ...
    #endif
    /* ... */ enum ArgValueKind { None, Boolean, Integer, String, StringList }

    [DebuggerDisplay("{" + nameof(DebugDisplay) + "(),nq}")]
    readonly partial struct ArgValue
    {
        readonly int _int;     // stores: bool, int
        readonly object? _ref; // stores: string, StringList

        public static readonly ArgValue None  = new(ArgValueKind.None, null);
        public static readonly ArgValue True  = new(ArgValueKind.Boolean, 1);
        public static readonly ArgValue False = new(ArgValueKind.Boolean, 0);

        ArgValue(ArgValueKind kind, object? value) : this(kind, 0, value) { }
        ArgValue(ArgValueKind kind, int @int, object? @ref = null) => (Kind, _int, _ref) = (kind, @int, @ref);

        public ArgValueKind Kind { get; }

        public bool IsNone       => Kind == ArgValueKind.None;
        public bool IsBoolean    => Kind == ArgValueKind.Boolean;
        public bool IsInteger    => Kind == ArgValueKind.Integer;
        public bool IsString     => Kind == ArgValueKind.String;
        public bool IsStringList => Kind == ArgValueKind.StringList;

        public bool IsTrue  => IsBoolean && !IsFalse;
        public bool IsFalse => IsBoolean && _int == 0;

        public object? Object =>
            Kind switch
            {
                ArgValueKind.None       => null,
                ArgValueKind.Boolean    => (bool)this ? Boxed.True : Boxed.False,
                ArgValueKind.Integer    => Boxed.Integer((int)this),
                ArgValueKind.String     => (string)this,
                ArgValueKind.StringList => (StringList)this,
                _                       => throw new InvalidOperationException()
            };

        string DebugDisplay() => $"{Kind}: {this}";

        public override string ToString() => ValueObject.Format(Object);

        public static implicit operator ArgValue(bool value) => value ? True : False;
        public static implicit operator ArgValue(int value) => new(ArgValueKind.Integer, value);
        public static implicit operator ArgValue(string value) => new(ArgValueKind.String, value);
        public static implicit operator ArgValue(StringList value) => new(ArgValueKind.StringList, value);

        public bool TryAsBoolean(out bool value) { value = IsBoolean && _int != 0; return IsBoolean; }
        public bool TryAsInteger(out int value) { value = IsInteger ? _int : default; return IsInteger; }
        public bool TryAsString([NotNullWhen(true)]out string? value) { value = _ref is string s ? s : default; return IsString; }
        public bool TryAsStringList([NotNullWhen(true)]out StringList? value) { value = _ref is StringList list ? list : default; return IsStringList; }

        public static explicit operator bool(ArgValue value) => value.TryAsBoolean(out var f) ? f : throw new InvalidCastException();
        public static explicit operator int(ArgValue value) => value.TryAsInteger(out var n) ? n : throw new InvalidCastException();
        public static explicit operator string(ArgValue value) => value.TryAsString(out var s) ? s : throw new InvalidCastException();
        public static explicit operator StringList(ArgValue value) => value.TryAsStringList(out var list) ? list : throw new InvalidCastException();

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
    }
}
