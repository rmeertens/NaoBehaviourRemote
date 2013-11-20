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
        public SubjectNumberWindow()
        {
            InitializeComponent();
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            if (ValidSubjectNumber())
                StartApplications();
            else
                MessageBox.Show("The subject number you entered is not a number!", "Error");
        }

        private bool ValidSubjectNumber()
        {
            string subjectNumberContent = SubjectNumberInput.Text;
            try
            {
                int subjectNumber = Int32.Parse(subjectNumberContent);
                return true;
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        private void StartApplications()
        {
            this.Hide();
            new MainWindow().Show();
        }
    }
}
