using LuxMedTest.Application.Dtos;
using MediatR;

namespace LuxMedTest.Application.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<RefreshTokenDto> { }
}
