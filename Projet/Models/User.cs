using Microsoft.AspNetCore.Mvc;

namespace Projet.Models
{
    public class User : IdentifyUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public User() { }
        public User(string firstName, string lastName, string email, string password, string role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
        }

    }
}
