using Microsoft.Extensions.Options;

namespace Tgs.GameInterface.SabaService.Core.UnitTests;

public class GameVendorMemberUnitTest
{
	[Fact]
	public void Convert_To_Game_Provider_Member_Dev_Environment()
	{
		var options = Options.Create(new GameInterfaceSettingsOptions()
		{
			ReleaseNamespace = "dev"
		});
		var vendorMember = new GameVendorMember(options);

		var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

		Assert.NotNull(actual);
		Assert.Equal("tg_4ucmqplxb029_test", actual);
	}

	[Fact]
	public void Convert_To_Game_Provider_Member_Prod_Environment()
	{
		var options = Options.Create(new GameInterfaceSettingsOptions()
		{
			ReleaseNamespace = "prod"
		});
		var vendorMember = new GameVendorMember(options);

		var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

		Assert.NotNull(actual);
		Assert.Equal("tg_4ucmqplxb029", actual);
	}

	[Fact]
	public void Convert_To_Game_Provider_Member_Staging_Environment()
	{
		var options = Options.Create(new GameInterfaceSettingsOptions()
		{
			ReleaseNamespace = "staging"
		});
		var vendorMember = new GameVendorMember(options);

		var actual = vendorMember.ConvertToGameProviderMember("tg", "4ucmqplxb029");

		Assert.NotNull(actual);
		Assert.Equal("tg_4ucmqplxb029_test", actual);
	}

	[Fact]
	public void Convert_To_Platform_Member_Account_Is_Test()
	{
		var options = Options.Create(new GameInterfaceSettingsOptions()
		{
			ReleaseNamespace = "prod"
		});
		var vendorMember = new GameVendorMember(options);

		var actual = vendorMember.ConvertToPlatformMember("tg", "tg_4ucmqplxb029_test");

		Assert.NotNull(actual);
		Assert.Equal("4ucmqplxb029", actual);
	}

	[Fact]
	public void Convert_To_Platform_Member_Account_Is_Official()
	{
		var options = Options.Create(new GameInterfaceSettingsOptions()
		{
			ReleaseNamespace = "prod"
		});
		var vendorMember = new GameVendorMember(options);

		var actual = vendorMember.ConvertToPlatformMember("tg", "tg_4ucmqplxb029");

		Assert.NotNull(actual);
		Assert.Equal("4ucmqplxb029", actual);
	}
}
