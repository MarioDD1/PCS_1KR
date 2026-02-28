using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Попытка использовать LocalDB SQL Server может привести к ошибкам на машинах без установленного LocalDB.
            // Чтобы приложение работало без установки SQL Server, используем локальную SQLite базу данных.
            // Если требуется SQL Server, замените строку ниже на нужный connection string и пакет SqlServer.
            optionsBuilder.UseSqlite("Data Source=LibraryManagement.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для Book с использованием Fluent API
            modelBuilder.Entity<Book>(entity =>
            {
                // Первичный ключ
                entity.HasKey(e => e.Id);

                // Обязательные поля и ограничения
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ISBN)
                    .HasMaxLength(20);

                entity.Property(e => e.PublishYear)
                    .IsRequired();

                entity.Property(e => e.QuantityInStock)
                    .IsRequired()
                    .HasDefaultValue(0);

                // Настройка связей
                entity.HasOne(e => e.Author)
                    .WithMany(e => e.Books)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict); // Запрет каскадного удаления

                entity.HasOne(e => e.Genre)
                    .WithMany(e => e.Books)
                    .HasForeignKey(e => e.GenreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Индексы для ускорения поиска
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.AuthorId);
                entity.HasIndex(e => e.GenreId);
            });

            // Конфигурация для Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Country)
                    .HasMaxLength(100);

                entity.Property(e => e.BirthDate)
                    .IsRequired();

                // Индекс для поиска по имени
                entity.HasIndex(e => new { e.LastName, e.FirstName });
            });

            // Конфигурация для Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                // Уникальное название жанра
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            // Заполнение начальными данными
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Добавление жанров
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Роман", Description = "Художественное произведение большого объема" },
                new Genre { Id = 2, Name = "Детектив", Description = "Произведение о расследовании преступлений" },
                new Genre { Id = 3, Name = "Фантастика", Description = "Произведение о вымышленных мирах" },
                new Genre { Id = 4, Name = "Поэзия", Description = "Стихотворные произведения" },
                new Genre { Id = 5, Name = "Биография", Description = "Описание жизни человека" }
            );

            // Добавление авторов
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Лев", LastName = "Толстой", BirthDate = new DateTime(1828, 9, 9), Country = "Россия" },
                new Author { Id = 2, FirstName = "Федор", LastName = "Достоевский", BirthDate = new DateTime(1821, 11, 11), Country = "Россия" },
                new Author { Id = 3, FirstName = "Антон", LastName = "Чехов", BirthDate = new DateTime(1860, 1, 29), Country = "Россия" },
                new Author { Id = 4, FirstName = "Александр", LastName = "Пушкин", BirthDate = new DateTime(1799, 6, 6), Country = "Россия" },
                new Author { Id = 5, FirstName = "Михаил", LastName = "Булгаков", BirthDate = new DateTime(1891, 5, 15), Country = "Россия" }
            );

            // Добавление книг
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Война и мир", AuthorId = 1, GenreId = 1, PublishYear = 1869, ISBN = "978-5-17-123456-7", QuantityInStock = 10 },
                new Book { Id = 2, Title = "Анна Каренина", AuthorId = 1, GenreId = 1, PublishYear = 1877, ISBN = "978-5-17-123457-4", QuantityInStock = 8 },
                new Book { Id = 3, Title = "Преступление и наказание", AuthorId = 2, GenreId = 1, PublishYear = 1866, ISBN = "978-5-17-123458-1", QuantityInStock = 7 },
                new Book { Id = 4, Title = "Идиот", AuthorId = 2, GenreId = 1, PublishYear = 1869, ISBN = "978-5-17-123459-8", QuantityInStock = 5 },
                new Book { Id = 5, Title = "Вишневый сад", AuthorId = 3, GenreId = 4, PublishYear = 1904, ISBN = "978-5-17-123460-4", QuantityInStock = 6 },
                new Book { Id = 6, Title = "Евгений Онегин", AuthorId = 4, GenreId = 1, PublishYear = 1833, ISBN = "978-5-17-123461-1", QuantityInStock = 9 },
                new Book { Id = 7, Title = "Мастер и Маргарита", AuthorId = 5, GenreId = 3, PublishYear = 1967, ISBN = "978-5-17-123462-8", QuantityInStock = 12 }
            );
        }
    }
}