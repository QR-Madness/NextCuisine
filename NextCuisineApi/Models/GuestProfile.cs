﻿using System.Collections.ObjectModel;
using NextCuisineApi.Tools;

namespace NextCuisineApi.Models
{
    public class GuestProfile
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string FavoriteRecipes { get; set; } = string.Empty;
        public string FavoriteSnacks { get; set; } = string.Empty;
        public string GoodCombos { get; set; } = string.Empty;
        //public Dictionary<string, string> AdditionalContent { get; set; } = new Dictionary<string, string>();
    }
}
