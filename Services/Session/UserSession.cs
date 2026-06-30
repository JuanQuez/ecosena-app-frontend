using System.Text;
using System.Text.Json;

namespace EcosenaApp.Services.Session;

public class UserSession : IUserSession
{
    public string Role { get; private set; } = "Invitado";
    public string Name { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public bool IsAuthenticated { get; private set; }

    public void SetFromToken(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2)
            {
                SetGuest();
                return;
            }

            var payload = DecodeBase64Url(parts[1]);
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            UserId = GetClaim(root, "nameid");
            Name = GetClaim(root, "name");
            Role = GetClaim(root, "role");
            IsAuthenticated = !string.IsNullOrEmpty(Role) && Role != "Invitado";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UserSession.SetFromToken error: {ex.Message}");
            SetGuest();
        }
    }

    public void SetGuest()
    {
        Role = "Invitado";
        Name = string.Empty;
        UserId = string.Empty;
        IsAuthenticated = false;
    }

    public void Clear()
    {
        Role = "Invitado";
        Name = string.Empty;
        UserId = string.Empty;
        IsAuthenticated = false;
    }

    private static string GetClaim(JsonElement root, string key)
    {
        if (root.TryGetProperty(key, out var value))
            return value.GetString() ?? string.Empty;

        // Algunos JwtSecurityTokenHandler mapean claims cortas a URIs largas
        foreach (var property in root.EnumerateObject())
        {
            if (property.Name.EndsWith($"/{key}", StringComparison.OrdinalIgnoreCase))
                return property.Value.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string DecodeBase64Url(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        var bytes = Convert.FromBase64String(padded);
        return Encoding.UTF8.GetString(bytes);
    }
}
