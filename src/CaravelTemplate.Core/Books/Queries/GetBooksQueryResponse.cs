using System.Collections.Generic;
using OneOf;

namespace CaravelTemplate.Core.Books.Queries
{
    public class GetBooksQueryResponse : OneOfBase<GetBooksQueryResponse.Success>
    {
        private GetBooksQueryResponse(OneOf<Success> _) : base(_) { }

        public record Success (IEnumerable<BookModel> Books);
        public static implicit operator GetBooksQueryResponse(Success _) => new (_);
    }
}