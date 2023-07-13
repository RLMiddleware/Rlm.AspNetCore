using Microsoft.AspNetCore.Http;

namespace Rlm.Extensions;

public static class DictionaryExtensions
{
    public static string FormatToLog(this Dictionary<string, string> dictionary) {
        return dictionary.Keys.Aggregate("", (current, key) => current + $"\t{key}: {dictionary[key]}\n");
    }
    
    
    public static Dictionary<string,string> Invoke(this Dictionary<string, Func<HttpContext, string>> dictionary, HttpContext context) {
        return dictionary.Keys.ToDictionary(prop => prop, prop => dictionary[prop].Invoke(context));
    }
    
    public static string FormatHeaders(IHeaderDictionary headers) => string.Join(",",
        headers.Select(kvp => $"{{{kvp.Key}: {string.Join(", ", kvp.Value)}}}"));
    
}