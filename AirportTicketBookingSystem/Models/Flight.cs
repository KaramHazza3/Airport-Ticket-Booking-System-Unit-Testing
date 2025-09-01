using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.CustomAttributes;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Models;

public class Flight : IEntity<Guid>
{
    [Required]
    public Guid Id { get; init; }
    public decimal BasePrice { get; set; }
    [Required]
    public Country Departure { get; set; }
    [Required]
    public Country Destination { get; set; }
    [Required]
    public Airport DepartureAirport { get; set; }
    [Required]
    public Airport ArrivalAirport { get; set; }
    [Required]
    [Future]
    public DateTime DepartureDate { get; set; }
    [Required]
    public List<FlightClassInfo> AvailableClasses { get; init; }
    private int TotalAvailableSeats => AvailableClasses.Sum(c => c.AvailableSeats);

    public Flight()
    {
        
    }
    public Flight(Country departure,
        Country destination, Airport departureAirport,
        Airport arrivalAirport, DateTime departureDate,
        List<FlightClassInfo> availableClasses)
    {
        this.Id = Guid.NewGuid(); 
        this.Departure = departure;
        this.Destination = destination;
        this.DepartureAirport = departureAirport;
        this.ArrivalAirport = arrivalAirport;
        this.DepartureDate = departureDate;
        this.AvailableClasses = availableClasses;
        this.BasePrice = AvailableClasses!.FirstOrDefault(ac => ac.ClassType == FlightClass.Economy)?.Price ?? 0;
    }
    
    public override string ToString()
    {
        var classInfo = string.Join(", ",
            AvailableClasses.Select(c =>
                $"{c.ClassType} (Seats: {c.AvailableSeats}, Price: {c.Price})"));

        return $"Flight ID: {Id}\n" +
               $"Base Price: {BasePrice:C}\n" +
               $"From: {Departure} - Airport: {DepartureAirport}\n" +
               $"To: {Destination} - Airport: {ArrivalAirport}\n" +
               $"Departure Date: {DepartureDate:yyyy-MM-dd HH:mm}\n" +
               $"Total Available Seats: {TotalAvailableSeats}\n" +
               $"Available Classes: {classInfo}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Flight other)
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