﻿using Cityton.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cityton.Data.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public Role Role { get; set; } = Common.Role.Member;
        public string Token { get; set; }
        public int GroupId { get; set; }

    }
}
