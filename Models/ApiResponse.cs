﻿namespace CurrencyConverterAPI.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Data { get; set; }
    }

}
