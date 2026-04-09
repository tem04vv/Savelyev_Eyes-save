using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private int _editingAgentID;
        private string _newLogoPath;
        public AddEditPage(int agentID)
        {
            InitializeComponent();
            _editingAgentID = agentID;
            LoadAgentData();
        }

        private void LoadAgentData()
        {
            var types = SavelyevEntities.GetContext().AgentType.ToList();
            AgentComboType.ItemsSource = types;
            AgentComboType.DisplayMemberPath = "Title";
            AgentComboType.SelectedValuePath = "ID";

            if (_editingAgentID == 0)
            {
                return;
            }

            var currentAgent = SavelyevEntities.GetContext().Agent.FirstOrDefault(a => a.ID == _editingAgentID);

            if (currentAgent != null)
            {
                TitleTB.Text = currentAgent.Title;
                AddressTB.Text = currentAgent.Address;
                DirectorTB.Text = currentAgent.DirectorName;
                PriorityTB.Text = currentAgent.Priority.ToString();
                INN_TB.Text = currentAgent.INN;
                KPP_TB.Text = currentAgent.KPP;
                Phone_TB.Text = currentAgent.Phone;
                EmailTB.Text = currentAgent.Email;

                AgentComboType.SelectedValue = currentAgent.AgentTypeID;

                if (!string.IsNullOrEmpty(currentAgent.Logo))
                    LogoImage.Source = new BitmapImage(new Uri(currentAgent.Logo, UriKind.Relative));
            }
        }

        private void ChangePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.png;*.jpg";

            if (dlg.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(dlg.FileName);

                string agentsFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agents");
                System.IO.Directory.CreateDirectory(agentsFolder);
                string fullPath = System.IO.Path.Combine(agentsFolder, fileName);

                System.IO.File.Copy(dlg.FileName, fullPath, true);

                _newLogoPath = "\\agents\\" + fileName;

                LogoImage.Source = new BitmapImage(new Uri(fullPath));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Agent currentAgent;

            if (_editingAgentID == 0)
            {
                currentAgent = new Agent();
            }
            else
            {
                currentAgent = SavelyevEntities.GetContext().Agent.FirstOrDefault(a => a.ID == _editingAgentID);
                if (currentAgent == null)
                {
                    MessageBox.Show("Агент не найден");
                    return;
                }
            }

            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TitleTB.Text))
                errors.AppendLine("Укажите наименование агента");

            if (string.IsNullOrWhiteSpace(AddressTB.Text))
                errors.AppendLine("Укажите адрес агента");

            if (string.IsNullOrWhiteSpace(DirectorTB.Text))
                errors.AppendLine("Укажите ФИО директора");

            if (AgentComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");

            int priority;
            if (!int.TryParse(PriorityTB.Text, out priority))
            {
                errors.AppendLine("Приоритет должен быть числом");
            }
            else if (priority <= 0)
            {
                errors.AppendLine("Приоритет должен быть положительным числом");
            }

            if (string.IsNullOrWhiteSpace(INN_TB.Text))
                errors.AppendLine("Укажите ИНН агента");

            if (string.IsNullOrWhiteSpace(KPP_TB.Text))
                errors.AppendLine("Укажите КПП агента");

            if (string.IsNullOrWhiteSpace(Phone_TB.Text))
            {
                errors.AppendLine("Укажите телефон агента");
            }
            else
            {
                string ph = Phone_TB.Text.Replace("(", "").Replace("-", "").Replace("+", "");
                if (ph.Length < 2)
                    errors.AppendLine("Укажите правильно телефон агента");
                else if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }

            if (string.IsNullOrWhiteSpace(EmailTB.Text))
                errors.AppendLine("Укажите почту агента");

            if (!string.IsNullOrEmpty(_newLogoPath))
            {
                currentAgent.Logo = _newLogoPath;
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }


            currentAgent.Title = TitleTB.Text;
            currentAgent.Address = AddressTB.Text;
            currentAgent.DirectorName = DirectorTB.Text;
            currentAgent.Priority = priority;
            currentAgent.INN = INN_TB.Text;
            currentAgent.KPP = KPP_TB.Text;
            currentAgent.Phone = Phone_TB.Text;
            currentAgent.Email = EmailTB.Text;
            currentAgent.AgentTypeID = (int)AgentComboType.SelectedValue;

            if (_editingAgentID == 0)
            {
                SavelyevEntities.GetContext().Agent.Add(currentAgent);
            }

            try
            {
                SavelyevEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.Navigate(new AgentsPage());
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    MessageBox.Show(ex.InnerException.Message);
                else
                    MessageBox.Show(ex.Message);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentAgent = SavelyevEntities.GetContext().Agent.FirstOrDefault(a => a.ID == _editingAgentID);

            if (currentAgent == null)
            {
                MessageBox.Show("Агент не найден!");
                return;
            }

            if (SavelyevEntities.GetContext().ProductSale.Any(ps => ps.AgentID == currentAgent.ID))
            {
                MessageBox.Show("Невозможно удалить агента, так как у него есть информация о реализованной продукции.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить агента?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var shops = SavelyevEntities.GetContext().Shop.Where(s => s.AgentID == currentAgent.ID).ToList();
                SavelyevEntities.GetContext().Shop.RemoveRange(shops);

                var histories = SavelyevEntities.GetContext().AgentPriorityHistory.Where(h => h.AgentID == currentAgent.ID).ToList();
                SavelyevEntities.GetContext().AgentPriorityHistory.RemoveRange(histories);

                SavelyevEntities.GetContext().Agent.Remove(currentAgent);

                SavelyevEntities.GetContext().SaveChanges();
                Manager.MainFrame.Navigate(new AgentsPage());
            }
        }
        private void SalesHistory_Click(object sender, RoutedEventArgs e)
        {
            SalesHistoryWindow window = new SalesHistoryWindow(_editingAgentID);
            window.Show();
        }

        
    }
}
