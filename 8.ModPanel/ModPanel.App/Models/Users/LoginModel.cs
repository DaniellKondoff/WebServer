﻿using ModPanel.App.Infrastructure.Validation.Users;


namespace ModPanel.App.Models.Users
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}