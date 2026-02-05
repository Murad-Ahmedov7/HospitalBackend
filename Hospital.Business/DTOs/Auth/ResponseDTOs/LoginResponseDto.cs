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
        public Guid UserId { get; set; }
        public int ExpiresIn { get; set; }
    }
}
