using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название жанра обязательно")]
        [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Максимальная длина 500 символов")]
        public string Description { get; set; }

        // Навигационное свойство
        public ICollection<Book> Books { get; set; }
    }
}