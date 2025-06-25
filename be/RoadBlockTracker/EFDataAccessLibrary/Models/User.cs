using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccessLibrary.Models
{
    public enum Role
    {
        ADMIN,
        USER
    }
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public DateOnly? BirthDate { get; set; }
        public Role Role { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public User(string username, string email, string passwordHash, string name,
                   string lastName, DateOnly birthDate, Role role)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Name = name;
            LastName = lastName;
            BirthDate = birthDate;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
