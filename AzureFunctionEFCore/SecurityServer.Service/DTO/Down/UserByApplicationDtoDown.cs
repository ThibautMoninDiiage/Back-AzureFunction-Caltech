using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServer.Service.DTO.Down
{
    public class UserByApplicationDtoDown
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Mail { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public RoleByApplicationIdDtoDown? Role { get; set; }
    }
}
