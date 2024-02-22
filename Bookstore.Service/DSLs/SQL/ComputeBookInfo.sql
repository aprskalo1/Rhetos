SELECT
    b.ID,
    NumberOfChapters = COUNT(bc.ID)
FROM
    Bookstore.Book b
    LEFT JOIN Bookstore.Chapter bc ON bc.BookID = b.ID
GROUP BY
    b.ID