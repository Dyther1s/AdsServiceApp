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
    /// Логика взаимодействия для AdsPage.xaml
    /// </summary>
    public partial class AdsPage : Page
    {
        public AdsPage()
        {
            InitializeComponent();
            LoadActiveAds();
            LoadCities();
            LoadAdTypes();
            this.Loaded += AdsPage_Loaded;
        }

        private void AdsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadActiveAds();
        }

        private void LoadActiveAds()
        {
            try
            {
                using (var db = new Entities())
                {
                    var activeAds = db.Ads.Where(a => a.StatusID == 1)
                                          .Select(a => new
                                          {
                                              a.AdID,
                                              a.Title,
                                              a.Description,
                                              a.Price,
                                              a.PublishDate,
                                              a.Cities.CityName,
                                              a.Categories.CategoryName,
                                              a.AdTypes.TypeName,
                                              a.ad_image
                                          }).AsQueryable();

                    if (!string.IsNullOrEmpty(SearchBox.Text))
                    {
                        activeAds = activeAds.Where(a => a.Title.Contains(SearchBox.Text) || a.Description.Contains(SearchBox.Text));
                    }

                    if (CitySortComboBox.SelectedItem is ComboBoxItem cityItem && cityItem.Content.ToString() != "Сортировка по городу")
                    {
                        string cityName = cityItem.Content.ToString();
                        activeAds = activeAds.Where(a => a.CityName == cityName);
                    }

                    if (AdTypeSortComboBox.SelectedItem is ComboBoxItem adTypeItem && adTypeItem.Content.ToString() != "Сортировка по типу")
                    {
                        string adTypeName = adTypeItem.Content.ToString();
                        activeAds = activeAds.Where(a => a.TypeName == adTypeName);
                    }

                    AdsListView.ItemsSource = activeAds.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadActiveAds();
        }

        private void CitySortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadActiveAds();
        }

        private void AdTypeSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadActiveAds();
        }

        private void LoadCities()
        {
            try
            {
                using (var db = new Entities())
                {
                    CitySortComboBox.Items.Add(new ComboBoxItem { Content = "Сортировка по городу" });

                    var cities = db.Cities.Select(c => c.CityName).ToList();
                    foreach (var city in cities)
                    {
                        CitySortComboBox.Items.Add(new ComboBoxItem { Content = city });
                    }

                    CitySortComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки городов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAdTypes()
        {
            try
            {
                using (var db = new Entities())
                {
                    AdTypeSortComboBox.Items.Add(new ComboBoxItem { Content = "Сортировка по типу" });

                    var adTypes = db.AdTypes.Select(at => at.TypeName).ToList();
                    foreach (var adType in adTypes)
                    {
                        AdTypeSortComboBox.Items.Add(new ComboBoxItem { Content = adType });
                    }

                    AdTypeSortComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
