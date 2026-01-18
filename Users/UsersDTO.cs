using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LoggAutorz.Users
{    
        public class RegisterUserDTO
        {
            [Required]
        [MaxLength(100)]
            public string Name { get; set; } = string.Empty;
            [Required]
        [EmailAddress]
        [MaxLength(100)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(8)]
            
        public string Password { get; set; } = string.Empty;
        [JsonIgnore][DefaultValue("Employee")]public string[] role { get; set; } = new[] { "Employee" };
    }

}
