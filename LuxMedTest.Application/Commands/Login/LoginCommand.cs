using FluentValidation;
using LuxMedTest.Application.Dtos;
using MediatR;

namespace LuxMedTest.Application.Commands.Login
{
    public record LoginCommand : IRequest<RefreshTokenDto>
    {
        public required string Username { get; init; }
        public required string Password { get; init; }

        public class LoginCommandValidator : AbstractValidator<LoginCommand>
        {
            public LoginCommandValidator()
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            }
        }
    }
}
