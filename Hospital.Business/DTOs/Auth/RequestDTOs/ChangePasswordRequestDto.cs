
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Hospital.Business.DTOs.Auth.RequestDTOs
{
    public class ChangePasswordRequestDto
    {
        [Required]
        [MinLength(8)]
        public string CurrentPassword { get; set; } = null!;


        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = null!;
    }
}






