namespace DocoptNet
{
    class Command : LeafPattern
    {
        public Command(string name, bool value = false) : base(name, value ? Value.True : Value.False)
        {
        }
    }
}
