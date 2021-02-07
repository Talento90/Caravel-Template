using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class UpdateBookCommandResponse : OneOfBase<UpdateBookCommandResponse.Success, UpdateBookCommandResponse.NotFound> {
        private UpdateBookCommandResponse(OneOf<Success, NotFound> _) : base(_)
        {
        }

        public record Success (BookModel Response);

        public record NotFound(Error Error);

        public static implicit operator UpdateBookCommandResponse(Success r) => new(r);
        public static implicit operator UpdateBookCommandResponse(NotFound r) => new(r);
    }
}