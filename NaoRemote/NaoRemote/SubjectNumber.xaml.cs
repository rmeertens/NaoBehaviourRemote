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

namespace NaoRemote
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SubjectNumberWindow : Window
    {
        private int SubjectNumber;

        public SubjectNumberWindow()
        {
            InitializeComponent();
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            if (ValidSubjectNumber())
                StartApplications();
        }

        private bool ValidSubjectNumber()
        {
            string subjectNumberContent = SubjectNumberInput.Text;
            try
            {
                SubjectNumber = Int32.Parse(subjectNumberContent);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void StartApplications()
        {
            this.Hide();
            this.StartMainWindow();
        }

        private void StartMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.SetSubjectNumber(SubjectNumber);
            mainWindow.Show();
        }
    }
}
