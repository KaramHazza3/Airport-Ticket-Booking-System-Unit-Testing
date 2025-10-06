using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Models;

public class User : IEntity<Guid>
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    public string Name { get; init; } = string.Empty;
    [Required]
    public string Email { get; init; } = string.Empty;
    [Required]
    public string Password { get; init; } = string.Empty;
    [Required]
    public UserRole Role { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }

    public User(string name, string email, string password, UserRole role)
    {
        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Email = email;
        this.Password = password;
        this.Role = role;
    }

    public override string ToString()
    {
        return $"Id: {this.Id}, Name: {this.Name}, Email: {this.Email}, Role: {this.Role}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not User other)
        {
            return false;
        }

        return this.Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }
}