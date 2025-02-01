using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdsServiceApp.pages
{
    public partial class UserPage : Page
    {
        private readonly Entities db;
        private readonly int _UserID;

        public UserPage(int UserID)
        {
            InitializeComponent();
            db = new Entities();
            _UserID = UserID;
            LoadActiveAds();
            LoadCompletedAds();
            this.Loaded += UserPage_Loaded;
            ActiveAdsListView.MouseDoubleClick += ActiveAdsListView_MouseDoubleClick;
        }

        private void UserPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadActiveAds();
            LoadCompletedAds();
        }

        private void LoadActiveAds()
        {
            try
            {
                var activeAds = db.Ads
                    .Where(a => a.UserID == _UserID && a.CompletionDate == null)
                    .Select(a => new
                    {
                        a.AdID,
                        a.Title,
                        a.Description,
                        a.Price,
                        a.PublishDate,
                        CityName = a.Cities.CityName,
                        CategoryName = a.Categories.CategoryName,
                        TypeName = a.AdTypes.TypeName,
                        a.ad_image
                    })
                    .ToList();

                ActiveAdsListView.ItemsSource = activeAds;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки активных объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCompletedAds()
        {
            try
            {
                var completedAds = db.Ads
                    .Where(a => a.UserID == _UserID && a.CompletionDate != null)
                    .Select(a => new
                    {
                        a.AdID,
                        a.Title,
                        a.Description,
                        a.Price,
                        a.CompletionDate,
                        a.ad_image
                    })
                    .ToList();

                CompletedAdsListView.ItemsSource = completedAds;

                decimal totalProfit = completedAds.Sum(a => a.Price);
                TotalProfitTextBlock.Text = $"Общая прибыль: {totalProfit:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки завершенных объявлений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActiveAdsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ActiveAdsListView.SelectedItem != null)
            {
                dynamic selectedAd = ActiveAdsListView.SelectedItem;
                NavigationService.Navigate(new EditAdPage(selectedAd.AdID));
            }
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddAdPage());
        }

        private void CompleteAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAdsListView.SelectedItem != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите завершить объявление?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        dynamic selectedAd = ActiveAdsListView.SelectedItem;
                        int adId = selectedAd.AdID;

                        var adToComplete = db.Ads.FirstOrDefault(a => a.AdID == adId);
                        if (adToComplete != null)
                        {
                            adToComplete.CompletionDate = DateTime.Now;
                            adToComplete.StatusID = 4;

                            db.SaveChanges();

                            var user = db.Users.FirstOrDefault(u => u.UserID == _UserID);
                            if (user != null)
                            {
                                user.Profit += adToComplete.Price;
                                db.SaveChanges();
                            }

                            LoadActiveAds();
                            LoadCompletedAds();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка завершения объявления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите активное объявление для завершения.");
            }
        }

        private void DeleteAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAdsListView.SelectedItem != null)
            {
                if (MessageBox.Show("Вы точно хотите удалить выбранное объявление?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        dynamic selectedAd = ActiveAdsListView.SelectedItem;
                        int adId = selectedAd.AdID;

                        var adToDelete = db.Ads.FirstOrDefault(a => a.AdID == adId);
                        if (adToDelete != null)
                        {
                            db.Ads.Remove(adToDelete);
                            db.SaveChanges();
                            LoadActiveAds();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления объявления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объявление для удаления.");
            }
        }
    }
}
