using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public class LoginUserCommandResponse : 
        OneOfBase<LoginUserCommandResponse.Success, LoginUserCommandResponse.NotFound, LoginUserCommandResponse.InvalidPassword>
    {
        public class Success : LoginUserCommandResponse
        {
            public AccessTokenModel Response { get; }

            public Success(AccessTokenModel response)
            {
                Response = response;
            }
        }
        
        public class NotFound : LoginUserCommandResponse
        {
            public Error Error { get; set; }

            public NotFound(Error error)
            {
                Error = error;
            }
        }
        
        public class InvalidPassword : LoginUserCommandResponse
        {
            public Error Error { get; set; }

            public InvalidPassword(Error error)
            {
                Error = error;
            }
        }
    }
}