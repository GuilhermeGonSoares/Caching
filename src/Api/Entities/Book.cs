namespace Api.Entities;

public class Book
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public bool IsAvailable { get; private set; }

#pragma warning disable CS8618
    public Book() { }
#pragma warning restore CS8618

    public Book(string title, string author, bool isAvailable)
    {
        Title = title;
        Author = author;
        IsAvailable = isAvailable;
    }

    public void Update(string title, string author, bool isAvailable)
    {
        Title = title;
        Author = author;
        IsAvailable = isAvailable;
    }

    public void Borrow()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("Book is not available");
        }

        IsAvailable = false;
    }

    public void Return()
    {
        if (IsAvailable)
        {
            throw new InvalidOperationException("Book is already available");
        }

        IsAvailable = true;
    }
}
