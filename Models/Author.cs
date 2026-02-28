using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
        public string Country { get; set; }

        // Навигационное свойство
        public ICollection<Book> Books { get; set; }

        // Вычисляемое поле для полного имени
        public string FullName => $"{FirstName} {LastName}";
    }
}