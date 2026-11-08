
using AutoMapper;
using UserManager.Data;
using UserManager.DTO;
using UserManager.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManager.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UserService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<UserDTO> CreateAsync(CreateUserDTO user)
        {
            var newUser = _mapper.Map<User>(user);

            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<UserDTO>(newUser);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _appDbContext.Users.FindAsync(id);

            if (user == null || user.IsDeleted)
                return false;

            user.IsDeleted = true;

            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid id)
        {
            var user = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return null;

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            var users = await _appDbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}
