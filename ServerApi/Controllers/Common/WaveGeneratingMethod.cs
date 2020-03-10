using ServerApi.Classes;
using ServerApi.Controllers.Wave;
using ServerApi.Models.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Spire.Doc;
using Spire.Doc.Documents;

namespace ServerApi.Controllers.Common
{
    public class WaveGeneratingMethod
    {
        /// <summary>
        /// 类型编号1.用于替换只有{fw}的模板
        /// </summary>
        public static string WaveFW(string modelText,MissionInfo missionInfo)
        {
            //替换海浪预报数据{fw}
            var forecastList = ReshapeWave(missionInfo);
            int i = 0;
            try
            {
                foreach (var temp in forecastList)
                {
                    modelText = ForecastReplace.Replace(modelText, "{fw}", temp);
                    i++;
                }
            }catch (Exception e)
            {
                throw e;
            }
            return modelText;
        }

        /// <summary>
        /// 类型编号2中，用于替换{fwt}
        /// </summary>
        /// <param name="modelText"></param>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        public static string WaveFWT(string modelText, MissionInfo missionInfo)
        {
            //替换海浪预报数据{fwt}
            var forecastList = ReshapeWave(missionInfo);
            int i = 0;
            try
            {
                foreach (var temp in forecastList)
                {
                    //按规则将浪高转换为适宜海上游览\较适宜海上游览\不适宜海上游览
                    float h = float.Parse(temp);
                    string s="";
                    if (h <= 1) s = "适宜海上游览";
                    if (h > 1 & h <= 1.8) s = "较适宜海上游览";
                    if (h > 1.8) s = "不适宜海上游览";
                    modelText = ForecastReplace.Replace(modelText, "{fwt}", s);
                    i++;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return modelText;
        }


        /// <summary>
        /// 类型编号3.用于海水浴场的海浪数据输入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outPath"></param>
        /// <param name="missionInfo"></param>
        public static void WaveHSYC(string path,string outPath, MissionInfo missionInfo)
        {
            Document document = new Document(path,FileFormat.Doc);
            TextSelection[] textFW = document.FindAllString("{fw}", true, false);
            TextSelection[] textFWYC = document.FindAllString("{fwyc}", true, false);
            TextSelection textY = document.FindString("{yyyy}", true, false);
            TextSelection textM = document.FindString("{mm}", true, false);
            TextSelection textD = document.FindString("{dd}", true, false);
            textY.GetAsOneRange().Text = DateTime.Today.ToString("yyyy");
            textM.GetAsOneRange().Text = DateTime.Today.ToString("MM");
            textD.GetAsOneRange().Text = DateTime.Today.ToString("dd");
            //获取模板文件信息
            //var enc= GetEncoding(path);
            //path = Path.GetDirectoryName(path) + "\\word\\document.xml";
            //读取模板
            //string modelText = TxtLoad(path,enc);
            //替换海浪预报数据{fwqq}、{fw}
            var forecastList = ReshapeWave(missionInfo);
            int i = 0;
            try
            {
                foreach (var temp in forecastList)
                {
                    //按规则将浪高转换为适宜、较适宜、不适宜
                    float h = float.Parse(temp);
                    string s="";
                    if (h < 0) s = "缺报";
                    if (h <= 1) s = "适宜";
                    if (h > 1 & h <= 1.8) s = "较适宜";
                    if (h > 1.8) s = "不适宜";
                    //modelText = ForecastReplace.Replace(modelText, "{fwyc}", s);
                    //modelText = ForecastReplace.Replace(modelText, "{fw}", temp);
                    textFW[i].GetAsOneRange().Text = temp;
                    textFWYC[i].GetAsOneRange().Text = s;
                    i++;
                }
                //检查目录是否存在
                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }
                document.SaveToFile(outPath);
            }
            catch (Exception e)
            {
                throw e;
            }
            //存储修改后的模板文件
            //var xmlOutPath= Path.GetDirectoryName(outPath) + "\\Temp\\word\\document.xml";
            //if (!TxtWrite(xmlOutPath, modelText, enc)) throw new Exception("产品存储失败");
            

        }


        /// <summary>
        /// 类型编号4.用于海浪-旅游全球类型的{fwqq}
        /// </summary>
        /// <param name="modelText"></param>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        public static string WaveFWQQ(string modelText, MissionInfo missionInfo)
        {
            //替换海浪预报数据{fwqq}
            var forecastList = ReshapeWave(missionInfo);
            int i = 0;
            try
            {
                foreach (var temp in forecastList)
                {
                    //按规则将浪高转换为轻浪、中浪、大浪等
                    float h = float.Parse(temp);
                    string s="";
                    if (h < 0.1) s = "微浪";
                    if (h >= 0.1 & h < 0.5) s = "小浪";
                    if (h >= 0.5 & h < 1.25) s = "轻浪";
                    if (h >= 1.25 & h < 2.5) s = "中浪";
                    if (h >= 2.5 & h < 4) s = "大浪";
                    if (h >= 4 & h < 6) s = "巨浪";
                    if (h >= 6 & h < 9) s = "狂浪";
                    if (h >= 9 & h < 14) s = "狂涛";
                    if (h >= 14) s = "怒涛";
                    modelText = ForecastReplace.Replace(modelText, "{fwqq}", s);
                    i++;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return modelText;
        }


        /// <summary>
        /// 将海浪预报数据整合成stringlist形式
        /// </summary>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        private static List<string> ReshapeWave(MissionInfo missionInfo)
        {
            var stationList = ChartProcess.DailyFileRead(missionInfo);
            List<string> forecastList = new List<string>();
            foreach (var temp in stationList)
            {
                if (temp.forecastPrescription > 0)
                {
                    forecastList.Add(temp.forecastValue1.ToString("F1"));
                    temp.forecastPrescription--;
                    if (temp.forecastPrescription > 0)
                    {
                        forecastList.Add(temp.forecastValue2.ToString("F1"));
                        temp.forecastPrescription--;
                        if (temp.forecastPrescription > 0)
                        {
                            forecastList.Add(temp.forecastValue3.ToString("F1"));
                            temp.forecastPrescription--;
                            if (temp.forecastPrescription > 0)
                            {
                                forecastList.Add(temp.forecastValue4.ToString("F1"));
                                temp.forecastPrescription--;
                                if (temp.forecastPrescription > 0)
                                {
                                    forecastList.Add(temp.forecastValue5.ToString("F1"));
                                }
                            }
                        }
                    }
                }
            }
            return forecastList;
        }

        /// <summary>
        /// 获取文本格式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string path)
        {
            //模型文本格式
            return TxtFileEncoding.GetEncoding(path);
        }

        /// <summary>
        /// 读取txt文档，并返回内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string TxtLoad(string path,Encoding enc)
        {
            
            //读取产品模型
            StreamReader sr = new StreamReader(path, enc);
            StringBuilder sb = new StringBuilder();
            while (sr.EndOfStream == false)
            {
                sb.AppendLine(sr.ReadLine());
            }
            string modelText = sb.ToString();
            sr.Close();
            modelText = DateReplace(modelText);
            return modelText;
        }

        /// <summary>
        /// 保存txt文档
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool TxtWrite(string outPath, string modelText,Encoding enc)
        {
            //检查目录是否存在
            if (!Directory.Exists(Path.GetDirectoryName(outPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            }
            try
            {
                StreamWriter FileWriter = new StreamWriter(outPath, false, enc); //写文件
                FileWriter.Write(modelText);//将字符串写入
                FileWriter.Close(); //关闭StreamWriter对象
                return true;
            }
            catch { return false; }

        }

        /// <summary>
        /// 将预设的日期填入
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DateReplace(string s)
        {
            //替换日期信息
            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("yyyy", DateTime.Today.ToString("yyyy"));
            a.Add("yy", DateTime.Today.ToString("yy"));
            a.Add("mm", DateTime.Today.ToString("MM"));
            a.Add("dd", DateTime.Today.ToString("dd"));
            s = StringReplace(s, a);
            return s;
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