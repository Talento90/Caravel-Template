using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public class RefreshTokenCommandResponse :
        OneOfBase<
            RefreshTokenCommandResponse.Success,
            RefreshTokenCommandResponse.RefreshTokenNotFound,
            RefreshTokenCommandResponse.InvalidAccessToken
        >
    {
        public class Success : RefreshTokenCommandResponse
        {
            public AccessTokenModel Response { get; }

            public Success(AccessTokenModel response)
            {
                Response = response;
            }
        }

        public class RefreshTokenNotFound : RefreshTokenCommandResponse
        {
            public Error Error { get; }

            public RefreshTokenNotFound(Error error)
            {
                Error = error;
            }
        }

        public class InvalidAccessToken : RefreshTokenCommandResponse
        {
            public Error Error { get; }

            public InvalidAccessToken(Error error)
            {
                Error = error;
            }
        }
    }
}