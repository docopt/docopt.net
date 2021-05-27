namespace DocoptNet
{
    static class Var
    {
        public static Var<T> Specific<T>(T value) => Var<T>.Specific(value);
    }

    readonly struct Var<T>
    {
        readonly bool   _isSpecific;
        readonly T      _value;
        readonly object _boxed;

        public static Var<T> Specific(T value) => new(true, value, null);
        public static Var<T> General(object value) => new(false, default, value);

        Var(bool isSpecific, T value, object boxed)
        {
            _isSpecific = isSpecific;
            _value = value;
            _boxed = boxed;
        }

        public T Value => _isSpecific ? _value : (T)_boxed;
        public object Object => _isSpecific ? _value : _boxed;

        public override string ToString() =>
            (_isSpecific ? Value?.ToString() : Object?.ToString()) ?? string.Empty;
    }
}
