using Microsoft.AspNetCore.Mvc;

namespace Rest_API.Models
{
    public class Miembro
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { set; get; } = string.Empty;
        public string Username { set; get; } = string.Empty;
        public string PhoneNumber { set; get; } = string.Empty;
        public DateTime BirthDate { set; get; }
        public string Country { set; get; } = string.Empty;
        public string Bio { set; get; } = string.Empty;
        public string Role { set; get; } = string.Empty;
        public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
        public string UserId { set; get; } = string.Empty;
    }
}
