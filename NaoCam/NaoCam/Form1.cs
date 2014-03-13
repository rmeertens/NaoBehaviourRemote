using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NaoCam
{
    public partial class Form1 : Form
    {

        private string ip = "10.0.1.2";
        private int port = 5994;
        private Camera _cam;
        private Bitmap _naoCamBitmap;

        public Form1()
        {
            InitializeComponent();
            _cam = new Camera(ip);
            _cam.Connect(ip);
            _naoCamBitmap = new Bitmap(320 , 240, PixelFormat.Format24bppRgb);
            timerUpdate.Tick += new EventHandler(UpdateScreen);
            timerUpdate.Interval = (int)Math.Ceiling(1000.0 / 120);
            timerUpdate.Start();
        }

        void UpdateScreen(object sender, EventArgs e)
        {
            //System.Console.WriteLine("Alli");
            DrawNaoCamImageToPictureBox(pictureBox1);
        }


        private bool updatingPicture = false;
        void DrawNaoCamImageToPictureBox(PictureBox pb)
        {
            if (!updatingPicture)
            {
                updatingPicture = true;
                try
                {
                    byte[] imageBytes = _cam.GetImage();

                    if (imageBytes != null)
                    {
                        unsafe
                        {
                            BitmapData bmData =
                                _naoCamBitmap.LockBits(new Rectangle(0, 0, _naoCamBitmap.Width, _naoCamBitmap.Height),
                                                       ImageLockMode.WriteOnly,
                                                       PixelFormat.Format24bppRgb);

                            // Get a pointer to the beginning of the pixel data  
                            byte* p = (byte*)bmData.Scan0;
                            int diff = bmData.Stride - _naoCamBitmap.Width * 3;

                            for (int i = 0; i < imageBytes.Length; i++)
                            {
                                *p++ = imageBytes[i];
                                if (i % (_naoCamBitmap.Width * 3) == 0 && i > 0)
                                {
                                    p += diff;
                                }
                            }

                            // Release the memory  
                            _naoCamBitmap.UnlockBits(bmData);
                        }

                        pb.Image = _naoCamBitmap;
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Exception while accessing image: " + e);
                }
            }
            updatingPicture = false;
        }

    }
}
