public interface IAuthService
{
    Task Register(RegisterDto registerDto);
    Task<string> Login(LoginDto loginDto);
    Task ChangePassword(changePasswordDto changePasswordDto, string userId);
    Task<string> ForgotPassword(string email);
    Task ResetPassword(ResetPasswordDto resetPasswordDto);
    Task DeleteUser(string email, string userId);
}

public class AuthService : IAuthService
{
    private readonly AuthRepository _authRepository;

    public AuthService(AuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task Register(RegisterDto registerDto)
    {
        await _authRepository.RegisterUser(registerDto);
    }

    public async Task<string> Login(LoginDto loginDto)
    {
        return await _authRepository.Login(loginDto);
    }

    public async Task ChangePassword(changePasswordDto changePasswordDto, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        await _authRepository.ChangePassword(changePasswordDto, userId);
    }

    public async Task<string> ForgotPassword(string email)
    {
        // Gera o token de reset e "envia" (simulação)
        var token = await _authRepository.ForgotPassword(email);

        if(token == null)
        {
            // Para evitar vazamento de informações, não indicamos se o email existe ou não
            return "";
        }

        return token;
    }

    public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        await _authRepository.ResetPassword(resetPasswordDto);
    }

    public async Task DeleteUser(string email, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var requester = await _authRepository.GetUserByIdentityId(userId);
        if (requester?.Email == null || !string.Equals(requester.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("You can only delete your own account");
        }

        await _authRepository.DeleteUser(email);
    }
}