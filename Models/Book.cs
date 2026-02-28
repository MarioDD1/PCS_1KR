using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        [StringLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        public string Title { get; set; }

        [Range(1800, 2026, ErrorMessage = "Год должен быть от 1800 до 2026")]
        public int PublishYear { get; set; }

        [StringLength(20, ErrorMessage = "Максимальная длина ISBN 20 символов")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Количество в наличии обязательно")]
        [Range(0, 1000, ErrorMessage = "Количество должно быть от 0 до 1000")]
        public int QuantityInStock { get; set; }

        // Внешние ключи
        public int AuthorId { get; set; }
        public int GenreId { get; set; }

        // Навигационные свойства
        public Author Author { get; set; }
        public Genre Genre { get; set; }
    }
}