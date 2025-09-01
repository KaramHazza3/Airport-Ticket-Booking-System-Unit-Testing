using System.Globalization;
using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;
using AirportTicketBookingSystem.Models.DTOs;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Common.Validators.CsvValidators.Flight;

public static class FlightCsvValidator
{
    public static List<CsvValidationError> Validate(FlightCsvDto dto, int rowNumber)
    {
        var errors = new List<CsvValidationError>();

        ValidateId(dto.Id, rowNumber, errors);
        ValidateRequiredField(dto.DepartureCountry, nameof(dto.DepartureCountry), rowNumber, errors);
        ValidateRequiredField(dto.DepartureAirport, nameof(dto.DepartureAirport), rowNumber, errors);
        ValidateRequiredField(dto.DestinationCountry, nameof(dto.DestinationCountry), rowNumber, errors);
        ValidateRequiredField(dto.ArrivalAirport, nameof(dto.ArrivalAirport), rowNumber, errors);
        ValidateDepartureDate(dto.DepartureDate, rowNumber, errors);
        ValidateAvailableClasses(dto.AvailableClasses, rowNumber, errors);

        return errors;
    }

    private static void ValidateId(object? id, int rowNumber, List<CsvValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(Convert.ToString(id)) || !Guid.TryParse(Convert.ToString(id), out _))
        {
            errors.Add(new CsvValidationError(rowNumber, nameof(FlightCsvDto.Id), "Invalid or missing Id."));
        }
    }

    private static void ValidateRequiredField(string? value, string propertyName, int rowNumber, List<CsvValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new CsvValidationError(rowNumber, propertyName, $"{propertyName} is required."));
        }
    }

    private static void ValidateDepartureDate(string? date, int rowNumber, List<CsvValidationError> errors)
    {
        if (!DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            errors.Add(new CsvValidationError(rowNumber, nameof(FlightCsvDto.DepartureDate), "Invalid DepartureDate format."));
        }
        else if (parsedDate < DateTime.Now)
        {
            errors.Add(new CsvValidationError(rowNumber, nameof(FlightCsvDto.DepartureDate), "DepartureDate cannot be in the past."));
        }
    }

    private static void ValidateAvailableClasses(string? availableClasses, int rowNumber, List<CsvValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(availableClasses))
        {
            errors.Add(new CsvValidationError(rowNumber, nameof(FlightCsvDto.AvailableClasses), "AvailableClasses is required."));
            return;
        }

        var classEntries = availableClasses.Split(';');
        foreach (var classEntry in classEntries)
        {
            var parts = classEntry.Split(':');
            if (parts.Length != 3)
            {
                errors.Add(new CsvValidationError(rowNumber, nameof(FlightCsvDto.AvailableClasses), $"Invalid format: '{classEntry}'. Expected: ClassType:Seats:Price"));
                continue;
            }

            var (classType, seatsStr, priceStr) = (parts[0], parts[1], parts[2]);

            if (!Enum.TryParse<FlightClass>(classType, out _))
            {
                errors.Add(new(rowNumber, nameof(FlightCsvDto.AvailableClasses), $"Invalid class type: '{classType}'"));
            }

            if (!int.TryParse(seatsStr, out int seats) || seats < 0)
            {
                errors.Add(new(rowNumber, nameof(FlightCsvDto.AvailableClasses), $"Invalid seat count: '{seatsStr}'. Must be non-negative."));
            }

            if (!decimal.TryParse(priceStr, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                errors.Add(new(rowNumber, nameof(FlightCsvDto.AvailableClasses), $"Invalid price: '{priceStr}'. Must be a positive decimal."));
            }
        }
    }
}