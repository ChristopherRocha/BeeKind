public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> GetUserById(int id);
    Task CreateUser(UserDto userDto);
    Task UpdateUser(int id, UserDto userDto);
    Task DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
        => await _userRepository.GetUsers();

    public async Task<User> GetUserById(int id)
        => await _userRepository.GetUserById(id);

    public async Task CreateUser(UserDto userDto)
        => await _userRepository.AddUser(userDto);

    public async Task UpdateUser(int id, UserDto userDto)
        => await _userRepository.UpdateUser(id, userDto);

    public async Task DeleteUser(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");
        await _userRepository.DeleteUser(id);
    }
}