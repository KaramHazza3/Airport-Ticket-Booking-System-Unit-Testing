using System.Globalization;
using AirportTicketBookingSystem.Models.DTOs;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Models.Mappers;

public static class FlightMapper
{
    public static Flight FromCsvDto(FlightCsvDto dto)
    {
        var departureCountry = new Country { Id = Guid.NewGuid(), Name = dto.DepartureCountry };
        var destinationCountry = new Country { Id = Guid.NewGuid(), Name = dto.DestinationCountry };

        var availableClasses = ParseFlightClasses(dto.AvailableClasses);

        var basePrice = availableClasses
            .Where(ac => ac.ClassType == FlightClass.Economy)
            .Sum(x => x.Price);

        return new Flight
        {
            Id = Guid.Parse(dto.Id),
            DepartureDate = DateTime.Parse(dto.DepartureDate),
            Departure = departureCountry,
            Destination = destinationCountry,
            DepartureAirport = new Airport
            {
                Id = Guid.NewGuid(),
                Name = dto.DepartureAirport,
                Country = departureCountry
            },
            ArrivalAirport = new Airport
            {
                Id = Guid.NewGuid(),
                Name = dto.ArrivalAirport,
                Country = destinationCountry
            },
            AvailableClasses = availableClasses,
            BasePrice = basePrice
        };
    }

    
    public static List<FlightClassInfo> ParseFlightClasses(string classesString)
    {
        if (string.IsNullOrWhiteSpace(classesString))
            return new List<FlightClassInfo>();

        return classesString.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(classEntry =>
            {
                var parts = classEntry.Split(':');
                if (parts.Length != 3)
                    throw new FormatException($"Invalid flight class format: '{classEntry}'");

                var classType = Enum.Parse<FlightClass>(parts[0]);
                var availableSeats = int.Parse(parts[1]);
                var price = decimal.Parse(parts[2], CultureInfo.InvariantCulture);

                return new FlightClassInfo(classType, availableSeats, price);
            })
            .ToList();
    }
}
