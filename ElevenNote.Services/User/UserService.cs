using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Models.User;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ElevenNote.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // public Task<bool> RegisterUserAsync(UserRegister model)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<bool> RegisterUserAsync(UserRegister model)
        {
            if(await GetUserByEmailAsync(model.Email) != null || await GetUserByUsernameAsync(model.UserName) != null)
            return false;
            var entity = new UserEntity
            {
                Email = model.Email,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateCreated = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<UserEntity>();
            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _context.Users.Add(entity);
            var numberOfChanges = await _context.SaveChangesAsync();
            
            return numberOfChanges == 1;
        }

        public async Task<UserDetail> GetUserByIdAsync(int userId)
        {
            var entity = await _context.Users.FindAsync(userId);
            if (entity is null)
            return null;

            var userDetail = new UserDetail
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateCreated = entity.DateCreated
            };
            return userDetail;
        }

        private async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
        }

        private async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == username.ToLower());
        }

    }
}