namespace AirportTicketBookingSystem.Models.DTOs;

public class FlightCsvDto
{
    public string Id { get; set; }
    public string DepartureCountry { get; set; }
    public string DepartureAirport { get; set; }
    public string DestinationCountry { get; set; }
    public string ArrivalAirport { get; set; }
    public string DepartureDate { get; set; }
    public string AvailableClasses { get; set; }
}
