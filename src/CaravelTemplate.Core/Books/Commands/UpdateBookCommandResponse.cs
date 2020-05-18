using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class UpdateBookCommandResponse :
        OneOfBase<UpdateBookCommandResponse.Success, UpdateBookCommandResponse.NotFound>
    {
        public class Success : UpdateBookCommandResponse
        {
            public BookModel Response { get; }

            public Success(BookModel response)
            {
                Response = response;
            }
        }

        public class NotFound : UpdateBookCommandResponse
        {
            public Error Error { get; set; }

            public NotFound(Error error)
            {
                Error = error;
            }
        }
    }
}