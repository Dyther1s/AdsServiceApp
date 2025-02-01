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
    /// Логика взаимодействия для EditAdPage.xaml
    /// </summary>
    public partial class EditAdPage : Page
    {
        private int _adId;

        public EditAdPage(int adId)
        {
            InitializeComponent();
            _adId = adId;
            LoadAdData();
            LoadCities();
            LoadCategories();
            LoadAdTypes();
        }

        private void LoadAdData()
        {
            try
            {
                using (var db = new Entities())
                {
                    var ad = db.Ads.Single(a => a.AdID == _adId);

                    TitleTextBox.Text = ad.Title;
                    DescriptionTextBox.Text = ad.Description;
                    PriceTextBox.Text = ad.Price.ToString();

                    var city = db.Cities.Single(c => c.CityID == ad.CityID);
                    CityComboBox.SelectedItem = city.CityName;

                    var category = db.Categories.Single(c => c.CategoryID == ad.CategoryID);
                    CategoryComboBox.SelectedItem = category.CategoryName;

                    var adType = db.AdTypes.Single(at => at.AdTypeID == ad.AdTypeID);
                    AdTypeComboBox.SelectedItem = adType.TypeName;

                    ImagePathTextBox.Text = ad.ad_image ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки объявления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void SaveAdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                    {
                        MessageBox.Show("Пожалуйста, введите корректную стоимость.");
                        return;
                    }

                    var ad = db.Ads.Single(a => a.AdID == _adId);
                    string selectedCity = (CityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    string selectedcategory = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    string selectedtype = (AdTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    var imagePath = ImagePathTextBox.Text;

                    if (selectedCity == null || selectedcategory == null || selectedtype == null || string.IsNullOrEmpty(imagePath))
                    {
                        MessageBox.Show("Пожалуйста, выберите все необходимые параметры и укажите путь к изображению.");
                        return;
                    }

                    ad.Title = TitleTextBox.Text;
                    ad.Description = DescriptionTextBox.Text;
                    ad.Price = price;
                    ad.CityID = db.Cities.Single(c => c.CityName == selectedCity).CityID;
                    ad.CategoryID = db.Categories.Single(c => c.CategoryName == selectedcategory).CategoryID;
                    ad.AdTypeID = db.AdTypes.Single(at => at.TypeName == selectedtype).AdTypeID;

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        ad.ad_image = imagePath;
                    }

                    db.SaveChanges();
                }
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения объявления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
