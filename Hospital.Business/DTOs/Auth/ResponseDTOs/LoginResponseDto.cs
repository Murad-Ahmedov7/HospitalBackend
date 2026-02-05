using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Business.DTOs.Auth.ResponseDTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        //public int? UserId { get; set; }         // optional
        //public int? ExpiresIn { get; set; }      // optional
        //public string? Role { get; set; }        // optional
        //public string? RefreshToken { get; set; } // optional
    }
}
