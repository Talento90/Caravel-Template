using Caravel.Errors;
using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class DeleteBookCommandResponse : OneOfBase<DeleteBookCommandResponse.Success, DeleteBookCommandResponse.NotFound>
    {
        private DeleteBookCommandResponse(OneOf<Success, NotFound> _): base(_)
        {
            
        }
        public record Success { }

        public record NotFound (Error Error);
        
        public static implicit operator DeleteBookCommandResponse(Success r) => new (r);
        public static implicit operator DeleteBookCommandResponse(NotFound r) => new (r);
    }
}