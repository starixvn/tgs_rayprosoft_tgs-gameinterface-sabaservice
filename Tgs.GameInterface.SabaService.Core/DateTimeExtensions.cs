namespace Tgs.GameInterface.SabaService.Core;

public static class DateTimeExtensions
{
    public static DateTime StringConvertToDatetime(this string str)
    {
        var _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        if (string.IsNullOrEmpty(str))
        {
            return DateTime.UnixEpoch;
        }
        var date = DateTime.Parse(str);

        return new DateTimeOffset(date, _timeZone.GetUtcOffset(date)).UtcDateTime;
    }

    public static string DatetimeConvertToString(this DateTime date)
    {
        var _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        return TimeZoneInfo.ConvertTime(date.ToUniversalTime(), _timeZone).ToString("yyyy-MM-ddTHH:mm:ss");
    }
}
