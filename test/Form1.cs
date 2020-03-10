using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = "d:\\0908pm.txt";
            StreamReader sr = new StreamReader(path,TxtFileEncoding.GetEncoding(path));
            StringBuilder sb = new StringBuilder();
            while (sr.EndOfStream == false)
            {
                sb.AppendLine(sr.ReadLine());
            }
            string jsonText = sb.ToString();
            sr.Close();
            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("yyyy", DateTime.Today.ToString("yyyy"));
            a.Add("yy", DateTime.Today.ToString("yy"));
            a.Add("mm", DateTime.Today.ToString("MM"));
            a.Add("dd", DateTime.Today.ToString("dd"));
            jsonText = StringReplace(jsonText, a);

            StreamWriter FileWriter = new StreamWriter("d:\\0908pm1.txt", false, TxtFileEncoding.GetEncoding(path)); //写文件
            FileWriter.Write(jsonText);//将字符串写入
            FileWriter.Close(); //关闭StreamWriter对象
        }

        public static string StringReplace(string msg, Dictionary<string, string> dic)
        {
            foreach (var obj in dic)
            {
                string r = "{" + obj.Key + "}";
                msg = msg.Replace(r, obj.Value);
            }

            return msg;
        }

    }
}
