using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ServerApi.Controllers.Common
{
    public class CommonTools
    {

        public static void WriteLog(string str)
        {
            string fileName = DateTime.Today.ToString("yyyyMMdd") + ".txt";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\";
            str = DateTime.Now.ToString() + str;
            //检查目录是否存在
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            //检查今日文件是否存在
            if (!File.Exists(baseDirectory + fileName))
            {
                FileStream fs = new FileStream(baseDirectory + fileName, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(str);//开始写入值
                sw.Close();
                fs.Close();
            }
            else
            {
                FileStream fs = new FileStream(baseDirectory + fileName, FileMode.Append, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(str);//开始写入值
                sr.Close();
                fs.Close();
            }
        }

    }
}