﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServer.Service.DTO.Down
{
    public class ApplicationDtoDown
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}