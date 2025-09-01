using AirportTicketBookingSystem.Common.Helpers.Menus.Constants;
using AirportTicketBookingSystem.Common.Validators.CsvValidators.Flight;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.DTOs;
using AirportTicketBookingSystem.Models.Mappers;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Services.ImportFileService;

namespace AirportTicketBookingSystem.Common.Helpers.Menus;

public class ManagerMenu
{
    private readonly IBookingService<Guid> _bookingService;
    private readonly IFileImportService _importService;
    private readonly IFlightService<Guid> _flightService;


    public ManagerMenu(IBookingService<Guid> bookingService, IFileImportService importService, IFlightService<Guid> flightService)
    {
        _bookingService = bookingService;
        _importService = importService;
        _flightService = flightService;
    }
    
    public async Task Handle()
    {
        while (true)
        {
            ShowMenu();
            var managerInput = Console.ReadLine();

            switch (managerInput)
            {
                case ManagerMenuConstants.ImportFlightsCsv:
                    await ImportFlightsFromCSV();
                    break;
                case ManagerMenuConstants.GetSpecificClassConstraints:
                    GetSpecificClassConstraints();
                    break;
                case ManagerMenuConstants.FilterBookings:
                    await FilterBookings();
                    break;
                case ManagerMenuConstants.Exit:
                    AppMenu.Exit();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }

    private void GetSpecificClassConstraints()
    {
        ShowAvailableClassesMenu();
        Console.Write("Enter the class name: ");
        var className = Console.ReadLine();
        if (!ValidateRequiredString(className, nameof(className))) return;

        switch (className)
        {
            case ManagerMenuConstants.GetUserConstraints:
                foreach (var attributeConstraint in AttributeConstraintsGenerator.GetValidationConstraints<User>())
                {
                    Console.WriteLine(attributeConstraint);
                }
                break;
            case ManagerMenuConstants.GetFlightConstraints:
                foreach (var attributeConstraint in AttributeConstraintsGenerator.GetValidationConstraints<Flight>())
                {
                    Console.WriteLine(attributeConstraint);
                }
                break;
            case ManagerMenuConstants.GetBookingConstraints:
                foreach (var attributeConstraint in AttributeConstraintsGenerator.GetValidationConstraints<Booking>())
                {
                    Console.WriteLine(attributeConstraint);
                }
                break;
            case ManagerMenuConstants.Exit:
                AppMenu.Exit();
                break;
            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }

    private async Task FilterBookings()
    {
      
        ShowFilterBookingMenu();
        Console.WriteLine("Enter filter strategy: ");
        var filterInput = Console.ReadLine();
        if (!ValidateRequiredString(filterInput, nameof(filterInput))) return;
       
        switch (filterInput)
        {
            case ManagerMenuConstants.FilterByFlight:
                Console.Write("Enter flight id: ");
                var flightId = GetGuid(Console.ReadLine()!);
                if (!ValidateGuid(flightId)) return;
                await FilterBooking(b => b.Flight.Id == flightId);
                break;
            case ManagerMenuConstants.FilterByUser:
                Console.Write("Enter user id: ");
                var userId = GetGuid(Console.ReadLine()!);
                if (!ValidateGuid(userId)) return;
                await FilterBooking(b => b.Passenger.Id == userId);
                break;
            case ManagerMenuConstants.FilterByDepartureCountry:
                Console.Write("Enter country name: ");
                var departureCountry = Console.ReadLine();
                if (!ValidateRequiredString(departureCountry, nameof(departureCountry))) return;
                await FilterBooking(b => CompareStringsWithoutCaseSensitive(b.Flight.Departure.Name, departureCountry!));
                break;
            case ManagerMenuConstants.FilterByDestinationCountry:
                Console.Write("Enter the country name: ");
                var destinationCountry = Console.ReadLine();
                if (!ValidateRequiredString(destinationCountry, nameof(destinationCountry))) return;
                await FilterBooking(b => CompareStringsWithoutCaseSensitive(b.Flight.Destination.Name, destinationCountry!));
                break;
            case ManagerMenuConstants.FilterByDepartureAirport:
                Console.Write("Enter the airport name: ");
                var departureAirport = Console.ReadLine();
                if (!ValidateRequiredString(departureAirport, nameof(departureAirport))) return;
                await FilterBooking(b => CompareStringsWithoutCaseSensitive(b.Flight.DepartureAirport.Name, departureAirport!));
                break;
            case ManagerMenuConstants.FilterByDestinationAirport:
                Console.Write("Enter the airport name: ");
                var destinationAirport = Console.ReadLine();
                if (!ValidateRequiredString(destinationAirport, nameof(destinationAirport))) return;
                await FilterBooking(b => CompareStringsWithoutCaseSensitive(b.Flight.ArrivalAirport.Name, destinationAirport!));
                break;
            case ManagerMenuConstants.FilterByClass:
                Console.Write("Enter the flight class name: ");
                var flightClassNameInput = Console.ReadLine();
                if (!ValidateRequiredString(flightClassNameInput, nameof(flightClassNameInput))) return;
                await FilterBooking(b => CompareStringsWithoutCaseSensitive(b.FlightClass.ToString(), flightClassNameInput!));
                break;
            case ManagerMenuConstants.FilterExit:
                AppMenu.Exit();
                break;
            default:
                Console.WriteLine("Invalid Input");
                break;
        }
    }

    private async Task ImportFlightsFromCSV()
    {
        Console.WriteLine("Import from CSV");
        Console.Write("Enter your file path: ");
        Console.WriteLine("Note: you can take a look for sample file in the Project folder");


        var filePath = Console.ReadLine();
        if (!ValidateRequiredString(filePath, nameof(filePath))) return;
        var csvResult = await _importService.ImportFileAsync<Flight, FlightCsvDto, Guid>(filePath!, FlightMapper.FromCsvDto,
            _flightService, FlightCsvValidator.Validate);

        if (csvResult.IsFailure)
        {
            Console.WriteLine(csvResult.Error);
            return;
        }

        Console.WriteLine("Import successful");
    }
    
    private async Task FilterBooking(Func<Booking, bool> predicate)
    {
        var result = await _bookingService.FilterAsync(predicate);
        if (result.IsFailure)
        {
            Console.WriteLine(result.Error.Description);
            return;
        }

        var bookings = result.Value;
        if (!bookings.Any())
        {
            Console.WriteLine("No flights found matching your criteria.");
            return;
        }

        foreach (var booking in bookings)
        {
            Console.WriteLine(booking);
        }
    }
    
    private static void ShowMenu()
    {
        Console.WriteLine("1. Import from CSV");
        Console.WriteLine("2. View Validations Rules");
        Console.WriteLine("3. Filter Bookings");
        Console.WriteLine("4. Exit");
    }

    private static void ShowFilterBookingMenu()
    {
        Console.WriteLine("Filter Bookings");
        Console.WriteLine("1. By Flight");
        Console.WriteLine("2. By Passenger");
        Console.WriteLine("3. By Departure Country");
        Console.WriteLine("4. By Destination Country");
        Console.WriteLine("5. By Departure Airport");
        Console.WriteLine("6. By Destination Airport");
        Console.WriteLine("7. By Flight Class Type");
        Console.WriteLine("8. Exit");
    }
    
    private static void ShowAvailableClassesMenu()
    {
        Console.WriteLine("Validation rules of available classes");
        Console.WriteLine("1. User");
        Console.WriteLine("2. Flight");
        Console.WriteLine("3. Booking");
    }

    private static Guid GetGuid(string guid)
    {
        return Guid.TryParse(guid, out var userId) ? userId : Guid.Empty;
    }

    private static bool ValidateGuid(Guid id)
    {
        if (id != Guid.Empty)
        {
            return true;
        }
        Console.WriteLine("Invalid input");
        return false;
    }
    
    private static bool ValidateRequiredString(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine($"{fieldName} is invalid.");
            return false;
        }
        return true;
    }
    private static bool CompareStringsWithoutCaseSensitive(string source, string to)
    {
        return String.Equals(source, to, StringComparison.OrdinalIgnoreCase);
    }
}