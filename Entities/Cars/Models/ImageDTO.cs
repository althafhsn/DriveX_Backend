﻿namespace DriveX_Backend.Entities.Cars.Models
{
    public class ImageDTO
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
    }

    public class ImageRequestDTO
    {
        public string ImagePath { get; set; }
    }
}
