using System.Collections.Generic;
using OneOf;

namespace CaravelTemplate.Core.Books.Queries
{
    public class
        GetBooksQueryResponse : OneOfBase<GetBooksQueryResponse.Success>
    {
        public class Success : GetBooksQueryResponse
        {
            public IEnumerable<BookModel> Books { get; }

            public Success(IEnumerable<BookModel> books)
            {
                Books = books;
            }
        }
    }
}