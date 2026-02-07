

using Azure.Core;
using Hospital.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital.Business.DTOs.Auth.RequestDTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(2)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;


    }
}


