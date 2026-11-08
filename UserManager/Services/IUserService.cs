using UserManager.DTO;
using UserManager.Models;

namespace UserManager.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid Id);
        Task<UserDTO> CreateAsync(CreateUserDTO user);
        Task<bool> DeleteAsync(Guid id);
    }
}
