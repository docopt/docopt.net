namespace DocoptNet.Generated
{
    using System;

    #if DOCOPTNET_PUBLIC
    public
    #endif
         enum ValueType { Bool, List, String, }

    partial class ArgumentNode : Node
    {
        public ArgumentNode(string name, ValueType valueType) : base(name, valueType) { }
    }

    partial class OptionNode : Node
    {
        public OptionNode(string name, ValueType valueType) : base(name, valueType) { }
    }

    partial class CommandNode : Node
    {
        public CommandNode(string name) : base(name, ValueType.Bool) { }
    }

    abstract partial class Node : IEquatable<Node>
    {
        protected Node(string name, ValueType valueType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            this.Name = name;
            this.ValueType = valueType;
        }

        public ValueType ValueType { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return $"{GetType().Name} {Name} {ValueType}";
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.ValueType.GetHashCode();
        }

        public bool Equals(Node other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Name == this.Name
                && other.ValueType == this.ValueType;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Node)obj);
        }
    }

}
