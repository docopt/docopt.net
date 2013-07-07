namespace DocoptNet
{
    /// <summary>
    ///     Marker/placeholder for [options] shortcut.
    /// </summary>
    internal class OptionsShortcut : Optional
    {
        public OptionsShortcut() : base(new Pattern[0])
        {
        }
    }
}