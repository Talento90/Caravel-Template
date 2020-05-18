using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class DeleteBookCommandResponse :
        OneOfBase<DeleteBookCommandResponse.Success, DeleteBookCommandResponse.NotFound>
    {
        public class Success : DeleteBookCommandResponse { }

        public class NotFound : DeleteBookCommandResponse
        {
            public Error Error { get; set; }

            public NotFound(Error error)
            {
                Error = error;
            }
        }
    }
}