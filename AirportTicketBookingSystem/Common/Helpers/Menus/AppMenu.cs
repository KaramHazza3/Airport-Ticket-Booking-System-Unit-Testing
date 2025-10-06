using AirportTicketBookingSystem.Common.Helpers.Menus.Constants;
using AirportTicketBookingSystem.Models.Enums;
using AirportTicketBookingSystem.Services.AuthService;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Services.ImportFileService;

namespace AirportTicketBookingSystem.Common.Helpers.Menus;

public class AppMenu
{
    private readonly IAuthService _authService;
    private readonly IFileImportService _importService;
    private readonly PassengerMenu _passengerMenu;
    private readonly ManagerMenu _managerMenu;


    public AppMenu(IAuthService authService, IBookingService<Guid> bookingService , IFlightService<Guid> flightService, IFileImportService importService)
    {
        _authService = authService;
        _importService = importService;
        _passengerMenu = new PassengerMenu(bookingService, flightService);
        _managerMenu = new ManagerMenu(bookingService, importService, flightService);
    }
    
    public async Task Handle()
    {
        while (true)
        {
            Show();
            var input = Console.ReadLine();

            switch (input)
            {
                case AppMenuConstants.Login:
                    await HandleLogin();
                    break;
                case AppMenuConstants.Register:
                    await HandleRegistration();
                    break;
                case AppMenuConstants.Exit:
                    Exit();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }
    
    public static void Exit() => Environment.Exit(0);
    
    private async Task HandleLogin()
    {
        Console.WriteLine("Login");
        Console.WriteLine("Enter your email");
        var emailToLogin = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter your password");
        var passwordToLogin = Console.ReadLine() ?? string.Empty;
        var loginResult = await _authService.LoginAsync(emailToLogin, passwordToLogin);

        if (loginResult.IsFailure)
        {
            Console.WriteLine(loginResult.Error);
            return;
        }

        var user = loginResult.Value;

        if (user.Role == UserRole.Passenger)
        {
            await _passengerMenu.Handle(user);
        }
        else if (user.Role == UserRole.Manager)
        {
            await _managerMenu.Handle();
        }
    }
    
    private async Task HandleRegistration()
    {
        Console.WriteLine("Register");
        
        Console.WriteLine("Enter your name");
        var nameToRegister = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter your email");
        var emailToRegister = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter your password");
        var passwordToRegister = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Select your role");
        Console.WriteLine("1. Passenger");
        Console.WriteLine("2. Manager");
        var roleInput = Console.ReadLine();

        if (!int.TryParse(
                roleInput,
                out var roleSelection) || roleSelection < 1 || roleSelection > 2)
        {
            Console.WriteLine("Invalid role selection");
            return;
        }

        var role = roleSelection switch
        {
            1 => UserRole.Passenger,
            2 => UserRole.Manager,
            _ => UserRole.Passenger
        };

        var registerResult = await _authService.RegisterAsync(nameToRegister,emailToRegister, passwordToRegister, role);

        if (registerResult.IsFailure)
        {
            Console.WriteLine(registerResult.Error);
            return;
        }

        Console.WriteLine("Registration successful");
    }
    
    private static void Show()
    {
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Exit");
    }
}