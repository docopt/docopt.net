namespace DocoptNet
{
    class OneOrMore : BranchPattern
    {
        public OneOrMore(params Pattern[] patterns)
            : base(patterns)
        {
        }
    }
}
