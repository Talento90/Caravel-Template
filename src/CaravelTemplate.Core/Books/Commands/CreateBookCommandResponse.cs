using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class CreateBookCommandResponse : 
        OneOfBase<CreateBookCommandResponse.Success>
    {
        public class Success : CreateBookCommandResponse
        {
            public BookModel Response { get; }

            public Success(BookModel response)
            {
                Response = response;
            }
        }
    }
}