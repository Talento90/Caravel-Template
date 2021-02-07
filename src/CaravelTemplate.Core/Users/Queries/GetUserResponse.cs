using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Users.Queries
{
    public class GetUserResponse : OneOfBase<GetUserResponse.Success, GetUserResponse.NotFound>
    {
        private GetUserResponse(OneOf<Success, NotFound> _) : base(_) { }

        public record Success (UserModel Response);

        public record NotFound (Error Error);
        
        public static implicit operator GetUserResponse(Success r) => new (r);
        public static implicit operator GetUserResponse(NotFound r) => new (r);
    }
}