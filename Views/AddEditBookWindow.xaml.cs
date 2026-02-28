using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement.Views
{
    public partial class AddEditBookWindow : Window
    {
        private readonly LibraryContext _context;
        private Book _book;
        private readonly int? _bookId;
        private readonly bool _isEditMode;

        public AddEditBookWindow(LibraryContext context, int? bookId = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bookId = bookId;
            _isEditMode = bookId.HasValue;

            InitializeComponent();

            try
            {
                // Загружаем авторов и жанры
                var authors = _context.Authors.ToList();
                var genres = _context.Genres.ToList();
                AuthorComboBox.ItemsSource = authors;
                GenreComboBox.ItemsSource = genres;

                if (_isEditMode && _bookId.HasValue)
                {
                    Title = "Редактирование книги";
                    TitleTextBlock.Text = "Редактирование книги";

                    _book = _context.Books.FirstOrDefault(b => b.Id == _bookId.Value);

                    if (_book != null)
                    {
                        TitleTextBox.Text = _book.Title ?? string.Empty;

                        // Выбираем автора и жанр через ItemsSource (без прямого перечисления ItemCollection)
                        var selectedAuthor = authors.FirstOrDefault(a => a.Id == _book.AuthorId);
                        if (selectedAuthor != null)
                            AuthorComboBox.SelectedItem = selectedAuthor;

                        var selectedGenre = genres.FirstOrDefault(g => g.Id == _book.GenreId);
                        if (selectedGenre != null)
                            GenreComboBox.SelectedItem = selectedGenre;

                        YearTextBox.Text = _book.PublishYear.ToString();
                        ISBNTextBox.Text = _book.ISBN ?? string.Empty;
                        QuantityTextBox.Text = _book.QuantityInStock.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show(this, "Введите название книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (AuthorComboBox.SelectedItem is not Author)
                {
                    MessageBox.Show(this, "Выберите автора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (GenreComboBox.SelectedItem is not Genre)
                {
                    MessageBox.Show(this, "Выберите жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int currentYear = DateTime.Now.Year;
                if (!int.TryParse(YearTextBox.Text, out int year) || year < 1800 || year > currentYear)
                {
                    MessageBox.Show(this, $"Введите корректный год (1800-{currentYear})", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show(this, "Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode)
                {
                    // Редактирование
                    int? idToFind = _book?.Id ?? _bookId;
                    if (!idToFind.HasValue)
                    {
                        MessageBox.Show(this, "Не удалось определить редактируемую книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var book = _context.Books.Find(idToFind.Value);
                    if (book != null)
                    {
                        book.Title = TitleTextBox.Text;
                        if (AuthorComboBox.SelectedItem is Author selA)
                            book.AuthorId = selA.Id;
                        if (GenreComboBox.SelectedItem is Genre selG)
                            book.GenreId = selG.Id;
                        book.PublishYear = year;
                        book.ISBN = ISBNTextBox.Text;
                        book.QuantityInStock = quantity;
                    }
                    else
                    {
                        MessageBox.Show(this, "Книга для редактирования не найдена в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // Добавление — безопасные приведения с проверкой типов
                    if (AuthorComboBox.SelectedItem is not Author newAuthor || GenreComboBox.SelectedItem is not Genre newGenre)
                    {
                        MessageBox.Show(this, "Выберите корректного автора и жанр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newBook = new Book
                    {
                        Title = TitleTextBox.Text,
                        AuthorId = newAuthor.Id,
                        GenreId = newGenre.Id,
                        PublishYear = year,
                        ISBN = ISBNTextBox.Text,
                        QuantityInStock = quantity
                    };
                    _context.Books.Add(newBook);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
