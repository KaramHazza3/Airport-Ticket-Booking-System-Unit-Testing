using AirportTicketBookingSystem.Common.Helpers.Menus;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.AuthService;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Services.ImportFileService;
using AirportTicketBookingSystem.Services.UserService;
using AirportTicketBookingSystem.Wrappers;
using CsvHelper;

namespace AirportTicketBookingSystem;

class Program
{
    static void Main(string[] args)
    {
        IRepository fileRepo = FileRepository.Instance;
        var importService = new ImportCsvFileService(new CSVReader());
        var userService = new UserService(fileRepo);
        var flightService = new FlightService(fileRepo);
        var bookingService = new BookingService(fileRepo);
        var authService = new AuthService(userService);
        var appMenu = new AppMenu(authService, bookingService, flightService, importService);
        appMenu.Handle().Wait();
    }
}
