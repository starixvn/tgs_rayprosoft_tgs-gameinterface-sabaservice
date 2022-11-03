using Microsoft.Extensions.Options;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class VendorMemberUnitTest
{
    [TestMethod]
    public void Convert_To_Game_Provider_Member_Dev_Environment()
    {
        var options = Options.Create(new GameInterfaceSettingsOptions()
        {
            ReleaseNamespace = "dev"
        });
        var vendorMember = new VendorMember(options);

        var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

        Assert.IsNotNull(actual);
        Assert.IsTrue("tg_4ucmqplxb029_test".Equals(actual));
    }

    [TestMethod]
    public void Convert_To_Game_Provider_Member_Prod_Environment()
    {
        var options = Options.Create(new GameInterfaceSettingsOptions()
        {
            ReleaseNamespace = "prod"
        });
        var vendorMember = new VendorMember(options);

        var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

        Assert.IsNotNull(actual);
        Assert.IsTrue("tg_4ucmqplxb029".Equals(actual));
    }

    [TestMethod]
    public void Convert_To_Game_Provider_Member_Staging_Environment()
    {
        var options = Options.Create(new GameInterfaceSettingsOptions()
        {
            ReleaseNamespace = "staging"
        });
        var vendorMember = new VendorMember(options);

        var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

        Assert.IsNotNull(actual);
        Assert.IsTrue("tg_4ucmqplxb029_test".Equals(actual));
    }

    [TestMethod]
    public void Convert_To_Platform_Member_Account_Is_Test()
    {
        var options = Options.Create(new GameInterfaceSettingsOptions()
        {
            ReleaseNamespace = "dev"
        });
        var vendorMember = new VendorMember(options);

        var actual = vendorMember.ConvertToPlatformMember("tg", "tg_4ucmqplxb029_test");

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual));
    }

    [TestMethod]
    public void Convert_To_Platform_Member_Account_Is_Official()
    {
        var options = Options.Create(new GameInterfaceSettingsOptions()
        {
            ReleaseNamespace = "prod"
        });
        var vendorMember = new VendorMember(options);

        var actual = vendorMember.ConvertToPlatformMember("tg", "tg_4ucmqplxb029");

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual));
    }
}
