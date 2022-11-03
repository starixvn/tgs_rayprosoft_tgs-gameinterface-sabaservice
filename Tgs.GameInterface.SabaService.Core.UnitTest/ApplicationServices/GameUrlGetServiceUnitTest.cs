using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class GameUrlGetServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var query = new GameUrlQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VendorMemberId: "4ucmqplxb029",
            IsMobile: false);

        var vendorMemberId = $"{query.OperatorId}_{query.VendorMemberId}_test";

        var result = new GetGameUrlResult(
            IsPlayerExist: true,
            GameUrl: "http://h0i2ob11.SABADomain.com/Deposit_ProcessLogin.aspx?token=xxxxxxx&lang=xx");
        _ = fakeGameApiClient
            .GetSabaUrlAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                vendorMemberId: Arg.Is(vendorMemberId),
                platformType: Arg.Is(GamePlatformType.Desktop))
            .Returns(result);

        var sut = new GameUrlGetService(
            fakeGameApiClient,
            vendorMember);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
        Assert.IsTrue(actual.GameUrl.Equals(result.GameUrl));
    }
}
