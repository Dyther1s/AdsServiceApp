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

namespace AdsServiceApp.pages
{
    /// <summary>
    /// Логика взаимодействия для AddAdPage.xaml
    /// </summary>
    public partial class AddAdPage : Page
    {
        public AddAdPage()
        {
            InitializeComponent();
            LoadCities();
            LoadCategories();
            LoadAdTypes();
        }

        private void LoadCities()
        {
            try
            {
                using (var db = new Entities())
                {
                    var cities = db.Cities.Select(c => c.CityName).ToList();
                    foreach (var city in cities)
                    {
                        CityComboBox.Items.Add(new ComboBoxItem { Content = city });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки городов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Entities())
                {
                    var categories = db.Categories.Select(c => c.CategoryName).ToList();
                    foreach (var category in categories)
                    {
                        CategoryComboBox.Items.Add(new ComboBoxItem { Content = category });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAdTypes()
        {
            try
            {
                using (var db = new Entities())
                {
                    var adTypes = db.AdTypes.Select(at => at.TypeName).ToList();
                    foreach (var adType in adTypes)
                    {
                        AdTypeComboBox.Items.Add(new ComboBoxItem { Content = adType });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    var cityName = (CityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    var categoryName = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    var adTypeName = (AdTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    var imagePath = ImagePathTextBox.Text;

                    if (cityName == null || categoryName == null || adTypeName == null || string.IsNullOrEmpty(imagePath))
                    {
                        MessageBox.Show("Пожалуйста, выберите все необходимые параметры и укажите путь к изображению.");
                        return;
                    }

                    if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                    {
                        MessageBox.Show("Пожалуйста, введите корректную стоимость.");
                        return;
                    }

                    var newAd = new Ads
                    {
                        Title = TitleTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        Price = price,
                        PublishDate = DateTime.Now,
                        UserID = 1,
                        CityID = db.Cities.Single(c => c.CityName == cityName).CityID,
                        CategoryID = db.Categories.Single(c => c.CategoryName == categoryName).CategoryID,
                        AdTypeID = db.AdTypes.Single(at => at.TypeName == adTypeName).AdTypeID,
                        StatusID = 1,
                        ad_image = imagePath
                    };

                    db.Ads.Add(newAd);
                    db.SaveChanges();
                }
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления объявления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
