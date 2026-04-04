public class changePasswordDto
{
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmNewPassword { get; set; } = default!;
}