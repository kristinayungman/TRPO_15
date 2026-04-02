using Pr15_Shop.Models;
using Pr15_Shop.Service;
using Pr15_Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pr15_Shop.Pages
{
    public partial class AddProductPage : Page
    {
        private readonly Pr15ShopContext _db = DBService.Instance.Context;
        public Product CurrentProduct { get; set; } = new()
        {
            CreatedAt = DateOnly.FromDateTime(DateTime.Now),
            Rating = 0,
            Stock = 0,
            Price = 0
        };

        public AddProductPage()
        {
            InitializeComponent();
            DataContext = CurrentProduct;
            LoadComboBoxes();
        }
        public AddProductPage(Product productToEdit)
        {
            InitializeComponent();
            CurrentProduct = productToEdit;
            DataContext = CurrentProduct;
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            // Загружаем категории
            var categories = _db.Categories.OrderBy(c => c.Name).ToList();
            CategoryComboBox.ItemsSource = categories;

            // Загружаем бренды
            var brands = _db.Brands.OrderBy(b => b.Name).ToList();
            BrandComboBox.ItemsSource = brands;
            if (CurrentProduct.Id != 0)
            {
                CategoryComboBox.SelectedValue = CurrentProduct.CategoryId;
                BrandComboBox.SelectedValue = CurrentProduct.BrandId;
            }

            PriceTextBox.Text = CurrentProduct.Price.ToString();
            StockTextBox.Text = CurrentProduct.Stock.ToString();
            RatingTextBox.Text = CurrentProduct.Rating.ToString();

            // Загружаем теги
            var tags = _db.Tags.OrderBy(t => t.Name).ToList();
            TagsListBox.ItemsSource = tags;

            //  редактируем - выбираем привязанные теги
            if (CurrentProduct.Id != 0)
            {
                var productTagIds = _db.ProductTags
                    .Where(pt => pt.ProductId == CurrentProduct.Id)
                    .Select(pt => pt.TagId)
                    .ToList();

                // Очищаем текущий выбор и выбираем нужные
                TagsListBox.SelectedItems.Clear();
                foreach (var tag in tags)
                {
                    if (productTagIds.Contains(tag.Id))
                    {
                        TagsListBox.SelectedItems.Add(tag);
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name))
            {
                MessageBox.Show("Введите название товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentProduct.Description))
            {
                MessageBox.Show("Введите описание товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return;
            }
            string input = PriceTextBox.Text.Replace('.', ',');

            if (!decimal.TryParse(input, out var price))
            {
                MessageBox.Show("Цена должна быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }
            if (price < 0)
            {
                MessageBox.Show("Цена не может быть отрицательной", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }
            if (!int.TryParse(StockTextBox.Text, out var stock))
            {
                MessageBox.Show("Количество должно быть целым числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StockTextBox.Focus();
                return;
            }
            if (stock < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StockTextBox.Focus();
                return;
            }
            if (!double.TryParse(RatingTextBox.Text, out var rating))
            {
                MessageBox.Show("Рейтинг должен быть числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                RatingTextBox.Focus();
                return;
            }
            if (rating < 0 || rating > 5)
            {
                MessageBox.Show("Рейтинг должен быть от 0 до 5", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                RatingTextBox.Focus();
                return;
            }
            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BrandComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите бренд", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentProduct.Price = price;
            CurrentProduct.Stock = stock;
            CurrentProduct.Rating = rating;
            CurrentProduct.CategoryId = (int)CategoryComboBox.SelectedValue;
            CurrentProduct.BrandId = (int)BrandComboBox.SelectedValue;

            try
            {
                if (CurrentProduct.Id == 0)
                {

                    CurrentProduct.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

                    _db.Products.Add(CurrentProduct);
                    _db.SaveChanges(); // Сохраняем

                    // Добавляем связи с тегами
                    foreach (Tag selectedTag in TagsListBox.SelectedItems)
                    {
                        _db.ProductTags.Add(new ProductTag
                        {
                            ProductId = CurrentProduct.Id,
                            TagId = selectedTag.Id
                        });
                    }
                    _db.SaveChanges();

                    MessageBox.Show("Товар и теги успешно добавлены!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _db.Products.Update(CurrentProduct);

                    // Удаляем старые связи с тегами
                    var oldTags = _db.ProductTags.Where(pt => pt.ProductId == CurrentProduct.Id).ToList();
                    if (oldTags.Any())
                    {
                        _db.ProductTags.RemoveRange(oldTags);
                    }

                    // Добавляем новые связи с тегами
                    foreach (Tag selectedTag in TagsListBox.SelectedItems)
                    {
                        _db.ProductTags.Add(new ProductTag
                        {
                            ProductId = CurrentProduct.Id,
                            TagId = selectedTag.Id
                        });
                    }

                    _db.SaveChanges();

                    MessageBox.Show("Товар и теги успешно обновлены!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                    // После GoBack текущая страница уже не эта — Content у NavigationService
                    // может быть ещё не тот. Обновляем список через Frame главного окна на следующем кадре.
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (Application.Current.MainWindow is MainWindow mw
                            && mw.MainFrame.Content is ManagerPage mp)
                        {
                            mp.LoadList(null, null);
                        }
                    }), DispatcherPriority.Loaded);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
           
        NavigationService.GoBack();
            
        }
    }
}