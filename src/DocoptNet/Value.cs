#nullable enable

namespace DocoptNet
{
    using System;
    using System.Collections.Generic;

    enum ValueKind { Null, Boolean, Integer, String, StringList }

    readonly struct Value
    {
        readonly int _int;    // stores: bool, int
        readonly object? _ref; // stores: null, T, ArrayList

        public static readonly Value Null  = new(ValueKind.Null, null);
        public static readonly Value True  = new(ValueKind.Boolean, 1);
        public static readonly Value False = new(ValueKind.Boolean, 0);

        public static Value Init(bool value)         => value ? True : False;
        public static Value Init(int value)          => new(ValueKind.Integer, value);
        public static Value Init(string value)       => new(ValueKind.String, value);
        public static Value Init(List<string> value) => new(ValueKind.StringList, value);

        Value(ValueKind kind, object? value) : this(kind, 0, value) { }
        Value(ValueKind kind, int @int, object? @ref = null) => (Kind, _int, _ref) = (kind, @int, @ref);

        public ValueKind Kind { get; }

        public bool IsNull       => Kind == ValueKind.Null;
        public bool IsBoolean    => Kind == ValueKind.Boolean;
        public bool IsInteger    => Kind == ValueKind.Integer;
        public bool IsString     => Kind == ValueKind.String;
        public bool IsStringList => Kind == ValueKind.StringList;

        public bool IsTrue  => IsBoolean && !IsFalse;
        public bool IsFalse => IsBoolean && _int == 0;

        public object? Box() =>
            Kind switch
            {
                ValueKind.Null       => null,
                ValueKind.Boolean    => (bool)this ? Boxed.True : Boxed.False,
                ValueKind.Integer    => Boxed.Integer((int)this),
                ValueKind.String     => (string)this,
                ValueKind.StringList => (List<string>)this,
                _                    => throw new InvalidOperationException()
            };

        public override string ToString() => ValueObject.Format(Box());

        public static explicit operator bool(Value v) => v.Kind == ValueKind.Boolean ? v._int != 0 : throw new InvalidCastException();
        public static explicit operator int(Value v) => v.Kind == ValueKind.Integer ? v._int : throw new InvalidCastException();
        public static explicit operator string(Value v) => v._ref is string s ? s : throw new InvalidCastException();
        public static explicit operator List<string>(Value v) => v._ref is List<string> list ? list : throw new InvalidCastException();

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
