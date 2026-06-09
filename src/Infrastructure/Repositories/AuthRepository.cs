using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AuthRepository
{
    private readonly UserManager<IdentityUser> _context;
    private readonly AppDbContext _appDbContext;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthRepository(
        UserManager<IdentityUser> context,
        AppDbContext appDbContext,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _appDbContext = appDbContext;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task RegisterUser(RegisterDto registerDto)
    {
        var user = new IdentityUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            PhoneNumber = registerDto.phoneNumber
        };

        var result = await _context.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Adiciona o usuário à role 'User'
        await _context.AddToRoleAsync(user, "User");

        try
        {
            await EnsureDomainUserLinkedAsync(user, registerDto.Name);
        }
        catch
        {
            await _context.DeleteAsync(user);
            throw;
        }
    }

    public async Task ChangePassword(changePasswordDto changePasswordDto, string identityUserId)
    {
        var user = await _context.FindByIdAsync(identityUserId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.Equals(user.Email, changePasswordDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("You can only change your own password");
        }

        var result = await _context.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _context.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var result = await _context.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<string> ForgotPassword(string email)
    {
        var user = await _context.FindByEmailAsync(email);
        if (user == null)
        {
            // Para evitar vazamento de informações, não indicamos se o email existe ou não
            return string.Empty;
        }

        var token = await _context.GeneratePasswordResetTokenAsync(user);


        var body = $"""
        Ola,

        Recebemos uma solicitacao para redefinir sua senha no BeeKind.

        Use este token no endpoint POST /api/Auth/reset-password:

        Email: {user.Email ?? email}
        Token: {token}

        Este token expira em 30 minutos.
        Se voce nao solicitou a redefinicao de senha, ignore este email.
        """;

        await _emailService.SendEmailAsync(user.Email ?? email, "Redefinicao de senha - BeeKind", body);


        return string.Empty; // O token e enviado por email, nao e retornado pela API
    }

    public async Task DeleteUser(string email)
    {
        var user = await _context.FindByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var result = await _context.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var domainUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);
        if (domainUser != null)
        {
            _appDbContext.Users.Remove(domainUser);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task<IdentityUser?> GetUserByIdentityId(string identityUserId)
    {
        return await _context.FindByIdAsync(identityUserId);
    }

    public async Task<string> Login(LoginDto loginDto)
    {
        var user = await _context.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var passwordValid = await _context.CheckPasswordAsync(user, loginDto.Password);
        if (!passwordValid)
        {
            throw new UnauthorizedAccessException("Invalid password");
        }

        var appUser = await EnsureDomainUserLinkedAsync(user, user.UserName);

        // Geração do JWT
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "BeeKindIssuer";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "BeeKindAudience";

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName ?? ""),
            new System.Security.Claims.Claim("appUserId", appUser.Id.ToString())
        };

        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<User> EnsureDomainUserLinkedAsync(IdentityUser identityUser, string? fallbackName)
    {
        var domainUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);
        if (domainUser != null)
        {
            if (!string.Equals(domainUser.Email, identityUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                domainUser.UpdateEmail(identityUser.Email ?? domainUser.Email);
                await _appDbContext.SaveChangesAsync();
            }

            return domainUser;
        }

        domainUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == identityUser.Email);
        if (domainUser != null)
        {
            domainUser.LinkIdentityUser(identityUser.Id);
            await _appDbContext.SaveChangesAsync();
            return domainUser;
        }

        var name = string.IsNullOrWhiteSpace(fallbackName) ? identityUser.Email ?? "User" : fallbackName;
        domainUser = new User(name, identityUser.Email ?? string.Empty, identityUser.Id);
        _appDbContext.Users.Add(domainUser);
        await _appDbContext.SaveChangesAsync();

        return domainUser;
    }
}
