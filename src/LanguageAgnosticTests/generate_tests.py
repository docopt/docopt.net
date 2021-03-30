#! /usr/bin/env python

#
# Quick and dirty script to generate a C# test suite
# out of testcases.docopt
#
# e.g. generate_tests >LanguageAgnosticTests.generated.cs
#

import sys, json, re, os

fixtures = open(os.path.join(os.path.dirname(__file__), 'testcases.docopt'), 'r').read()

# remove comments
fixtures = re.sub('#.*$', '', fixtures, flags=re.M)

ids = [int(x) for x in sys.argv[1:]] if len(sys.argv) > 1 else None

fmt = """
      [Test]
      public void Test_%d()
      {
          var doc = @"%s";
          var actual = Docopt(doc, @"%s");
          var expected = @"%s";
          CheckResult(expected, actual);
      }
"""

print ("""using System.Collections.Generic;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public partial class LanguageAgnosticTests
    {
      #region Language agnostic tests generated code
""")

index = 0
for fixture in fixtures.split('r"""'):
    doc, _, body = fixture.partition('"""')
    for case in body.split('$')[1:]:
        index += 1
        if ids is not None and index not in ids:
            continue
        argv, _, expect = case.strip().partition('\n')
        prog, _, argv = argv.strip().partition(' ')
        assert prog == 'prog', repr(prog)
        doc = doc.replace('"', '""')
        expect = expect.replace('"', '""')
        argv = argv.replace('"', '"""')
        print (fmt % (index,doc,argv,expect))

print ("""
      #endregion
    }
}""")
