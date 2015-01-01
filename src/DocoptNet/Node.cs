using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocoptNet
{
    public enum ValueType { Bool, List, String, }

    public class ArgumentNode : Node
    {
        public ArgumentNode(string name, ValueType valueType) : base(name, valueType) { }
    }

    public class OptionNode : Node
    {
        public OptionNode(string name, ValueType valueType) : base(name, valueType) { }
    }

    public class CommandNode : Node
    {
        public CommandNode(string name) : base(name, ValueType.Bool) { }
    }

    public abstract class Node : IEquatable<Node>
    {
        private class EmptyNode : Node
        {
            public EmptyNode() : base("", (ValueType)0) { }
        }

        /// <summary>
        /// Indicates an empty or non-existant node.
        /// </summary>
        public static readonly Node Empty = new EmptyNode();

        protected Node(string name, ValueType valueType)
        {
            if (name == null) throw new ArgumentNullException("name");

            this.Name = name;
            this.ValueType = valueType;
        }

        public ValueType ValueType { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", GetType().Name, Name, ValueType);
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
