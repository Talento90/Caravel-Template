using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Queries
{
    public class GetBookByIdQueryResponse : OneOfBase<GetBookByIdQueryResponse.Success, GetBookByIdQueryResponse.NotFound> {
        private GetBookByIdQueryResponse(OneOf<Success, NotFound> _) : base(_) { }

        public record Success(BookModel Response);

        public record NotFound(Error Error);
        
        public static implicit operator GetBookByIdQueryResponse(Success r) => new(r);
        public static implicit operator GetBookByIdQueryResponse(NotFound r) => new(r);
    }
}