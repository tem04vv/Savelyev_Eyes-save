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

namespace Savelyev_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для AgentsPage.xaml
    /// </summary>
    public partial class AgentsPage : Page
    {
        public AgentsPage()
        {
            InitializeComponent();

            SortComboBox.SelectedIndex = 0;
            FiltrationComboBox.SelectedIndex = 0;
            UpdateAgents();
        }
        private void UpdateAgents()
        {
            var currentAgent = SavelyevEntities.GetContext().Agent.ToList();

            if (SortComboBox.SelectedIndex == 1)
            {
                currentAgent = currentAgent.OrderBy(a => a.Title).ToList();
            }
            if (SortComboBox.SelectedIndex == 2)
            {
                currentAgent = currentAgent.OrderByDescending(a => a.Title).ToList();
            }
            if (SortComboBox.SelectedIndex == 3)
            {
                currentAgent = currentAgent.OrderBy(a => a.Discount).ToList();
            }
            if (SortComboBox.SelectedIndex == 4)
            {
                currentAgent = currentAgent.OrderByDescending(a => a.Discount).ToList();
            }
            if (SortComboBox.SelectedIndex == 5)
            {
                currentAgent = currentAgent.OrderBy(a => a.Priority).ToList();
            }
            if (SortComboBox.SelectedIndex == 6)
            {
                currentAgent = currentAgent.OrderByDescending(a => a.Priority).ToList();
            }

            if (FiltrationComboBox.SelectedIndex == 1)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "ЗАО").ToList();
            }
            if (FiltrationComboBox.SelectedIndex == 2)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "МКК").ToList();
            }
            if (FiltrationComboBox.SelectedIndex == 3)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "МФО").ToList();
            }
            if (FiltrationComboBox.SelectedIndex == 4)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "ОАО").ToList();
            }
            if (FiltrationComboBox.SelectedIndex == 5)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "ООО").ToList();
            }
            if (FiltrationComboBox.SelectedIndex == 6)
            {
                currentAgent = currentAgent.Where(a => a.AgentType.Title == "ПАО").ToList();
            }

            currentAgent = currentAgent
                .Where(a => 
                    a.Title.ToLower().Contains(SearchTextbox.Text.ToLower()) || 
                    a.Phone.Contains(SearchTextbox.Text) ||
                    a.Email.Contains(SearchTextbox.Text)
                    )
                .ToList();

            AgentsListView.ItemsSource = currentAgent.ToList();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void FiltrationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }
    }
}
