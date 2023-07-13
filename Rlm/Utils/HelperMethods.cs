using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Rlm.Models;

namespace Rlm.Utils;

public static class HelperMethods
{
    internal static string FormatBody(string body, RlmSettings settings) {
        try {
            var deserializeObject = JsonConvert.DeserializeObject(body);
            var prettyPrintedString = JsonConvert.SerializeObject(deserializeObject, Formatting.Indented);

            if (settings.ObscureCredentials) {
                return ObscureCredentials(prettyPrintedString, settings);
            }

            return prettyPrintedString;
        }
        catch (Exception) {
            return body;
        }
    }

    private static string ObscureCredentials(string body, RlmSettings settings) {
        // rewrite passwords or security data
        var currentPattern = PublicConstants.JsonKeyValueSubstitutionPattern.Replace("{keywords}", string.Join('|', settings.ObscureKeywords));

        const string substitution = "\"$1\":\"********\"";
        const RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
        var regex = new Regex(currentPattern, options);

        // PASSWORD REWRITE
        return regex.IsMatch(body) ? regex.Replace(body, substitution) : body;
    }
}