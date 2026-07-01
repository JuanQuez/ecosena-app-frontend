namespace EcosenaApp.Models.Auth;

public class LoginResDto
{
    public string? Jwt { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
    public int? RemainingAttempts { get; set; }
    public string? Message { get; set; }
}
