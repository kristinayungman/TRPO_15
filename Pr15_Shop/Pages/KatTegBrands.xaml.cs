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
using Pr15_Shop.Models;
using Pr15_Shop.Service;

namespace Pr15_Shop.Pages
{
    /// <summary>
    /// Логика взаимодействия для KatTegBrands.xaml
    /// </summary>
    public partial class KatTegBrands : Page
    {
        private readonly DBService _dbService;
        private List<Brand> _brandsList;
        private List<Category> _categoriesList;
        private List<Tag> _tagsList;
        public KatTegBrands()
        {
            InitializeComponent();
            _dbService = DBService.Instance;
            LoadData();
        }
        private void LoadData()
        {
            // Загрузка Брендов
            _brandsList = _dbService.Context.Brands.ToList();
            dgBrands.ItemsSource = _brandsList;

            // Загрузка Категорий
            _categoriesList = _dbService.Context.Categories.ToList();
            dgCategories.ItemsSource = _categoriesList;

            // Загрузка Тегов
            _tagsList = _dbService.Context.Tags.ToList();
            dgTags.ItemsSource = _tagsList;
        }

        private void dgBrands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBrands.SelectedItem is Brand selected)
            {
                txtBrandName.Text = selected.Name;
            }
            else
            {
                txtBrandName.Text = string.Empty;
            }
        }

        private void btnBrandAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBrandName.Text))
                {
                    MessageBox.Show("Введите название бренда");
                    return;
                }

                var newBrand = new Brand { Name = txtBrandName.Text };
                _dbService.Context.Brands.Add(newBrand);
                _dbService.Context.SaveChanges();

                _brandsList.Add(newBrand);
                dgBrands.Items.Refresh();
                txtBrandName.Clear();
                MessageBox.Show("Бренд добавлен");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Выводим сообщение из внутреннего исключения
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при сохранении: {errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Ловим любые другие непредвиденные ошибки
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBrandUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgBrands.SelectedItem is not Brand selected)
            {
                MessageBox.Show("Выберите бренд для редактирования");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                MessageBox.Show("Название не может быть пустым");
                return;
            }

            selected.Name = txtBrandName.Text;
            _dbService.Context.SaveChanges();

            dgBrands.Items.Refresh();
            MessageBox.Show("Бренд обновлен");
        }

        private void btnBrandDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgBrands.SelectedItem is not Brand selected)
            {
                MessageBox.Show("Выберите бренд для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить бренд '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _dbService.Context.Brands.Remove(selected);
                _dbService.Context.SaveChanges();

                _brandsList.Remove(selected);
                dgBrands.Items.Refresh();
                txtBrandName.Clear();
            }
        }
        private void dgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCategories.SelectedItem is Category selected)
                txtCategoryName.Text = selected.Name;
            else
                txtCategoryName.Text = string.Empty;
        }

        private void btnCatAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text)) { MessageBox.Show("Введите название"); return; }

            var newItem = new Category { Name = txtCategoryName.Text };
            _dbService.Context.Categories.Add(newItem);
            _dbService.Context.SaveChanges();

            _categoriesList.Add(newItem);
            dgCategories.Items.Refresh();
            txtCategoryName.Clear();
        }

        private void btnCatUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem is not Category selected) { MessageBox.Show("Выберите категорию"); return; }
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text)) { MessageBox.Show("Название не может быть пустым"); return; }

            selected.Name = txtCategoryName.Text;
            _dbService.Context.SaveChanges();
            dgCategories.Items.Refresh();
        }

        private void btnCatDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem is not Category selected) { MessageBox.Show("Выберите категорию"); return; }

            if (MessageBox.Show($"Удалить '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _dbService.Context.Categories.Remove(selected);
                _dbService.Context.SaveChanges();
                _categoriesList.Remove(selected);
                dgCategories.Items.Refresh();
                txtCategoryName.Clear();
            }
        }
        private void dgTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTags.SelectedItem is Tag selected)
                txtTagName.Text = selected.Name;
            else
                txtTagName.Text = string.Empty;
        }

        private void btnTagAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTagName.Text)) { MessageBox.Show("Введите название"); return; }

            var newItem = new Tag { Name = txtTagName.Text };
            _dbService.Context.Tags.Add(newItem);
            _dbService.Context.SaveChanges();

            _tagsList.Add(newItem);
            dgTags.Items.Refresh();
            txtTagName.Clear();
        }

        private void btnTagUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgTags.SelectedItem is not Tag selected) { MessageBox.Show("Выберите тег"); return; }
            if (string.IsNullOrWhiteSpace(txtTagName.Text)) { MessageBox.Show("Название не может быть пустым"); return; }

            selected.Name = txtTagName.Text;
            _dbService.Context.SaveChanges();
            dgTags.Items.Refresh();
        }

        private void btnTagDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTags.SelectedItem is not Tag selected) { MessageBox.Show("Выберите тег"); return; }

            if (MessageBox.Show($"Удалить '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _dbService.Context.Tags.Remove(selected);
                _dbService.Context.SaveChanges();
                _tagsList.Remove(selected);
                dgTags.Items.Refresh();
                txtTagName.Clear();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
