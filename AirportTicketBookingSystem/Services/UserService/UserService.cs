using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;

namespace AirportTicketBookingSystem.Services.UserService;

public class UserService : IUserService<Guid>
{
    private readonly IRepository _repository;
    private readonly List<User> _users = [];
    public UserService(IRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result<ICollection<User>>> GetAllUsersAsync()
    {
        return await GetUsers();
    }

    public async Task<Result<User>> AddUserAsync(User user)
    {
        var users = await GetUsers();
        var isExist = users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));
        if (isExist)
        {
            return UserErrors.AlreadyExists;
        }
        users.Add(user);
        await this._repository.WriteAsync(users);
        _users.Clear();
        _users.AddRange(users);
        return user;
    }

    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        var users = await GetUsers();
        var user = users.SingleOrDefault(b => b.Id == userId);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        users.Remove(user);
        await this._repository.WriteAsync(users);
        _users.Clear();
        _users.AddRange(users);
        return Result.Success();
    }

    public async Task<Result<User>> ModifyUserAsync(Guid id, User user)
    {
        var users = await GetUsers();
        var isExist = users.Exists(b => b.Id == id);
        if (!isExist)
        {
            return UserErrors.NotFound;
        }
        var index = users.FindIndex(item => item.Id.Equals(user.Id));
        users[index] = user;
        await this._repository.WriteAsync(users);
        _users.Clear();
        _users.AddRange(users);
        return user;
    }
    
    private async Task<List<User>> GetUsers()
    {
        if (_users.Count > 0)
        {
            return _users;
        }
        var users = await this._repository.ReadAsync<User>();
        _users.Clear();
        _users.AddRange(users);
        return users.ToList();
    }

    public async Task<Result<User>> AddAsync(User data)
    {
        return await AddUserAsync(data);
    }
}