namespace Rlm.Models;

public class PublicConstants
{
    public const string HttpResponsePlaceholder = "response";
    public const string HttpRequestPlaceholder = "request";
    public const string JsonKeyValueSubstitutionPattern = @"\""({keywords})\"":\""(.*?)\""";
    public const string ApiVersionPattern = @"((\/api\/)(v\d))(/(.+))";
}