using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CaravelTemplate.Application.Metrics;

public class BookMetrics
{
    public const string MeterName = "CaravelTemplate.BookMeter";

    private readonly Counter<long> _booksCreatedCounter;

    public BookMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _booksCreatedCounter = meter.CreateCounter<long>(
            "books.created",
            description: "Number of books created"
        );
    }

    public void IncrementBookCreated(Guid bookId)
    {
        _booksCreatedCounter.Add(1, new TagList()
        {
            new("book.id", bookId.ToString())
        });
    }
}