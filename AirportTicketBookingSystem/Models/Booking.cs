using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.CustomAttributes;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Models;

public class Booking : IEquatable<Booking>, IEntity<Guid>
{
    [Required]
    public Guid Id { get; init; }
    [Required]
    public User Passenger { get; set; }
    [Required]
    public Flight Flight { get; set; }
    [Required]
    public FlightClass FlightClass { get; set; }

    [Required]
    [Future] 
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
   
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Must be greater than 0")]
    public decimal Price { get; set; }

    public Booking()
    {
        this.Id = Guid.NewGuid();
    }
    public Booking(User passenger, Flight flight, FlightClass flightClass)
    {
        this.Id = Guid.NewGuid();
        this.Passenger = passenger;
        this.Flight = flight;
        this.FlightClass = flightClass;
        this.Price = this.Flight.AvailableClasses
            .Where(x => x.ClassType == flightClass)
            .Sum(x => x.Price);
    }

    public override bool Equals(object? obj) => Equals(obj as Booking);

    public bool Equals(Booking? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }
    public override int GetHashCode() => Id.GetHashCode();
    
    public override string ToString()
    { 
        return $@"Booking {{
    Id           : {Id}
    PassengerName  : {Passenger.Name}
    FlightId    : {Flight.Id}
    FlightClass  : {FlightClass}
    BookingDate  : {BookingDate:yyyy-MM-dd HH:mm:ss} (UTC)
}}";
    }
    
}