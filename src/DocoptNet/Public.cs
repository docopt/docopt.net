// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2012 Vladimir Keleshev, 2013 Dinh Doan Van Bien, 2021 Atif Aziz

#if DOCOPTNET_PUBLIC

namespace DocoptNet
{
    public partial class Docopt { }
    public partial class PrintExitEventArgs { }
    public partial class DocoptBaseException { }
    public partial class DocoptExitException { }
    public partial class DocoptInputErrorException { }
    public partial class DocoptLanguageErrorException { }
    public partial class ValueObject { }
    public partial class ArgsParseOptions { }
    public partial class StringList { }
    public partial struct ArgValue { }
    public partial interface IParser<out T> { }
    public partial interface IHelpFeaturingParser<out T> { }
    public partial interface IVersionFeaturingParser<out T> { }
    public partial interface IBaselineParser<out T> { }
    public partial interface IArgumentsResult<out T> { }
    public partial interface IInputErrorResult { }
    public partial interface IHelpResult { }
    public partial interface IVersionResult { }
}

#endif
