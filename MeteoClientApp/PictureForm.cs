using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerApi.Models.Meteorological;
using CCWin;

namespace MeteoClientApp
{
    public partial class PictureForm : Form
    {
        Point m_Pnt;
        //Point pnt1;
        //Point pnt2;
        //Point pnt3;
        //Point pnt4;
        //Point pnt5;
        StationData stationData;
        public PictureForm()
        {
            InitializeComponent();
        }

        private void PictureForm_Load(object sender, EventArgs e)
        {
            m_Pnt = new Point(-1, -1);
            pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
        }

        public void ChangePicture(Image image,StationData sd)
        {
            stationData = sd;
            pictureBox1.Image = image;
            if (stationData == null) return;
            //float a = 10.20434f;
            //float b = -1067.09f;
            //float c = -10.8956f;
            //float d = 487.56f;
            //int pX = Convert.ToInt32(stationData.coordinateX * a + b) * pictureBox1.Width / 1600;
            ////int pX = Convert.ToInt32(stationData.coordinateX * 10 - 1100);//int.Parse((stationData.coordinateX * 10 - 1100).ToString());
            //int pY = Convert.ToInt32(stationData.coordinateY * c + d) * pictureBox1.Height / 900;
            ////int pY = Convert.ToInt32(430 - stationData.coordinateY * 10);//int.Parse((stationData.coordinateY * 10 - 90).ToString());
            //pnt1 = new Point(pX, pY);
            //pnt2 = new Point(pX + 320 * pictureBox1.Width / 1600, pY);
            //pnt3 = new Point(pX + 640 * pictureBox1.Width / 1600, pY);
            //pnt4 = new Point(pX + 960 * pictureBox1.Width / 1600, pY);
            //pnt5 = new Point(pX + 1280 * pictureBox1.Width / 1600, pY);
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (stationData == null)
            {
                return;
            }
            //Font f = new Font("宋体", 20.0f);
            //e.Graphics.FillEllipse( Brushes.Red, pnt1.X - 2, pnt1.Y - 2, 10, 10 );
            //e.Graphics.FillEllipse(Brushes.Red, pnt2.X - 2, pnt2.Y - 2, 10, 10);
            //e.Graphics.FillEllipse(Brushes.Red, pnt3.X - 2, pnt3.Y - 2, 10, 10);
            //e.Graphics.FillEllipse(Brushes.Red, pnt4.X - 2, pnt4.Y - 2, 10, 10);
            //e.Graphics.FillEllipse(Brushes.Red, pnt5.X - 2, pnt5.Y - 2, 10, 10);

            //e.Graphics.DrawString(stationData.stationName, f, Brushes.Red, pnt1);
            //e.Graphics.DrawString(stationData.stationName, f, Brushes.Red, pnt2);
            //e.Graphics.DrawString(stationData.stationName, f, Brushes.Red, pnt3);
            //e.Graphics.DrawString(stationData.stationName, f, Brushes.Red, pnt4);
            //e.Graphics.DrawString(stationData.stationName, f, Brushes.Red, pnt5);
        }

    }
}
