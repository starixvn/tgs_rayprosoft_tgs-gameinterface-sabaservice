namespace Tgs.GameInterface.SabaService.ViewModels;

public record FundResultViewModel(
    bool Check,
    bool IsOrderExist,
    string VendorTransId,
    string ActualVendorTransId,
    long TransId,
    decimal Balance);
