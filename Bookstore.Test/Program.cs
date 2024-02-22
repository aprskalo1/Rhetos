using ConsoleDump;
using Microsoft.AspNetCore.Mvc;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Logging;
using Rhetos.Utilities;

ConsoleLogger.MinLevel = EventType.Info; // Use EventType.Trace for more detailed log.
string rhetosHostAssemblyPath = @"C:\Users\aprskalo\Desktop\Day1\Bookstore.Service\bin\Debug\net6.0\Bookstore.Service.dll";
using (var scope = LinqPadRhetosHost.CreateScope(rhetosHostAssemblyPath))
{
    var context = scope.Resolve<Common.ExecutionContext>();
    var repository = context.Repository;

    // Query data from the `Common.Claim` table:

    var claims = repository.Common.Claim.Query()
        .Where(c => c.ClaimResource.StartsWith("Common.") && c.ClaimRight == "New")
        .ToSimple(); // Removes ORM navigation properties from the loaded objects.

    claims.ToString().Dump("Common.Claims SQL query");
    claims.Dump("Common.Claims items");

    // Add and remove a `Common.Principal`:

    var testUser = new Common.Principal { Name = "Test123", ID = Guid.NewGuid() };
    repository.Common.Principal.Insert(new[] { testUser });
    repository.Common.Principal.Delete(new[] { testUser });

    // Print logged events for the `Common.Principal`:

    repository.Common.LogReader.Query()
        .Where(log => log.TableName == "Common.Principal" && log.ItemId == testUser.ID)
        .ToList()
        .Dump("Common.Principal log");

    Console.WriteLine("Done.");

    //scope.CommitAndClose(); // Database transaction is rolled back by default.

    /*By using only Load() methods from the repositories, for each book print the book title and the name of its author.
    Notes: You can use the Load() method without parameters to read all the books, and then use the Load(Guid[]) method with ID parameter to read the author for each book.
    This approach is not efficient.*/

    //i have Person which can mbe any person, then i have BookAuthor which is a link between book and person by ID i now get all books with their authors 
    /*By using a Query() method, for all books print the book title and the name of its author.
    Notes: This will require only one Query() with a Select() method. Use the Dump() method to print all required data at once.
    Use the ToString() method on the LINQ query from the previous task to print the generated SQL query.*/
    //bookauthor is not a table, it is a link between book and person

    var query = repository.Bookstore.BookAuthor.Query()
    .Select(ba => new
    {
        Book = ba.Book.Title,
        Author = ba.Author.Name
    });

    var booksAndAuthors = query.ToList();
    var sqlQuery = query.ToString();

    booksAndAuthors.Dump("Books and Authors");
    Console.WriteLine(sqlQuery);

    var actionParameters = new Bookstore.CreateBooks
    {
        NumberOfBooks = 5,
        Title = "Auto Generated Book",
        BookName = "Auto Generated Book",
    };
    repository.Bookstore.CreateBooks.Execute(actionParameters);

    var filterParameter = new Bookstore.LongBooks
    {
        NumberOfPages = 100
    };
    var longBooksQuery = repository.Bookstore.Book.Query(filterParameter);
    longBooksQuery.ToString().Dump();
    longBooksQuery.ToSimple().ToList().Dump();
}
