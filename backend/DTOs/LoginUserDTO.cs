using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace To_Do_Project.DTOs
{
    public class LoginUserDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}