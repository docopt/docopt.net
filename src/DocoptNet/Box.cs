namespace DocoptNet
{
    static class Box
    {
        public static Box<T> Specific<T>(T value) => Box<T>.Specific(value);
    }

    readonly struct Box<T>
    {
        readonly bool   _isSpecific;
        readonly T      _value;
        readonly object _boxed;

        public static Box<T> Specific(T value) => new(true, value, null);
        public static Box<T> General(object value) => new(false, default, (T)value);

        Box(bool isSpecific, T value, object boxed)
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
