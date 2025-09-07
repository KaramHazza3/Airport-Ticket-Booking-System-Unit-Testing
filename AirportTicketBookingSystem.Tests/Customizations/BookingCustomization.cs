using AirportTicketBookingSystem.Models;
using AutoFixture;

namespace AirportTicketBookingSystem.Tests.Customizations;

public class BookingCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var booking = fixture.Build<Booking>()
            .With(b => b.Flight, fixture.Create<Flight>())
            .With(b => b.Price, 100m)
            .With(b => b.BookingDate, DateTime.UtcNow.AddDays(1))
            .Create();
        
        fixture.Inject(booking);
    }
}