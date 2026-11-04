
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Authorization;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Exceptions;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTicketBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(
            IAuthRepository authRepository,
            IConfiguration configuration,
            PasswordHasher passwordHasher)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        // Customer registration
        public async Task<AuthResponse> RegisterAsync(
            RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            var existingUsername =
                await _authRepository.GetByUsernameAsync(request.Username);

            if (existingUsername != null)
                throw new ConflictException("Username already exists.");

            var existingEmail =
                await _authRepository.GetByEmailAsync(request.Email);

            if (existingEmail != null)
                throw new ConflictException("Email already exists.");

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = Roles.Customer
            };

            await _authRepository.CreateAsync(user);

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateToken(user)
            };
        }

        // User login
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");

            var user =
                await _authRepository.GetByUsernameAsync(request.Username);

            if (user == null ||
                !_passwordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash))
            {
                throw new ArgumentException(
                    "Invalid username or password.");
            }

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateToken(user)
            };
        }

        // Generate JWT with user identity and role
        private string GenerateToken(User user)
        {
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "JWT key is not configured.");
            }

            var expiryMinutes =
                _configuration.GetValue<int>("Jwt:ExpiryMinutes");

            if (expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "JWT expiry minutes must be greater than zero.");
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role),

                new Claim(
                    ClaimTypes.Email,
                    user.Email)
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
