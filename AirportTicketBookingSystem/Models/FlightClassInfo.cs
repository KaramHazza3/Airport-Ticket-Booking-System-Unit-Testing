using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Models;

public class FlightClassInfo
{
    [Required]
    public FlightClass ClassType { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Seats must be greater than 0")]
    public int AvailableSeats { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public FlightClassInfo(FlightClass classType, int availableSeats, decimal price)
    {
        this.ClassType = classType;
        this.AvailableSeats = availableSeats;
        this.Price = price;
    }
    
    public override string ToString()
    {
        return $"Class: {ClassType}, Seats Available: {AvailableSeats}, Price: {Price:C}";
    }
}