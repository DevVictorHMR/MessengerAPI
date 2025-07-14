using MessengerAPI.DTOs;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(UserDto userDto);
}
