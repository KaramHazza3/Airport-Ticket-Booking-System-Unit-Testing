using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Models;

public class Airport : IEntity<Guid>
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    public string Name { get; set; }
    [Required]
    public Country Country { get; set; }

    public Airport()
    {
        this.Id = Guid.NewGuid();
    }
    public Airport(string name, Country country)
    {
        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Country = country;
    }
    
    public override string ToString()
    {
        return $"Airport: {Name} - Id: {Id}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Airport other)
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