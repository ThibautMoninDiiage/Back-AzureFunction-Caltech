﻿namespace SecurityServer.Service.DTO.Down
{
    public class ApplicationByUserDtoDown
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? RedirectUri { get; set; }
        public RoleByApplicationIdDtoDown? RoleByApplicationIdDtoDown { get; set; }
    }
}
