using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Users.Queries
{
    public class GetUserResponse : OneOfBase<GetUserResponse.Success, GetUserResponse.NotFound>
    {
        public class Success : GetUserResponse
        {
            public UserModel Response { get; }

            public Success(UserModel response)
            {
                Response = response;
            }
        }

        public class NotFound : GetUserResponse
        {
            public Error Error { get; set; }

            public NotFound(Error error)
            {
                Error = error;
            }
        }
    }
}