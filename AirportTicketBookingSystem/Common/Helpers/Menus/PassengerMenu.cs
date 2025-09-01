using AirportTicketBookingSystem.Common.Helpers.Menus.Constants;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Services.FlightService;

namespace AirportTicketBookingSystem.Common.Helpers.Menus;

public class PassengerMenu
{
    private readonly IBookingService<Guid> _bookingService;
    private readonly IFlightService<Guid> _flightService;

    public PassengerMenu(IBookingService<Guid> bookingService, IFlightService<Guid> flightService)
    {
        _bookingService = bookingService;
        _flightService = flightService;
    }
    public async Task Handle(User user)
    {
        while (true)
        {
            Show();
            var passengerInput = Console.ReadLine();

            switch (passengerInput)
            {
                case PassengerMenuConstants.SearchForAvailableFlights:
                    await SearchForAvailableFlights();
                    break;
                case PassengerMenuConstants.BookFlight:
                    await BookFlight(user);
                    break;
                case PassengerMenuConstants.CancelBooking:
                    await CancelBooking(user);
                    break;
                case PassengerMenuConstants.ViewBookings:
                    await ViewBookings(user);
                    break;
                case PassengerMenuConstants.EditBooking:
                    await EditBooking(user);
                    break;
                case PassengerMenuConstants.Exit:
                    AppMenu.Exit();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }

    private async Task SearchForAvailableFlights()
    {
        while (true)
        {
            ShowSearchParameters();
            var passengerSearchInput = Console.ReadLine();
            switch (passengerSearchInput)
            {
                case PassengerMenuConstants.SearchByPrice:
                    Console.Write("Enter the price: ");
                    var price = GetPrice(Console.ReadLine()!);
                    if (isNull(price)) return;
                    await FilterFlights(f => f.BasePrice == price);
                    break;
                case PassengerMenuConstants.SearchByDepartureCountry:
                    Console.Write("Enter the country: ");
                    var departureCountryName = Console.ReadLine();
                    if (!ValidateRequiredString(departureCountryName, nameof(departureCountryName))) return;
                    await FilterFlights(f => f.Departure.Name == departureCountryName);
                    break;
                case PassengerMenuConstants.SearchByDestinationCountry:
                    Console.Write("Enter the country: ");
                    var destinationCountryName = Console.ReadLine();
                    if (!ValidateRequiredString(destinationCountryName, nameof(destinationCountryName))) return;
                    await FilterFlights(f => f.Destination.Name == destinationCountryName);
                    break;
                case PassengerMenuConstants.SearchByDepartureAirport:
                    Console.Write("Enter the airport: ");
                    var departureAirportName = Console.ReadLine();
                    if (!ValidateRequiredString(departureAirportName, nameof(departureAirportName))) return;
                    await FilterFlights(f => f.DepartureAirport.Name == departureAirportName);
                    break;
                case PassengerMenuConstants.SearchByArrivalAirport:
                    Console.Write("Enter the airport: ");
                    var arrivalAirportName = Console.ReadLine();
                    if (!ValidateRequiredString(arrivalAirportName, nameof(arrivalAirportName))) return;
                    await FilterFlights(f => f.ArrivalAirport.Name == arrivalAirportName);
                    break;
                case PassengerMenuConstants.SearchByClass:
                    Console.Write("Enter the class: ");
                    var className = Console.ReadLine();
                    if (!ValidateRequiredString(className, nameof(className))) return;
                    if (Enum.TryParse<FlightClass>(className, out var parsedClass))
                    {
                        await FilterFlights(f => f.AvailableClasses.Exists(ac => ac.ClassType == parsedClass));
                        return;
                    }

                    Console.WriteLine("Invalid class type");
                    break;
                case PassengerMenuConstants.ExitSearch:
                    AppMenu.Exit();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }

        }
    }
    private async Task FilterFlights(Func<Flight, bool> predicate)
    {
        var result = await _flightService.SearchFlight(predicate);
        if (result.IsFailure)
        {
            Console.WriteLine(result.Error.Description);
            return;
        }

        var flights = result.Value;
        if (!flights.Any())
        {
            Console.WriteLine("No flights found matching your criteria.");
            return;
        }

        DisplayFlights(flights);
    }
    
    private static void DisplayFlights(List<Flight> flights)
    {
        foreach (var flight in flights)
        {
            Console.WriteLine(flight);
            Console.WriteLine("**********************");
        }
    }

    private async Task EditBooking(User user)
    {
        var myBookingsResult = await this._bookingService.FilterAsync(b => b.Passenger.Id == user.Id);
        if (myBookingsResult.IsFailure)
        {
            Console.WriteLine(myBookingsResult.Error.Description);
            return;
        }

        var myBookings = myBookingsResult.Value;
        Console.WriteLine("Edit my booking");
        Console.Write("Enter booking id: ");

        if (!Guid.TryParse(Console.ReadLine(), out var bookingId) || !myBookings.Exists(b => b.Id == bookingId))
        {
            Console.WriteLine("Invalid booking id");
            return;
        }

        var bookingToUpdate = myBookings.SingleOrDefault(b => b.Id == bookingId);
        ShowUpdateBookingParameters();
        Console.Write("Enter what you want to update: ");
        var input = Console.ReadLine();
        switch (input)
        {
            case PassengerMenuConstants.UpdateClass:
                await UpdateFlightClass(bookingToUpdate!);
                break;
            default:
                Console.WriteLine("Invalid Input");
                break;
        }

        await UpdateStorage(bookingId, bookingToUpdate);
    }

    private async Task UpdateStorage(Guid bookingId, Booking? bookingToUpdate)
    {
        var updateResult = await _bookingService.ModifyBookingAsync(bookingId, bookingToUpdate!);

        if (updateResult.IsFailure)
        {
            Console.WriteLine(updateResult.Error);
            return;
        }

        Console.WriteLine("Booking updated successfully");
    }

    private static void ShowUpdateBookingParameters()
    {
        Console.WriteLine("1. Flight Class");
    }
    
    private async Task BookFlight(User user)
    {
        Console.WriteLine("Book a flight");

        var flightsResult = await this._flightService.GetAllFlightsAsync();
        if (flightsResult.IsFailure)
        {
            Console.WriteLine(flightsResult.Error.Description);
            return;
        }

        var flights = flightsResult.Value.ToList();
        DisplayFlights(flights);
        
        Console.Write("Enter flight id: ");
        if (!Guid.TryParse(Console.ReadLine(), out var flightId) || !flights.Exists(f => f.Id == flightId))
        {
            Console.WriteLine("Invalid flight id");
            return;
        }

        ShowFlightClassesMenu(); 

        if (!int.TryParse(Console.ReadLine(), out var classInput) || classInput < 1 || classInput > 3)
        {
            Console.WriteLine("Invalid class selection");
            return;
        }

        var flightClass = classInput switch
        {
            1 => FlightClass.Economy,
            2 => FlightClass.Business,
            3 => FlightClass.FirstClass,
            _ => FlightClass.Economy
        };
        var flight = flights.SingleOrDefault(f => f.Id == flightId);
        if (!flight!.AvailableClasses.Exists(ac => ac.ClassType == flightClass))
        {
            Console.WriteLine("The flight doesn't contain this class");
            return;
        }

        if (flight.AvailableClasses.Find(ac => ac.ClassType == flightClass)!.AvailableSeats <= 0)
        {
            Console.WriteLine("There's no available seats for this class");
        }
        var booking = new Booking(user, flight!, flightClass);
        var bookingResult = await _bookingService.AddBookingAsync(booking);

        if (bookingResult.IsFailure)
        {
            Console.WriteLine(bookingResult.Error);
            return;
        }

        await HandleSeatsAsync(flightClass, flightId, -1);
        Console.WriteLine("Booking successful");
    }
    
    private async Task CancelBooking(User user)
    {
        Console.WriteLine("Cancel my booking");

        var myBookingsResult = await this._bookingService.FilterAsync(b => b.Passenger.Id == user.Id);
        if (myBookingsResult.IsFailure)
        {
            Console.WriteLine(myBookingsResult.Error.Description);
            return;
        }

        var myBookings = myBookingsResult.Value;

        Console.Write("Enter booking id: ");

        if (!Guid.TryParse(Console.ReadLine(), out var bookingId) || !myBookings.Exists(b => b.Id == bookingId))
        {
            Console.WriteLine("Invalid booking id");
            return;
        }

        var bookingToDelete = myBookings.SingleOrDefault(b => b.Id == bookingId);
        var deleteResult = await _bookingService.CancelBookingAsync(bookingToDelete!.Id);

        if (deleteResult.IsFailure)
        {
            Console.WriteLine(deleteResult.Error);
            return;
        }

        await HandleSeatsAsync(bookingToDelete.FlightClass, bookingToDelete.Flight.Id, 1);
        Console.WriteLine("Booking cancelled");
    }
    
     private async Task UpdateFlightClass(Booking bookingToUpdate)
    {
        ShowFlightClassesMenu();
        var flightClassinput = Console.ReadLine();
        switch (flightClassinput)
        {
            case PassengerMenuConstants.UpdateToEconomy:
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, 1);
                bookingToUpdate!.FlightClass = FlightClass.Economy;
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, -1);
                break;
            case PassengerMenuConstants.UpdateToBusiness:
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, 1);
                bookingToUpdate!.FlightClass = FlightClass.Business;
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, -1);
                break;
            case PassengerMenuConstants.UpdateToFirstClass:
                if (!bookingToUpdate!.Flight.AvailableClasses.Exists(ac =>
                        ac.ClassType == FlightClass.FirstClass))
                {
                    Console.WriteLine("The flight doesn't have first class");
                    return;
                }
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, 1);
                bookingToUpdate!.FlightClass = FlightClass.FirstClass;
                await HandleSeatsAsync(bookingToUpdate.FlightClass, bookingToUpdate.Flight.Id, -1);
                break;
            default:
                Console.WriteLine("Invalid Input");
                return;
        }
    }
    
    private async Task HandleSeatsAsync(FlightClass flightClass, Guid flightId, int seatChange)
    {
        var flightResult = await _flightService.GetFlightById(flightId);
        if (flightResult.IsFailure)
        {
            Console.WriteLine("The flight not found");   
        }

        var flight = flightResult.Value;
        var targetFlightClass = flight.AvailableClasses
            .Find(ac => ac.ClassType == flightClass);

        if (targetFlightClass is null)
            throw new InvalidOperationException($"Flight class {flightClass} not found.");
        targetFlightClass.AvailableSeats += seatChange;

        await _flightService.ModifyFlightAsync(flight.Id, flight);
    }
    private async Task ViewBookings(User user)
    {
        Console.WriteLine("View my bookings");
        var myBookingsResult = await this._bookingService.FilterAsync(b => b.Passenger.Id == user.Id);
        if (myBookingsResult.IsFailure)
        {
            Console.WriteLine(myBookingsResult.Error.Description);
            return;
        }

        var myBookings = myBookingsResult.Value;
        foreach (var booking in myBookings)
        {
            Console.WriteLine(booking);
        }
    }
    
    private static void Show()
    {
        Console.WriteLine("1. Search for available flights");
        Console.WriteLine("2. Book a flight");
        Console.WriteLine("3. Cancel a booking");
        Console.WriteLine("4. View my bookings");
        Console.WriteLine("5. Edit my booking");
        Console.WriteLine("6. Exit");
    }
    
    private static void ShowFlightClassesMenu()
    {
        Console.WriteLine("Select Class");
        Console.WriteLine("1. Economy");
        Console.WriteLine("2. Business");
        Console.WriteLine("3. First Class");
    }

    private static void ShowSearchParameters()
    {
        Console.WriteLine("1. By Price");
        Console.WriteLine("2. By Departure Country");
        Console.WriteLine("3. By Destination Country");
        Console.WriteLine("4. By Departure Airport");
        Console.WriteLine("5. By Arrival Airport");
        Console.WriteLine("6. By Class");
        Console.WriteLine("7. Exit");
    }

    private static decimal? GetPrice(string price)
    {
        if (!decimal.TryParse(Console.ReadLine(), out var validatedPrice))
        {
            Console.WriteLine("Invalid input");
            return null;
        }

        return validatedPrice;
    }

    private static bool isNull(object? input)
    {
        if (input == null)
        {
            Console.WriteLine("Invalid input");
            return true;
        }

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
}