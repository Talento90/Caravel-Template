using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public class LoginUserCommandResponse : 
        OneOfBase<LoginUserCommandResponse.Success, LoginUserCommandResponse.NotFound, LoginUserCommandResponse.InvalidPassword>
    {
        private LoginUserCommandResponse(OneOf<Success,NotFound, InvalidPassword> _):base(_) { }
        public record Success (AccessTokenModel Response);
        public record NotFound (Error Error);
        public record InvalidPassword (Error Error);
        
        public static implicit operator LoginUserCommandResponse(Success r) => new (r);
        public static implicit operator LoginUserCommandResponse(NotFound r) => new (r);
        public static implicit operator LoginUserCommandResponse(InvalidPassword r) => new (r);

    }
}