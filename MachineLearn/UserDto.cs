using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearn
{
    public class UserDto
    {
        public int UserId { get; set; }        // 对应 User.Id（名字不同）
        public string FullName { get; set; } = string.Empty; // 由 FirstName + LastName 合成
        public string Email { get; set; } = string.Empty;    // 同名，自动映射
        public DateTime CreatedAt { get; set; }              // 同名，自动映射
        public bool IsNew { get; set; }                      // 由 AfterMapping 赋值
    }
}
