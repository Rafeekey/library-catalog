using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryCatalog
{
    public class Book
    {
        private int _id;
        private string _title;
        private string _publicationDate;
        private int _copyCount;
        private int _authorCount;

        public Book(string title, string publicationDate, int copyCount = 0, int authorCount = 0, int id = 0)
        {
            _title = title;
            _publicationDate = publicationDate;
            _copyCount = copyCount;
            _authorCount = authorCount;
            _id = id;
        }

        public override bool Equals(System.Object otherBook)
        {
            if(!(otherBook is Book))
            {
                return false;
            }
            else
            {
                Book newBook = (Book) otherBook;
                bool idEquality = this.GetId() == newBook.GetId();
                bool titleEquality = this.GetTitle() == newBook.GetTitle();
                bool publicationDateEquality = this.GetPublicationDate() == newBook.GetPublicationDate();
                bool copyCountEquality = this.GetCopyCount() == newBook.GetCopyCount();
                bool authorCountEquality = this.GetAuthorCount() == newBook.GetAuthorCount();
                return(idEquality && titleEquality);
            }
        }

        public override int GetHashCode()
        {
            return this.GetTitle().GetHashCode();
        }

        public void Save()
        {
            Book foundBook = Book.FindByTitle(this.GetTitle());
            if(foundBook.GetCopyCount() == 0)
            {
                this.SetCopyCount();
                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO books (title, publication_date, copy_count, author_count) OUTPUT INSERTED.id VALUES (@Title, @PublicationDate, @CopyCount, @AuthorCount);", conn);

                cmd.Parameters.Add(new SqlParameter("@Title", this.GetTitle()));
                cmd.Parameters.Add(new SqlParameter("@PublicationDate", this.GetPublicationDate()));
                cmd.Parameters.Add(new SqlParameter("@CopyCount", this.GetCopyCount()));
                cmd.Parameters.Add(new SqlParameter("@AuthorCount", this.GetAuthorCount()));

                SqlDataReader rdr = cmd.ExecuteReader();

                while(rdr.Read())
                {
                    this._id = rdr.GetInt32(0);
                }

                DB.CloseSqlConnection(rdr, conn);
            }
            else{
                foundBook.SetCopyCount();
                this._id = foundBook.GetId();
                this._title = foundBook.GetTitle();
                this._publicationDate = foundBook.GetPublicationDate();
                this._authorCount = foundBook.GetAuthorCount();
                this._copyCount = foundBook.GetCopyCount();
            }
        }



        public static Book FindById(int booksId)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BooksId;", conn);

            cmd.Parameters.Add(new SqlParameter("@BooksId", booksId));

            SqlDataReader rdr = cmd.ExecuteReader();

            int bookId = 0;
            string bookTitle = null;
            string bookPublicationDate = null;
            int bookCopyCount = 0;
            int bookAuthorCount = 0;

            while(rdr.Read())
            {
                bookId = rdr.GetInt32(0);
                bookTitle = rdr.GetString(1);
                bookPublicationDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                bookCopyCount = rdr.GetInt32(3);
                bookAuthorCount = rdr.GetInt32(4);
            }

            Book newBook = new Book(bookTitle, bookPublicationDate, bookCopyCount, bookAuthorCount, bookId);

            DB.CloseSqlConnection(rdr, conn);

            return newBook;
        }

        public static Book FindByAuthorName(string authorName)
        {
            Author foundAuthor = Author.FindByName(authorName);

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT books.* FROM authors JOIN books_authors ON (authors.id = books_authors.authors_id) JOIN books ON (books_authors.books_id = books.id) WHERE authors.id = @AuthorId;", conn);

            cmd.Parameters.Add(new SqlParameter("@AuthorId", foundAuthor.GetId()));

            SqlDataReader rdr = cmd.ExecuteReader();

            int bookId = 0;
            string bookTitle = null;
            string bookPublicationDate = null;
            int bookCopyCount = 0;
            int bookAuthorCount = 0;

            while(rdr.Read())
            {
                bookId = rdr.GetInt32(0);
                bookTitle = rdr.GetString(1);
                bookPublicationDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                bookCopyCount = rdr.GetInt32(3);
                bookAuthorCount = rdr.GetInt32(4);
            }

            Book newBook = new Book(bookTitle, bookPublicationDate, bookCopyCount, bookAuthorCount, bookId);

            DB.CloseSqlConnection(rdr, conn);

            return newBook;
        }

        public static Book FindByTitle(string title)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE title = @BooksTitle;", conn);

            cmd.Parameters.Add(new SqlParameter("@BooksTitle", title));

            SqlDataReader rdr = cmd.ExecuteReader();

            int bookId = 0;
            string bookTitle = null;
            string bookPublicationDate = null;
            int bookCopyCount = 0;
            int bookAuthorCount = 0;

            while(rdr.Read())
            {
                bookId = rdr.GetInt32(0);
                bookTitle = rdr.GetString(1);
                bookPublicationDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                bookCopyCount = rdr.GetInt32(3);
                bookAuthorCount = rdr.GetInt32(4);
            }

            Book newBook = new Book(bookTitle, bookPublicationDate, bookCopyCount, bookAuthorCount, bookId);

            DB.CloseSqlConnection(rdr, conn);

            return newBook;
        }

        public void AddAuthor(int id)
        {
            this.SetAuthorCount();

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (books_id, authors_id) VALUES (@BooksId, @AuthorsId); UPDATE books SET author_count = @AuthorCount WHERE id = @BooksId;", conn);

            cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId().ToString()));
            cmd.Parameters.Add(new SqlParameter("@AuthorsId", id.ToString()));
            cmd.Parameters.Add(new SqlParameter("@AuthorCount", this.GetAuthorCount().ToString()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public void UpdateTitle(string newTitle)
        {
            Book foundBook = Book.FindByTitle(newTitle);
            if(foundBook.GetCopyCount() == 0)
            {
                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE books SET title = @Title WHERE id = @BooksId;", conn);

                cmd.Parameters.Add(new SqlParameter("@Title", newTitle));
                cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId()));

                cmd.ExecuteNonQuery();

                if(conn != null)
                {
                    conn.Close();
                }
            }
            else
            {
                foundBook.SetCopyCount();
                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE books SET title = @Title, publication_date = @PublicationDate, copy_count = @CopyCount, author_count = @AuthorCount WHERE id = @BooksId;", conn);

                cmd.Parameters.Add(new SqlParameter("@Title", foundBook.GetTitle()));
                cmd.Parameters.Add(new SqlParameter("@PublicationDate", foundBook.GetPublicationDate()));
                cmd.Parameters.Add(new SqlParameter("@CopyCount", foundBook.GetCopyCount()));
                cmd.Parameters.Add(new SqlParameter("@AuthorCount", foundBook.GetAuthorCount()));
                cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId()));

                cmd.ExecuteNonQuery();

                if(conn != null)
                {
                    conn.Close();
                }
            }
        }


        public int AvailableCopiesOfBook()
        {
            List<Copy> allCopies = Copy.GetAll();
            int booksAvailable = 0;

            foreach(var copy in allCopies)
            {
                if(copy.GetBooksId() == this.GetId())
                {
                    booksAvailable++;
                }
            }
            return booksAvailable;
        }



        public void DeleteBook()
        {
            Book foundBook = Book.FindById(this.GetId());
            foundBook.MinusCopyCount();

            List<Copy> allCopies = Copy.CopiesOfBook(this.GetId());

            if(allCopies.Count != 0)
            {
                SqlConnection conn1 = DB.Connection();
                conn1.Open();

                SqlCommand cmd1 = new SqlCommand("DELETE FROM copies WHERE id = @CopiesId;", conn1);

                cmd1.Parameters.Add(new SqlParameter("@CopiesId", allCopies[0].GetId()));

                cmd1.ExecuteNonQuery();

                if (conn1 != null)
                {
                    conn1.Close();
                }
            }

            if(foundBook.GetCopyCount() <= 0)
            {
                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BooksId; DELETE FROM books_authors WHERE books_id = @BooksId;", conn);
                cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId()));

                cmd.ExecuteNonQuery();

                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static List<Book> GetAll()
         {
             List<Book> allBooks = new List<Book>{};

             SqlConnection conn = DB.Connection();
             conn.Open();

             SqlCommand cmd = new SqlCommand("SELECT * FROM books", conn);

             SqlDataReader rdr = cmd.ExecuteReader();

             while(rdr.Read())
             {
                 int bookId = rdr.GetInt32(0);
                 string bookName = rdr.GetString(1);
                 string bookPublicationDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                 int bookCopyCount = rdr.GetInt32(3);
                 int bookAuthorCount = rdr.GetInt32(4);
                 Book newBook = new Book(bookName, bookPublicationDate, bookCopyCount, bookAuthorCount, bookId);
                 allBooks.Add(newBook);
             }

             DB.CloseSqlConnection(rdr, conn);

             return allBooks;
         }

         public List<Book> BooksNotCheckedOut()
         {
             List<Book> booksCheckedOut = new List<Book>{};

             SqlConnection conn = DB.Connection();
             conn.Open();

             SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE books_id = @BooksId AND checkedout = @Checkedout;", conn);

             cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId()));
             cmd.Parameters.Add(new SqlParameter("@Checkedout", "0"));

             SqlDataReader rdr = cmd.ExecuteReader();

             while(rdr.Read())
             {
                 int bookId = rdr.GetInt32(0);
                 string bookName = rdr.GetString(1);
                 string bookPublicationDate = rdr.GetDateTime(2).ToString("yyyy-MM-dd");
                 int bookCopyCount = rdr.GetInt32(3);
                 int bookAuthorCount = rdr.GetInt32(4);
                 Book newBook = new Book(bookName, bookPublicationDate, bookCopyCount, bookAuthorCount, bookId);
                 booksCheckedOut.Add(newBook);
             }

             DB.CloseSqlConnection(rdr, conn);

             return booksCheckedOut;
         }

         public List<Author> GetAuthors()
         {
             List<Author> allAuthors = new List<Author>{};

             SqlConnection conn = DB.Connection();
             conn.Open();

             SqlCommand cmd = new SqlCommand("SELECT authors.* FROM books JOIN books_authors ON (books.id = books_authors.books_id) JOIN authors ON (authors.id = books_authors.authors_id) WHERE books.id = @BooksId;", conn);

             cmd.Parameters.Add(new SqlParameter("@BooksId", this.GetId().ToString()));

             SqlDataReader rdr = cmd.ExecuteReader();

             while(rdr.Read())
             {
                 int authorId = rdr.GetInt32(0);
                 string authorName = rdr.GetString(1);
                 Author newAuthor = new Author(authorName, authorId);
                 allAuthors.Add(newAuthor);
             }

             DB.CloseSqlConnection(rdr, conn);

             return allAuthors;
         }


        public int GetId()
        {
            return _id;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public string GetTitle()
        {
            return _title;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public string GetPublicationDate()
        {
            return _publicationDate;
        }

        public void SetPublicationDate(string publicationDate)
        {
            _publicationDate = publicationDate;
        }
        public int GetCopyCount()
        {
            return _copyCount;
        }

        public void SetCopyCount()
        {
            if(this.GetCopyCount() == 0)
            {
                _copyCount += 1;
            }
            else
            {
                _copyCount += 1;

                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE books SET copy_count = @CopyCount WHERE title = @BooksTitle;", conn);

                cmd.Parameters.Add(new SqlParameter("@CopyCount", this.GetCopyCount()));
                cmd.Parameters.Add(new SqlParameter("@BooksTitle", this.GetTitle()));

                cmd.ExecuteNonQuery();

                if(conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void MinusCopyCount()
        {
                _copyCount -= 1;

                SqlConnection conn = DB.Connection();
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE books SET copy_count = @CopyCount WHERE title = @BooksTitle;", conn);

                cmd.Parameters.Add(new SqlParameter("@CopyCount", this.GetCopyCount()));
                cmd.Parameters.Add(new SqlParameter("@BooksTitle", this.GetTitle()));

                cmd.ExecuteNonQuery();

                if(conn != null)
                {
                    conn.Close();
                }
        }
        public int GetAuthorCount()
        {
            return _authorCount;
        }

        public void SetAuthorCount()
        {
            _authorCount += 1;

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("UPDATE books SET author_count = @AuthorCount WHERE title = @BooksTitle;", conn);

            cmd.Parameters.Add(new SqlParameter("@AuthorCount", this.GetAuthorCount()));
            cmd.Parameters.Add(new SqlParameter("@BooksTitle", this.GetTitle()));

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            DB.TableDeleteAll("books");
        }
    }
}
