using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;
using AutoFixture;

namespace AirportTicketBookingSystem.Tests.Customizations;

public class FlightCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var flightClasses = new List<FlightClassInfo>()
        {
            new FlightClassInfo(FlightClass.Economy, 300, 100m)
        };

        var flight = fixture.Build<Flight>()
            .With(f => f.BasePrice, 100m)
            .With(f => f.DepartureDate, DateTime.UtcNow.AddDays(1))
            .With(f => f.AvailableClasses, flightClasses)
            .Create();
        
        fixture.Inject(flight);
    }
}