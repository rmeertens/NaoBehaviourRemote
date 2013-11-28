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
using System.ComponentModel;

namespace NaoRemote
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void NoArgDelegate();
        private delegate void OneBooleanArgDelegate(bool arg);
        private delegate void OneStringArgDelegate(string arg);
        private delegate void BehaviorSequenceDelegate(BehaviorSequence arg);
        private delegate void BehaviorWaiterDelegate(int ID);
        private delegate void ConnectToNaoDelegate(string ip_address, int port);
        private TrialSequence sequence; 
 
        private TextToSpeechProxy TextToSpeechProxy;
        private BehaviorManagerProxy BehaviorManagerProxy;
        private LedsProxy LedsProxy;
        private VideoRecorderProxy VideoRecorderProxy;

        private BehaviorSequence currentSequence = BehaviorSequence.EmptyBehaviorSequence();
        private int SubjectNumber;

        public MainWindow()
        {
            InitializeComponent();
            sequence = TrialSequence.CreateEmptyTrialSequence();
            UpdateSequenceButtonContext();
            SetWozButtonsEnabled(false);
        }

        private void WaitForBehaviorToFinish(int ID)
        {
            bool continueWaiting = BehaviorManagerProxy.isRunning(ID);
            if(continueWaiting)
            {
                CurrentlyRunningLabel.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                    new BehaviorWaiterDelegate(WaitForBehaviorToFinish), ID);
            }
            else
            {
                BehaviorFinished();
            }
        }

        private void BehaviorButtonHandler(object sender, RoutedEventArgs e)
        {
            string behaviorName = TextBoxNaoBehaviorRoot.Text + (string)((Button)sender).Tag;
            if (BehaviorManagerProxy.isBehaviorPresent(behaviorName))
            {
                RunBehavior(behaviorName);
                if (!VideoRecorderProxy.isRecording())
                    StartVideoRecording();
            }
            else
            {
                MessageBox.Show("The behavior \"" + behaviorName + "\" was not located on Nao.",
                    "Unknown Behavior");
            }
        }

        private void StopButtonHandler(object sender, RoutedEventArgs e)
        {
            CurrentlyRunningLabel.Content = "Stopping all behaviors...";
            StopAllBehaviors();
        }

        private void BehaviorSequenceHandler(object sender, RoutedEventArgs e)
        {
            if(sequence.Count > 0) {
                currentSequence = sequence.Last();
                sequence.RemoveAt(sequence.Count -1);
                SequenceButton.Content = "Next Trial (" + sequence.Count + ")";
                SequenceButton.IsEnabled = false;
                RunBehaviorSequence();
            }
            else
                MessageBox.Show("All Trials Completed!!.",
                    "Trials Completed");
        }

        private void RunBehavior(string behaviorName)
        {
            CurrentlyRunningLabel.Content = "Currently Running: " + behaviorName;
            int ID = BehaviorManagerProxy.post.runBehavior(behaviorName);
            CurrentlyRunningLabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                new BehaviorWaiterDelegate(WaitForBehaviorToFinish), ID);
        }

        private void RunBehaviorSequence()
        {
            string behaviorToRun = currentSequence.First();
            currentSequence.Remove(behaviorToRun);
            RunBehavior(behaviorToRun);
        }

        private void BehaviorFinished()
        {
            if (currentSequence.Count > 0)
                RunBehaviorSequence();
            else
                UpdateUserInterfaceAfterBehaviorRun();
        }

        private void UpdateSequenceButtonContext()
        {
            SequenceButton.Content = "Next Trial (" + sequence.Count + ")";
        }

        private void UpdateUserInterfaceAfterBehaviorRun()
        {
            CurrentlyRunningLabel.Content = "Currently Running: None";
            if (currentSequence.Count == 0)
                SequenceButton.IsEnabled = true;
        }

        private void StopAllBehaviors()
        {
            try
            {
                int ID = BehaviorManagerProxy.post.stopAllBehaviors();
                UpdateUserInterfaceAfterBehaviorRun();
            }
            finally { }
        }

        private void NetworkSettingsUpdated(object sender, RoutedEventArgs e)
        {
            try
            {
                string nao_ip_address = TextBoxNaoIP.Text;
                int nao_port = Int32.Parse(TextBoxNaoPort.Text);
                ConnectButton.IsEnabled = false;
                ConnectButton.Content = "Connecting...";

                ConnectToNaoDelegate connector = new ConnectToNaoDelegate(this.ConnectToNao);
                connector.BeginInvoke(nao_ip_address, nao_port, null, null);
            }
            catch (FormatException)
            {
                //the field colors red magically
            }
        }

        private void StartVideoRecording()
        {
            VideoRecorderProxy.setFrameRate(Properties.Settings.Default.FrameRate);
            VideoRecorderProxy.setResolution(Properties.Settings.Default.VideoResolution);
            VideoRecorderProxy.setVideoFormat(Properties.Settings.Default.VideoFormat);
            VideoRecorderProxy.startRecording(Properties.Settings.Default.VideoDirectory,CreateVideoFileName());
        }

        private string CreateVideoFileName()
        {
            return Properties.Settings.Default.VideoFilePrefix + SubjectNumber + Properties.Settings.Default.VideoFileSuffix;
        }

        private void ConnectToNao(string nao_ip_address, int nao_port)
        {
            bool success = true;
            if (TextToSpeechProxy != null)
                TextToSpeechProxy.Dispose();
            if (BehaviorManagerProxy != null)
                BehaviorManagerProxy.Dispose();
            if (LedsProxy != null)
                LedsProxy.Dispose();
            if (VideoRecorderProxy != null)
            {
                if (VideoRecorderProxy.isRecording())
                    VideoRecorderProxy.stopRecording();
                VideoRecorderProxy.Dispose();
            }
            try
            {
                TextToSpeechProxy = new TextToSpeechProxy(nao_ip_address, nao_port);
                BehaviorManagerProxy = new BehaviorManagerProxy(nao_ip_address, nao_port);
                LedsProxy = new LedsProxy(nao_ip_address, nao_port);
                VideoRecorderProxy = new VideoRecorderProxy(nao_ip_address, nao_port);
            }
            catch (Exception)
            {
                success = false;
            }
            ConnectButton.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new OneBooleanArgDelegate(UpdateUserInterfaceAfterConnect), success);
        }

        private void UpdateUserInterfaceAfterConnect(bool success)
        {
            if (success)
                MessageBox.Show("You are now connected to Nao with IP-address: " + TextBoxNaoIP.Text +
                    " on port " + TextBoxNaoPort.Text + ".", "Successfully Connected");
            else
                MessageBox.Show("Could not connect to Nao  with IP-address: " + TextBoxNaoIP.Text +
                    " on port " + TextBoxNaoPort.Text + ".", "CONNECTION ERROR");
            ConnectButton.IsEnabled = true;
            ConnectButton.Content = "Connect";
            SetWozButtonsEnabled(true);
        }

        private void SetWozButtonsEnabled(bool enabled)
        {
            BehaviorButton1.IsEnabled = enabled;
            BehaviorButton2.IsEnabled = enabled;
            BehaviorButton3.IsEnabled = enabled;
            BehaviorButton4.IsEnabled = enabled;
            BehaviorButton5.IsEnabled = enabled;
            BehaviorButton6.IsEnabled = enabled;
            BehaviorButton7.IsEnabled = enabled;
            BehaviorButton8.IsEnabled = enabled;
            SequenceButton.IsEnabled = enabled;
            StopAllBehaviorsButton.IsEnabled = enabled;
            SayButton.IsEnabled = enabled;
        }

        private void InterfaceWindowClosing(object sender, CancelEventArgs e)
        {
            VideoRecorderProxy.stopRecording();
            Properties.Settings.Default.Save();
        }

        private void SayWords(object sender, RoutedEventArgs e)
        {
            this.TextToSpeechProxy.post.say(words_to_say.Text);
        }

        internal void SetSubjectNumber(int SubjectNumber)
        {
            this.SubjectNumber = SubjectNumber;
            if(SubjectNumber % 2 == 0) 
            {
                this.sequence = TrialSequence.CreatePredictiveTrialSequence();
            }
            else 
            {
                this.sequence = TrialSequence.CreateUnpredictiveTrialSequence();
            }
            UpdateSequenceButtonContext();
        }
    }
}
