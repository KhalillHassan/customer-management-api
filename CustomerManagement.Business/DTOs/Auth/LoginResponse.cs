using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagement.Business.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

    }
}
