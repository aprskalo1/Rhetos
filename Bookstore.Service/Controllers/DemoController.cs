using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;
using System.Security.Claims;

[Route("Demo/[action]")]
public class DemoController : ControllerBase
{
    private readonly IProcessingEngine processingEngine;
    private readonly IUnitOfWork unitOfWork;

    public DemoController(IRhetosComponent<IProcessingEngine> processingEngine, IRhetosComponent<IUnitOfWork> unitOfWork)
    {
        this.processingEngine = processingEngine.Value;
        this.unitOfWork = unitOfWork.Value;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task Login()
    {
        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "SampleUser") }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties() { IsPersistent = true });
    }

    [HttpGet]
    public IActionResult GetBookByPageNumber(int pageNumber)
    {
        string rhetosHostAssemblyPath = @"C:\Users\aprskalo\Desktop\Day1\Bookstore.Service\bin\Debug\net6.0\Bookstore.Service.dll";

        var scope = LinqPadRhetosHost.CreateScope(rhetosHostAssemblyPath);
        var context = scope.Resolve<Common.ExecutionContext>();
        var repository = context.Repository;

        var filterParameter = new Bookstore.LongBooks
        {
            NumberOfPages = pageNumber
        };
        var longBooksQuery = repository.Bookstore.Book.Query(filterParameter);
        return new JsonResult(longBooksQuery);
    }

    [HttpGet]
    public string ReadBooks()
    {
        var readCommandInfo = new ReadCommandInfo { DataSource = "Bookstore.Book", ReadTotalCount = true };
        var result = processingEngine.Execute(readCommandInfo);
        return $"{result.TotalCount} books.";
    }

    [HttpGet]
    public string WriteBook(string bookName, string title, string authorName, int numberOfPages)
    {
        var newBook = new Bookstore.Book { Title = title, BookName = bookName, NumberOfPages = numberOfPages };
        processingEngine.Execute(new SaveEntityCommandInfo { Entity = "Bookstore.Book", DataToInsert = new[] { newBook } });

        var newPerson = new Bookstore.Person { Name = authorName };
        processingEngine.Execute(new SaveEntityCommandInfo { Entity = "Bookstore.Person", DataToInsert = new[] { newPerson } });

        var bookAuthor = new Bookstore.BookAuthor { BookID = newBook.ID, AuthorID = newPerson.ID };
        processingEngine.Execute(new SaveEntityCommandInfo { Entity = "Bookstore.BookAuthor", DataToInsert = new[] { bookAuthor } });

        unitOfWork.CommitAndClose();
        return "Book successfully written.";
    }
}