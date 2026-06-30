namespace EcosenaApp.Services.Session;

public interface IUserSession
{
    string Role { get; }
    string Name { get; }
    string UserId { get; }
    bool IsAuthenticated { get; }
    void SetFromToken(string jwt);
    void SetGuest();
    void Clear();
}
