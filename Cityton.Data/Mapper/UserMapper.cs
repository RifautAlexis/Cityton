using Cityton.Data.Common;
using Cityton.Data.DTOs;
using Cityton.Data;
using Cityton.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Cityton.Data.Mapper
{
    public static class UserMapper
    {

        public static void DeepCopy(this User user, UserUpdateDTO userToUpdate)
        {
            user.Username = userToUpdate.Username;
            user.PhoneNumber = userToUpdate.PhoneNumber;
            user.Email = userToUpdate.Email;
            user.Picture = userToUpdate.Picture;
            user.Role = userToUpdate.Role;
        }

        public static Role ToRole(this string role)
        {
            Enum.TryParse(role, out Role roleToReturn);
            return roleToReturn;
        }

        public static UserDTO ToDTO(this User data)
        {
            if (data == null) return null;

            return new UserDTO
            {
                Id = data.Id,
                Username = data.Username,
                PhoneNumber = data.PhoneNumber,
                Email = data.Email,
                Picture = data.Picture,
                Role = data.Role,
                Token = data.Token,
                GroupId = data.ParticipantGroups.Where(pg => pg.Status == Status.Accepted).Select(pg => pg.BelongingGroupId).FirstOrDefault()
            };
        }

        public static User ToUser(this UserUpdateDTO data)
        {
            if (data == null) return null;

            return new User
            {
                Id = data.Id,
                Username = data.Username,
                PhoneNumber = data.PhoneNumber,
                Email = data.Email,
                Picture = data.Picture,
                Role = Role.Member
            };
        }

        public static User ToUser(this RegisterDTO data)
        {
            if (data == null) return null;

            return new User
            {
                Username = data.Username,
                PhoneNumber = data.PhoneNumber,
                Email = data.Email,
                Picture = "Default",
                Role = Role.Member,
                PasswordHash = null,
                PasswordSalt = null,
                Token = null,
                CompanyId = 1
            };
        }

        public static User ToUser(this UserDTO data)
        {
            if (data == null) return null;

            return new User
            {
                Id = data.Id,
                Username = data.Username,
                PhoneNumber = data.PhoneNumber,
                Email = data.Email,
                Picture = data.Picture,
                Role = data.Role,
                PasswordHash = null,
                PasswordSalt = null,
                Token = data.Token,
                CompanyId = 1
            };
        }

        public static UserMinimal ToMinimalDTO(this User data)
        {
            return (new UserMinimal {
                Id = data.Id,
                Username = data.Username
            });
        }

        public static List<UserMinimal> ToMinimalDTO(this IEnumerable<User> data)
        {
            return data.Select(u => u.ToMinimalDTO()).ToList();
        }
    }
}