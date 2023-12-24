using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Helpers;

namespace wDNS.Tests.Common.Helpers;

[TestClass]
public class NetworkHelpersTests
{
    [TestMethod]
    public void DefaultGateway()
    {
        // This test must be run only if there is an active connection.
        var address = NetworkHelpers.GetDefaultGateway();
        Assert.IsNotNull(address);
    }
}
