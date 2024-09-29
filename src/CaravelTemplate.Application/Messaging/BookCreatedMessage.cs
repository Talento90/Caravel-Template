using CaravelTemplate.Books;

namespace CaravelTemplate.Application.Messaging;

public record BookCreatedMessage(Guid Id, Book Book) : IMessage;
