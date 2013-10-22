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
using Aldebaran.Proxies;


namespace CadeauThea
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly static string NAO_IP_ADDRESS = "10.0.1.8";
        private readonly static int NAO_PORT = 9559;
        private readonly static string ROOT_DIR = "contingency/";
        private TextToSpeechProxy TextToSpeechProxy;
        private BehaviorManagerProxy BehaviorManagerProxy;
        private LedsProxy LedsProxy;

        public MainWindow()
        {
            InitializeComponent();
            
            this.TextToSpeechProxy = new TextToSpeechProxy(NAO_IP_ADDRESS, NAO_PORT);
            this.BehaviorManagerProxy = new BehaviorManagerProxy(NAO_IP_ADDRESS, NAO_PORT);
            this.LedsProxy = new LedsProxy(NAO_IP_ADDRESS, NAO_PORT);
        }

        private void runBehavior(object sender, RoutedEventArgs e)
        {
            string behaviorName = ROOT_DIR + (string)((Button)sender).Tag;
            if (BehaviorManagerProxy.isBehaviorPresent(behaviorName))
            {
                BehaviorManagerProxy.post.runBehavior(ROOT_DIR + behaviorName);
            }
            else
            {
                MessageBox.Show("The behavior \"" + behaviorName + "\" was not located on Nao.","Unknown Behavior");
            }           
        }

        private void Stop_all(object sender, RoutedEventArgs e)
        {
            BehaviorManagerProxy.post.stopAllBehaviors();
        }
    }
}
