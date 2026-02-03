using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoggAutorz.Users
{
    public class UsersEntity
    {
        private string email;

        public UsersEntity() { }

        public UsersEntity(int Id, string userName, string email, string passwordHash, string[] Role)
        {
            UserId = Id;
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            totalTry = 3;
            role = Role;
        }


        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int UserId { get; private set; }
        [Required][MaxLength(100)] public string UserName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [JsonIgnore][DefaultValue("Employee")] public string[]? role { get; set; } = new[] { "Employee" };
        [JsonIgnore][Required][MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;

        [JsonIgnore] public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        [JsonIgnore] public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        [JsonIgnore] public int totalTry { get; set; } = 3;


        public void Update(string? _userName, string? _email, string? _passwordhash, string[] role )
        {
            if (!string.IsNullOrWhiteSpace(_email))
                Email = _email.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(_userName))
                UserName = _userName;

            if (!string.IsNullOrWhiteSpace(_email))
                Email = _email;

            if (!string.IsNullOrWhiteSpace(_passwordhash))
                PasswordHash = _passwordhash;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
