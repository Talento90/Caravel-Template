using AutoMapper;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Core.Books
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<CreateBookCommand, Book>();
            CreateMap<UpdateBookCommand, Book>();
            CreateMap<Book, BookModel>();
        }
    }
}