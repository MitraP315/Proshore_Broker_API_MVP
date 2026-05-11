using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProshoreHouseBroker.Application.DTOs
{
    public class AdminChangePasswordDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
