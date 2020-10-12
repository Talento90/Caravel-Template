using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Users.Commands
{
    public class CreateUserCommandResponse : 
        OneOfBase<CreateUserCommandResponse.Success, CreateUserCommandResponse.InvalidUser>
    {
        public class Success : CreateUserCommandResponse
        {
            public UserModel Response { get; }

            public Success(UserModel response)
            {
                Response = response;
            }
        }
        
        public class InvalidUser : CreateUserCommandResponse
        {
            public Error Error { get; set; }

            public InvalidUser(Error error)
            {
                Error = error;
            }
        }
    }
}