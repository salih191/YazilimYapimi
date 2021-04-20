﻿using Core.Entities;

namespace Entities.DTOs
{
    public class UserForChangePasswordDto : IDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}