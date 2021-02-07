using OneOf;

namespace CaravelTemplate.Core.Books.Commands
{
    public class CreateBookCommandResponse : OneOfBase<CreateBookCommandResponse.Success>
    {
        public CreateBookCommandResponse(OneOf<Success> _) : base(_)
        {
        }
        public record Success(BookModel Response);
        
        public static implicit operator CreateBookCommandResponse(Success r) => new (r);
    }
}