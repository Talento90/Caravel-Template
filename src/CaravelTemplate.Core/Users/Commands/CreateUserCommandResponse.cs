using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Users.Commands
{
    public class CreateUserCommandResponse : OneOfBase<CreateUserCommandResponse.Success, CreateUserCommandResponse.InvalidUser>
    {
        private CreateUserCommandResponse(OneOf<Success, InvalidUser> _): base(_) { }
        
        public record Success(UserModel Response);

        public record InvalidUser(Error Error);
        
        public static implicit operator CreateUserCommandResponse(Success r) => new (r);
        public static implicit operator CreateUserCommandResponse(InvalidUser r) => new (r);
    }
}