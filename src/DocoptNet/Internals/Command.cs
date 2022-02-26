namespace DocoptNet.Internals
{
    class Command : LeafPattern
    {
        public Command(string name, bool value = false) : base(name, value ? ArgValue.True : ArgValue.False)
        {
        }
    }
}
