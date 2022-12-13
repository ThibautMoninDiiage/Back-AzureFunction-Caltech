﻿using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Models.Models
{
    public class Application : BaseModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        public string? Description { get; set; }
        public List<User>? Users { get; set; }
    }
}