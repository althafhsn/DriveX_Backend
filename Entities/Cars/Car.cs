﻿namespace DriveX_Backend.Entities.Cars
{
    public class Car
    {
        public Guid Id { get; set; }
        public List<CarImage> Images { get; set; }
        public Guid BrandId { get; set; }
        public string RegNo { get; set; }
        public Brand Brand { get; set; }
        public Guid ModelId { get; set; }
        public Model Model { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerHour { get; set; }

    }

}
