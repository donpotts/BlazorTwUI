namespace BlazorTwUI.Services;

using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// Masks credentials before agent data is shown, logged or exported. Credential-key classification rules:
/// keys are split into lowercase words so <c>accessToken</c>, <c>access_token</c> and <c>Access-Token</c> all match.
/// </summary>
public static partial class RedactionService
{
    public const string Mask = "[redacted]";

    [GeneratedRegex("password|passwd|passphrase|passcode|credential|api(key|token)|privatekey|subscriptionkey|(access|refresh|auth|session)token|clientsecret")]
    private static partial Regex Credential();

    [GeneratedRegex(" (secret|id ?token)s? | (authorization|cookies?|jwt|bearer ?token|[cx]srf ?token) $|^ (pwd|pass|token|otp) $")]
    private static partial Regex CredentialWord();

    // Metadata about a credential is not the credential: passwordExpiresAt, credentialId.
    [GeneratedRegex(" (ids?|at|in|expires?|expiry|length|count|enabled|required|policy|hint|type) $")]
    private static partial Regex Metadata();

    [GeneratedRegex("([a-z][\\w+.-]{0,31}://)[^\\s/?#\"]*@", RegexOptions.IgnoreCase)]
    private static partial Regex UserInfo();

    [GeneratedRegex("\\b(bearer|basic)\\s+[A-Za-z0-9._~+/=-]{8,}", RegexOptions.IgnoreCase)]
    private static partial Regex AuthScheme();

    [GeneratedRegex("([\\w$.%+\\[\\]-]{1,64})(\\s*(?:=|:(?=\\s|$))\\s*)([^\\s&,;)}\\]]+)", RegexOptions.IgnoreCase)]
    private static partial Regex Pair();

    public static bool IsCredentialKey(string key)
    {
        var words = " " + string.Join(' ', Regex.Split(Regex.Replace(key, "([a-z\\d])(?=[A-Z])", "$1 ").ToLowerInvariant(), "[^a-z\\d]+")
            .Where(w => w.Length > 0)) + " ";
        return !Metadata().IsMatch(words) && (Credential().IsMatch(words.Replace(" ", "")) || CredentialWord().IsMatch(words));
    }

    /// <summary>Redacts free text: URL userinfo, auth schemes and <c>key=value</c> / <c>key: value</c> pairs with credential keys.</summary>
    public static string RedactText(string? text)
    {
        if (string.IsNullOrEmpty(text)) return text ?? "";
        text = UserInfo().Replace(text, "$1" + Mask + "@");
        text = AuthScheme().Replace(text, m => m.Groups[1].Value + " " + Mask);
        return Pair().Replace(text, m => IsCredentialKey(m.Groups[1].Value) ? m.Groups[1].Value + m.Groups[2].Value + Mask : m.Value);
    }

    /// <summary>Redacts a JSON document by key (recursively). Invalid JSON falls back to <see cref="RedactText"/>.</summary>
    public static string RedactJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return json ?? "";
        try
        {
            using var doc = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                Write(w, doc.RootElement, null);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (JsonException)
        {
            return RedactText(json);
        }
    }

    private static void Write(Utf8JsonWriter w, JsonElement e, string? key)
    {
        if (key is not null && IsCredentialKey(key) && e.ValueKind is not (JsonValueKind.Null or JsonValueKind.Object or JsonValueKind.Array))
        {
            w.WriteStringValue(Mask);
            return;
        }

        switch (e.ValueKind)
        {
            case JsonValueKind.Object:
                w.WriteStartObject();
                foreach (var p in e.EnumerateObject())
                {
                    w.WritePropertyName(p.Name);
                    Write(w, p.Value, p.Name);
                }
                w.WriteEndObject();
                break;
            case JsonValueKind.Array:
                w.WriteStartArray();
                foreach (var item in e.EnumerateArray()) Write(w, item, key);
                w.WriteEndArray();
                break;
            case JsonValueKind.String:
                w.WriteStringValue(RedactText(e.GetString()));
                break;
            default:
                e.WriteTo(w);
                break;
        }
    }
}
