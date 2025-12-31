using QuizDesk.Application.Features.Auth.DTOs;
using QuizDesk.Domain.Entities;

namespace QuizDesk.Application.Common.Mappings
{
    public static class UserToDtoMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                AvatarUrl = user.AvatarUrl,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            };

        }
    }
}
