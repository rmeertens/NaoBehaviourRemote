using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aldebaran.Proxies;
using System.Windows.Forms;


namespace NaoCam
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string ip = "10.0.1.2";
            int port = 9559;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
/*            System.Console.WriteLine("Hoi1");
            VisionToolboxProxy tool = new VisionToolboxProxy(ip, port);
            VideoDeviceProxy vid = new VideoDeviceProxy(ip, port);
            tool.
            System.Console.WriteLine("Hoi");
            System.Console.WriteLine(vid.getActiveCamera());
  */
        }
    }
}
