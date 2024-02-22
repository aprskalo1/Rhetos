using Bookstore.Service.Test.Tools;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.TestCommon;

namespace Bookstore.Service.Test
{
    [TestClass]
    public class BookTest
    {
        [TestMethod]
        public void AutomaticallyUpdateNumberOfChapters()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var book = new Book { Title = Guid.NewGuid().ToString() };
                repository.Bookstore.Book.Insert(book);

                int? readNumberOfChapters() => repository.Bookstore.BookInfo
                    .Query(bi => bi.ID == book.ID)
                    .Select(bi => bi.NumberOfChapters)
                    .Single();

                Assert.AreEqual(0, readNumberOfChapters());

                var c1 = new Chapter { BookID = book.ID, Heading = "c1" };
                var c2 = new Chapter { BookID = book.ID, Heading = "c2" };
                var c3 = new Chapter { BookID = book.ID, Heading = "c3" };

                repository.Bookstore.Chapter.Insert(c1);
                Assert.AreEqual(1, readNumberOfChapters());

                repository.Bookstore.Chapter.Insert(c2, c3);
                Assert.AreEqual(3, readNumberOfChapters());

                repository.Bookstore.Chapter.Delete(c1);
                Assert.AreEqual(2, readNumberOfChapters());

                repository.Bookstore.Chapter.Delete(c2, c3);
                Assert.AreEqual(0, readNumberOfChapters());
            }
        }

        [TestMethod]
        public void CommonMisspellingValidation()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var book = new Book { Title = "x curiousity y" };

                TestUtility.ShouldFail<UserException>(
                    () => repository.Bookstore.Book.Insert(book),
                    "It is not allowed to enter misspelled word");
            }
        }
    }
}