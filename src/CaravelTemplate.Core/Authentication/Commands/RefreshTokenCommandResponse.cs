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
        private RefreshTokenCommandResponse(OneOf<Success, RefreshTokenNotFound, InvalidAccessToken> _):base(_) { }
        
        public record Success (AccessTokenModel Response);
        public record RefreshTokenNotFound (Error Error);
        public record InvalidAccessToken (Error Error);
        
        public static implicit operator RefreshTokenCommandResponse(Success r) => new (r);
        public static implicit operator RefreshTokenCommandResponse(RefreshTokenNotFound r) => new (r);
        public static implicit operator RefreshTokenCommandResponse(InvalidAccessToken r) => new (r);
    }
}