using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aldebaran.Proxies;
using System.Collections;

namespace NaoCam
{

    public struct NaoCamImageFormat
    {
        public string name;
        public int id;
        public int width;
        public int height;
    }

public class Camera
    {
        private VideoDeviceProxy _videoDeviceProxy = null;
        string ip;

        public List<NaoCamImageFormat> NaoCamImageFormats = new List<NaoCamImageFormat>();

        public Camera(string ip)
        {
            this.ip = ip;
            NaoCamImageFormat format0 = new NaoCamImageFormat();
            format0.name = "160 * 120";
            format0.id = 0;
            format0.width = 160;
            format0.height = 120;

            NaoCamImageFormat format1 = new NaoCamImageFormat();
            format1.name = "320 * 240";
            format1.id = 1;
            format1.width = 320;
            format1.height = 240;

            NaoCamImageFormat format2 = new NaoCamImageFormat();
            format2.name = "640 * 480";
            format2.id = 2;
            format2.width = 640;
            format2.height = 480;

            NaoCamImageFormats.Add(format0);
            NaoCamImageFormats.Add(format1);
            NaoCamImageFormats.Add(format2);
        }

        public void Connect(string ip)
        {
            try
            {
                if (_videoDeviceProxy != null)
                {
                    Disconnect();
                }
                _videoDeviceProxy = new VideoDeviceProxy(ip, 9559);
                try
                {
                    // good practice to unsubscribe before subscribing,
                    // just in case we didn't unsubscribe last time
                    _videoDeviceProxy.unsubscribe("NaoCam");
                }
                catch (Exception)
                {
                }
                _videoDeviceProxy.subscribe("NaoCam", 1, 13, 120);
            } 
            catch(Exception e)
            {
                _videoDeviceProxy = null;
                Console.Out.WriteLine("Camera.Connect exception: " + e);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_videoDeviceProxy != null)
                {
                    _videoDeviceProxy.unsubscribe("NaoCam");
                    _videoDeviceProxy = null;
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Camera.Disconnect exception: " + e);
            }
        }

        public byte[] GetImage()
        {
            byte[] imageBytes = new byte[0];
    
            try
            {
                if (_videoDeviceProxy != null)
                {
                    //System.Console.WriteLine("Getting image");
                    Object imageObject = _videoDeviceProxy.getImageRemote("NaoCam");
                    imageBytes = (byte[]) ((ArrayList) imageObject)[6];
                   /* for (int i = 0 ; i<6 ; i++)
                        System.Console.WriteLine(imageBytes[i]);
                    * */
                }
            } 
            catch(Exception e)
            {
                Console.Out.WriteLine("Camera.GetImage exception: " + e);
            }
            return imageBytes;
        }

    }
}
