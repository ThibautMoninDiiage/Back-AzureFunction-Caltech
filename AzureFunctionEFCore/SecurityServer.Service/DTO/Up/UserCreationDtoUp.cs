﻿using SecurityServer.Service.DTO.Down;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecurityServer.Service.DTO.Up
{
    public class UserCreationDtoUp
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        public RoleUserDtoDown Role { get; set; }
    }
}
