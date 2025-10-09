using System.Text.Json.Serialization;
using Core.Enums;

namespace WebApp.Models.Users;

public class UserWebApiModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    public string Token { get; set; } = string.Empty;
}
