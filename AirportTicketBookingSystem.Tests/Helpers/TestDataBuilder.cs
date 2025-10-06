using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;
using AutoFixture;

namespace AirportTicketBookingSystem.Tests.Helpers;

public class TestDataBuilder
{
    private readonly IFixture _fixture;

    public TestDataBuilder(IFixture fixture)
    {
        _fixture = fixture;
    }

    public Flight BuildFlight(
        decimal basePrice = 100m,
        int daysFromNow = 1,
        List<FlightClassInfo>? classes = null)
    {
        var flightClasses = classes ?? new List<FlightClassInfo>
        {
            new(FlightClass.Economy, 300, 100m)
        };

        return _fixture.Build<Flight>()
            .With(f => f.BasePrice, basePrice)
            .With(f => f.DepartureDate, DateTime.UtcNow.AddDays(daysFromNow))
            .With(f => f.AvailableClasses, flightClasses)
            .Create();
    }

    public Booking BuildBooking(
        decimal price = 100m,
        FlightClass flightClass = FlightClass.Economy,
        DateTime? bookingDate = null)
    {
        var flight = BuildFlight();

        return _fixture.Build<Booking>()
            .With(b => b.Flight, flight)
            .With(b => b.Price, price)
            .With(b => b.BookingDate, bookingDate ?? DateTime.UtcNow.AddDays(1))
            .With(b => b.FlightClass, flightClass)
            .Create();
    }
}
