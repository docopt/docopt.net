using System.Collections.Generic;

namespace NDocOpt
{
    public class Command : Argument
    {
        public Command(string name, ValueObject value = null) : base(name, value ?? new ValueObject(false))
        {
        }

        public override SingleMatchResult SingleMatch(IList<Pattern> left)
        {
            for (var i = 0; i < left.Count; i++)
            {
                var pattern = left[i];
                if (pattern is Argument)
                {
                    if (pattern.Value.ToString() == Name)
                        return new SingleMatchResult(i, new Command(Name, new ValueObject(true)));
                    break;
                }
            }
            return new SingleMatchResult();
        }
    }
}