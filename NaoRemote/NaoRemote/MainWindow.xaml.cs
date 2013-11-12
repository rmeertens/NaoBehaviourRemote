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
using System.Windows.Threading;
using System.Threading;

using Aldebaran.Proxies;


namespace CadeauThea
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void NoArgDelegate();
        private delegate void OneArgDelegate(bool arg);
        private const string IP_ADDRESS_TEXT_BOX = "nao_ip";
        private const string PORT_TEXT_BOX = "nao_port";
        private const string ROOT_DIRECTORY_TEXT_BOX = "nao_root_directory";
        private string nao_ip_address;
        private int nao_port;
        private string nao_behavior_root_dir;
        private TextToSpeechProxy TextToSpeechProxy;
        private BehaviorManagerProxy BehaviorManagerProxy;
        private LedsProxy LedsProxy;

        public MainWindow()
        {
            InitializeComponent();
            nao_ip_address = TextBoxNaoIP.Text;
            nao_port = (int)Int32.Parse(TextBoxNaoPort.Text);
            nao_behavior_root_dir = TextBoxNaoBehaviorRoot.Text;
        }

        private void runBehavior(object sender, RoutedEventArgs e)
        {
            string behaviorName = nao_behavior_root_dir + (string)((Button)sender).Tag;
            if (BehaviorManagerProxy.isBehaviorPresent(behaviorName))
            {
                int postID = BehaviorManagerProxy.post.runBehavior(behaviorName);
                CurrentlyRunningLabel.Content = "Currently Running: " + behaviorName;
                //TODO: Move this to a separate thread to keep UI responsive
                //BehaviorManagerProxy.wait(postID, 0);
                //CurrentlyRunningLabel.Content = "Currently Running: None";
            }
            else
            {
                MessageBox.Show("The behavior \"" + behaviorName + "\" was not located on Nao.",
                    "Unknown Behavior");
            }           
        }

        private void Stop_all(object sender, RoutedEventArgs e)
        {
            BehaviorManagerProxy.post.stopAllBehaviors();
        }

        private void NetworkSettingsUpdated()
        {
            ConnectButton.IsEnabled = false;
            ConnectButton.Content = "Connecting...";
            NoArgDelegate connector = new NoArgDelegate(this.ConnectToNao);
            connector.BeginInvoke(null, null);
        }

        private void ConnectToNao()
        {
            bool success = true;
            if (TextToSpeechProxy != null)
                TextToSpeechProxy.Dispose();
            if (BehaviorManagerProxy != null)
                BehaviorManagerProxy.Dispose();
            if (LedsProxy != null)
                LedsProxy.Dispose();
            try
            {
                TextToSpeechProxy = new TextToSpeechProxy(nao_ip_address, nao_port);
                BehaviorManagerProxy = new BehaviorManagerProxy(nao_ip_address, nao_port);
                LedsProxy = new LedsProxy(nao_ip_address, nao_port);
            }
            catch (Exception e)
            {
                success = false;
            }
            ConnectButton.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new OneArgDelegate(UpdateUserInterfaceAfterConnect), success);
        }

        private void UpdateUserInterfaceAfterConnect(bool success)
        {
            if (success)
                MessageBox.Show("You are now connected to Nao with IP-address: " + nao_ip_address +
                    " on port " + nao_port + ".", "Successfully Connected");
            else
                MessageBox.Show("Could not connect to Nao  with IP-address: " + nao_ip_address +
                    " on port " + nao_port + ".", "CONNECTION ERROR");
            ConnectButton.IsEnabled = true;
            ConnectButton.Content = "Connect";

        }

        private void UpdateNetworkSettings(object sender, RoutedEventArgs e)
        {
            TextBox updatedTextBox = (TextBox)sender;
            string updatedTextBoxName = (string)updatedTextBox.Tag;
            string newValue = (string)updatedTextBox.Text;
            switch (updatedTextBoxName)
            {
                case IP_ADDRESS_TEXT_BOX:
                    if (!newValue.Equals(nao_ip_address))
                    {
                        nao_ip_address = newValue;
                    }
                    break;
                case PORT_TEXT_BOX:
                    int portnr = (int)Int32.Parse(newValue);
                    if (nao_port != portnr)
                    {
                        nao_port = portnr;
                    }
                    break;
                case ROOT_DIRECTORY_TEXT_BOX: 
                    nao_behavior_root_dir = newValue;
                    break;
                default: MessageBox.Show("Unknown value edited, this should not happen.", "Interface Error");
                    break;
            }
        }

        private void ConnectToNao(object sender, RoutedEventArgs e) 
        {
            NetworkSettingsUpdated();
        }

        private void SayWords(object sender, RoutedEventArgs e)
        {
            this.TextToSpeechProxy.post.say(words_to_say.Text);
        }
    }
}
