using Microsoft.EntityFrameworkCore;
using Pr15_Shop.Models;
using Pr15_Shop.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Pr15_Shop.Pages
{
    /// <summary>
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        public Pr15ShopContext db = DBService.Instance.Context;
        public int? selectedBrandId { get; set; } = null;
        public int? selectedCategoryId { get; set; } = null;

        public ObservableCollection<Category> categories { get; set; } = new();

        public ObservableCollection<Brand> brands { get; set; } = new();

        public ObservableCollection<Product> products { get; set; } = new();

        public ObservableCollection<Tag> tags { get; set; } = new();

        public ObservableCollection<ProductTag> product_tags { get; set; } = new();
        public ICollectionView formsView { get; set; }

        public string searchQuery { get; set; } = null!;

        public string filterPriceFrom { get; set; } = null!;
        public string filterPriceTo { get; set; } = null!;
        public ManagerPage()
        {

            formsView = CollectionViewSource.GetDefaultView(products);
            formsView.Filter = FilterForms;
            InitializeComponent();
            DataContext = this;
            LoadList(null, null);
            LoadFilters();
        }
            private void LoadFilters()
            {
                // Загружаем бренды
                var brandList = db.Brands.ToList();
                foreach (var brand in brandList)
                {
                    BrandFilterComboBox.Items.Add(brand);
                }
                // Загружаем категории
                var categoryList = db.Categories.ToList();
                foreach (var category in categoryList)
                {
                    CategoryFilterComboBox.Items.Add(category);
                }
            }
            public void LoadList(object sender, EventArgs e)
            {
                products.Clear();
                var productsList = db.Products.Include(p => p.Category).ToList();
                var productIds = productsList.Select(p => p.Id).ToList();
                var productTags = db.ProductTags
                    .Where(pt => productIds.Contains(pt.ProductId))
                    .Join(db.Tags, pt => pt.TagId, t => t.Id, (pt, t) => new { pt.ProductId, TagName = t.Name })
                    .ToList();

                foreach (var form in productsList)
                {
                    form.DisplayTags = string.Join(" ", productTags
                        .Where(x => x.ProductId == form.Id)
                        .Select(x => "#" + x.TagName));
                    products.Add(form);
                }
            }
            private void BrandFilter(object sender, SelectionChangedEventArgs e)
            {
                selectedBrandId = (BrandFilterComboBox.SelectedItem as Brand)?.Id ?? 0;
                formsView.Refresh();
            }
            private void CategoryFilter(object sender, SelectionChangedEventArgs e)
            {
                selectedCategoryId = (CategoryFilterComboBox.SelectedItem as Category)?.Id ?? 0;
                formsView.Refresh();
            }
            public bool FilterForms(object obj)
            {
                if (obj is not Product)
                    return false;
                var form = (Product)obj;
                if (searchQuery != null && !form.Name.Contains(searchQuery,
             StringComparison.CurrentCultureIgnoreCase))
                    return false;
                if (!string.IsNullOrEmpty(filterPriceFrom) && decimal.TryParse(filterPriceFrom, out var minPrice) && form.Price < minPrice)
                    return false;
                if (!string.IsNullOrEmpty(filterPriceTo) && decimal.TryParse(filterPriceTo, out var maxPrice) && form.Price > maxPrice)
                    return false;
                if (selectedBrandId > 0 && form.BrandId != selectedBrandId)
                    return false;
                if (selectedCategoryId > 0 && form.CategoryId != selectedCategoryId)
                    return false;
                return true;
            }
            private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            {
                formsView.Refresh();
            }
            private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                formsView.SortDescriptions.Clear();
                var cb = (ComboBox)sender;
                var selected = (ComboBoxItem)cb.SelectedItem;
                switch (selected.Tag)
                {
                    case "Name":
                        formsView.SortDescriptions.Add(new SortDescription("Name",
                        ListSortDirection.Ascending));
                        break;
                    case "PriceUp":
                        formsView.SortDescriptions.Add(new SortDescription("Price",
                        ListSortDirection.Ascending));
                        break;
                    case "PriceDown":
                        formsView.SortDescriptions.Add(new SortDescription("Price",
                        ListSortDirection.Descending));
                        break;
                    case "StockUp":
                        formsView.SortDescriptions.Add(new SortDescription("Stock",
                        ListSortDirection.Ascending));
                        break;
                    case "StockDown":
                        formsView.SortDescriptions.Add(new SortDescription("Stock",
                        ListSortDirection.Descending));
                        break;
                }
                formsView.Refresh();
            }
            private void Clear(object sender, RoutedEventArgs e)
            {
                searchQuery = string.Empty;
                filterPriceFrom = string.Empty;
                filterPriceTo = string.Empty;
                selectedBrandId = null;
                selectedCategoryId = null;
                BrandFilterComboBox.SelectedIndex = 0;
                CategoryFilterComboBox.SelectedIndex = 0;
                formsView.SortDescriptions.Clear();
                formsView.Refresh();
            }

        private void Add_Prod(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage());
        }

        private void Delete_Prod(object sender, RoutedEventArgs e)
        {
            if (FormsList.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Выберите товар для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить товар \"{selectedProduct.Name}\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                //Находим все связи
                var relatedTags = DBService.Instance.Context.ProductTags
                                            .Where(pt => pt.ProductId == selectedProduct.Id)
                                            .ToList();
                //Если связи есть, удаляем их первыми
                if (relatedTags.Any())
                {
                    DBService.Instance.Context.ProductTags.RemoveRange(relatedTags);
                }
                //Удаляем сам товар
                DBService.Instance.Context.Products.Remove(selectedProduct);
                //Сохраняем изменения
                DBService.Instance.Context.SaveChanges();
                // Удаляем из списка
                products.Remove(selectedProduct);
                // Сбрасываем выделение
                FormsList.SelectedItem = null;
                MessageBox.Show("Товар успешно удален", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KatTegBrands());
        }
        private void Edit_Prod(object sender, RoutedEventArgs e)
        {
            // 1. Получаем выбранный товар из ListView (так же, как в Delete_Prod)
            if (FormsList.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Выберите товар для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Переходим на страницу редактирования, передавая товар
            NavigationService.Navigate(new AddProductPage(selectedProduct));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
