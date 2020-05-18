using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Queries
{
    public class
        GetBookByIdQueryResponse : OneOfBase<GetBookByIdQueryResponse.Success, GetBookByIdQueryResponse.NotFound>
    {
        public class Success : GetBookByIdQueryResponse
        {
            public BookModel Response { get; }

            public Success(BookModel response)
            {
                Response = response;
            }
        }

        public class NotFound : GetBookByIdQueryResponse
        {
            public Error Error { get; set; }

            public NotFound(Error error)
            {
                Error = error;
            }
        }
    }
}