#if NET9_0_OR_GREATER
#pragma warning disable MEZ_NET9 // Type or member is obsolete
#endif
using Meziantou.Framework.IO;
using Xunit;

namespace Meziantou.Framework.Tests.IO;

public class TeeTextWriterTests
{
    [Fact]
    public void WriteTest01()
    {
        using var sw1 = new StringWriter();
        using var sw2 = new StringWriter();
        using var tee = new TeeTextWriter(sw1, sw2);
        tee.Write("abc");
        tee.Flush();
        Assert.Equal("abc", sw1.ToString());
        Assert.Equal("abc", sw2.ToString());
    }
}
