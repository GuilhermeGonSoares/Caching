namespace Api.Entities;

public class Library
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public List<Book> Books { get; private set; }

#pragma warning disable CS8618
    public Library() { }
#pragma warning restore CS8618

    public Library(string name)
    {
        Name = name;
        Books = [];
    }

    public void AddBook(string title, string author)
    {
        if (Books.Any(b => b.Title == title))
        {
            return;
        }
        var book = new Book(title, author, true);
        Books.Add(book);
    }

    public void RemoveBook(int bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");
        Books.Remove(book);
    }

    public void BorrowBook(int bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");

        book.Borrow();
    }

    public void ReturnBook(int bookId)
    {
        var book =
            Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found");

        book.Return();
    }
}
