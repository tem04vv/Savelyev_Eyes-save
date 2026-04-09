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
using System.Windows.Shapes;

namespace Savelyev_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для SalesHistoryWindow.xaml
    /// </summary>
    public partial class SalesHistoryWindow : Window
    {
        private int _agentId;
        public SalesHistoryWindow(int agentId)
        {
            InitializeComponent();
            _agentId = agentId;
            LoadProducts();
            LoadSales();
        }

        private void LoadProducts()
        {
            var products = SavelyevEntities.GetContext().Product.ToList();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.DisplayMemberPath = "Title";
            ProductComboBox.SelectedValuePath = "ID";
        }

        private void LoadSales()
        {
            var sales = SavelyevEntities.GetContext().ProductSale.Where(a => a.AgentID == _agentId).ToList();
            SalesListView.ItemsSource = sales;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (!int.TryParse(ProductCount.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            ProductSale newSale = new ProductSale
            {
                AgentID = _agentId,
                ProductID = (int)ProductComboBox.SelectedValue,
                SaleDate = SaleDatePicker.SelectedDate ?? DateTime.Today,
                ProductCount = count
            };

            SavelyevEntities.GetContext().ProductSale.Add(newSale);
            SavelyevEntities.GetContext().SaveChanges();

            LoadSales();

            ProductCount.Clear();
            SaleDatePicker.SelectedDate = DateTime.Today;
        }

        private void SaleDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ProductSale selectedSale = SalesListView.SelectedItem as ProductSale;

            if (selectedSale == null)
            {
                MessageBox.Show("Выберите продажу для удаления");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить выбранную продажу?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SavelyevEntities.GetContext().ProductSale.Remove(selectedSale);
                SavelyevEntities.GetContext().SaveChanges();

                LoadSales();

                MessageBox.Show("Продажа удалена");
            }
        }
    }
}
