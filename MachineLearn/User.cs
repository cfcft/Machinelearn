using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearn
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // 敏感字段
        public DateTime CreatedAt { get; set; }
    }
}
