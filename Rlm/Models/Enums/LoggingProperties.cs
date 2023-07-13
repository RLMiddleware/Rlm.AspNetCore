namespace Rlm.Models.Enums;

public class LoggingProperties
{
    public const int None = -1;
    public const int All = 0;
    public const int Scheme = 1;
    public const int Host = 2;
    public const int Path = 3;
    public const int Method = 4;
    public const int QueryParams = 5;
    public const int Body = 6;
    public const int Headers = 7;
    public const int RemoteIpv4 = 8;
    public const int RemoteIpv6 = 9;
}