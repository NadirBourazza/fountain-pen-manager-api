using Newtonsoft.Json;

namespace Models.Requests.UserManagement;

public class GetUserRequest
{
    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; init; }

    [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
    public string? Password { get; init; }
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(Email)) throw new ArgumentNullException(nameof(Email), "email is required.");
        if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password), "password is required.");
    }
}