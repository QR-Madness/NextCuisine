﻿using NextCuisine.Data;

namespace NextCuisine.Models
{
    public class GuestUploadFile
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string Filename { get; set; } = String.Empty;
        public DateTime UploadDateTime { get; set; } = DateTime.Now;
    }
}