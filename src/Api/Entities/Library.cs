namespace Api.Entities;

public class Library
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public List<Book> Books { get; private set; }

#pragma warning disable CS8618
    public Library() { }
#pragma warning restore CS8618

    public Library(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Books = [];
    }

    public void AddBook(string title, string author)
    {
        Books.Add(new Book(title, author, true));
    }

    public void RemoveBook(Guid bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");

        Books.Remove(book);
    }

    public void BorrowBook(Guid bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");

        book.Borrow();
    }

    public void ReturnBook(Guid bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");

        book.Return();
    }
}
