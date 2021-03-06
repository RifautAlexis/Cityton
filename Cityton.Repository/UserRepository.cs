﻿using Cityton.Data;
using Cityton.Data.Common;
using Cityton.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cityton.Data.Mapper;

namespace Cityton.Repository
{

    public interface IUserRepository : IRepository<User>
    {

        Task<User> GetWithRequests(int id);
        Task<User> GetByEmail(string email);
        Task<List<User>> GetAllByEmail(string email);
        Task<User> GetByUsername(string username);
        Task<List<User>> GetAllByUsername(string email);
        Task<User> GetByPhoneNumber(string phoneNumber);
        Task<List<User>> GetAllByPhoneNumber(string email);
        Task<List<User>> GetByUsernameRole(string username, string filterRole);
        Task<List<User>> GetUsersWithoutGroup();
        Task<User> Get_Challenges_Achievements(int userId);
        Task<User> Get_Achievements(int userId);

    }

    public class UserRepository : Repository<User>, IUserRepository
    {

        public UserRepository(ApplicationContext context) : base(context) { }

        public async Task<User> GetByEmail(string email)
        {
            return await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetWithRequests(int id)
        {
            return await context.Users.Where(u => u.Id == id)
                .Include(u => u.ParticipantGroups)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllByEmail(string email)
        {
            return await context.Users.Where(u => u.Email == email).ToListAsync();
        }

        public async Task<User> GetByUsername(string username)
        {
            return await context.Users.Where(user => user.Username == username).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllByUsername(string username)
        {
            return await context.Users.Where(u => u.Username == username).ToListAsync();
        }

        public async Task<User> GetByPhoneNumber(string phoneNumber)
        {
            return await context.Users.Where(u => u.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllByPhoneNumber(string phoneNumber)
        {
            return await context.Users.Where(u => u.PhoneNumber == phoneNumber).ToListAsync();
        }

        public async Task<List<User>> GetByUsernameRole(string filterRole, string username = "")
        {
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;

            if (!filterRole.Equals("All"))
            {
                Role role = filterRole.ToRole();
                return await context.Users.Where(u => (u.Username.Contains(username, comparison)) && (u.Role == role)).ToListAsync();
            }

            return await context.Users.Where(user => user.Username.Contains(username, comparison)).ToListAsync();
        }

        public async Task<List<User>> GetUsersWithoutGroup()
        {

            return await context.Users.Where(user => user.Role == Role.Member && (user.ParticipantGroups.Count() == 0 || !user.ParticipantGroups.Any(pg => pg.Status == Status.Accepted))).ToListAsync();
        }

        public async Task<User> Get_Challenges_Achievements(int userId)
        {
            return await context.Users
                .Where(user => user.Id == userId)
                .Include(user => user.Challenges)
                .Include(user => user.Achievements)
                .FirstOrDefaultAsync();
        }

        public async Task<User> Get_Achievements(int userId)
        {
            return await context.Users
                .Where(user => user.Id == userId)
                .Include(user => user.Achievements)
                .FirstOrDefaultAsync();
        }

    }
}