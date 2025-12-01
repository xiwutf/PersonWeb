using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Api.Models.Dto;

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "新密码长度至少6位字符")]
    public string NewPassword { get; set; } = string.Empty;
}

