#nullable enable

namespace DocoptNet.Tests
{
    using NUnit.Framework.Constraints;

    public static class NUnitExtensions
    {
        // Credit: https://github.com/nunit/nunit/issues/2820#issuecomment-511488287

        public static Constraint ParamName(this ConstraintExpression with, string paramName) =>
            with.Property("ParamName").EqualTo(paramName);
    }
}
