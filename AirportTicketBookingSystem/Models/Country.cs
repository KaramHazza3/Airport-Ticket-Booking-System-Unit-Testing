using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Models;

public class Country : IEntity<Guid>
{
    public Guid Id { get; init; }
    [Required]
    public  string Name { get; set; }
    public string Code { get; set; } = string.Empty;
    
    public Country() {}
    public Country(string name, string code)
    {
        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Code = code;
    }
    public override string ToString()
    {
        return $"{Name} ({Code}) - Id: {Id}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Country other)
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