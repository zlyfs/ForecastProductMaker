using Newtonsoft.Json;
using ServerApi.Classes;
using ServerApi.Controllers.Common;
using ServerApi.Models.Wave;
using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Wave
{
    public class WaveProdConvController : ApiController
    {
        /// <summary>
        /// 生成9OC等文件所有的产品
        /// </summary>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public string ProdConv(MissionInfo missionInfo)
        {
            //预报文件名
            //string prodName = prodNameHead + DateTime.Today.ToString("yyyyMMdd");
            //预报文件夹
            string foreDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\ForecastFiles\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
            //预报产品模板文件夹
            string prodDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\OutPutModel\\" + missionInfo.forecastFilesHead;
            //产品输出目录
            string outDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Products\\" + DateTime.Today.ToString("yyyyMMdd") + "\\" + missionInfo.forecastFilesHead;
            Encoding enc;
            try
            {
                //获取当前任务的站点列表
                List<StationData> stationList = DailyFileRead(missionInfo);
                //获取预报产品模板文件夹下所有文件
                DirectoryInfo root = new DirectoryInfo(prodDirectory);
                FileInfo[] files = root.GetFiles();
                //针对每个产品模板进行操作
                foreach(var f in files)
                {
                    string outFileName = DateReplace(f.Name);
                    //txt类文件
                    if (f.Extension==".txt")
                    {
                        try
                        {
                            //读取文件内容
                            enc = GetEncoding(f.FullName);
                            string modelText = TxtLoad(prodDirectory + "//" + f.Name, enc);
                            //先替换气象的fmv
                            //用到了气象的信息
                            try
                            {
                                Models.Meteorological.MissionInfo meteoMissionInfo = new Models.Meteorological.MissionInfo();
                                meteoMissionInfo.forecastFilesHead = "JYC";
                                meteoMissionInfo.stationInfoFile = "JYStationInfo.txt";
                                modelText = MeteoGeneratingMethod.MeteoFMV(modelText, meteoMissionInfo);//替换{fm}
                            }
                            catch (Exception e) { }
                            //查询并替换
                            modelText = WaveTxt(modelText, stationList);
                            TxtWrite(outDirectory + "//" + outFileName, modelText, enc);
                        }
                        catch(Exception e)
                        {
                            CommonTools.WriteLog("txt产品生成出错：" + f.FullName + "\r\n" + e.Message);
                            return f.Name + "txt产品生成出错";
                        }
                    }
                    //doc类文件
                    if (f.Extension == ".doc")
                    {                        
                        Document document = new Document(f.FullName, FileFormat.Doc);
                        try
                        {                            
                            //TextSelection[] textFW = document.FindAllString("{fw}", true, false);
                            //TextSelection[] textFWYC = document.FindAllString("{fwyc}", true, false);
                            //替换日期信息
                            TextSelection textY = document.FindString("{yyyy}", true, false);
                            TextSelection textM = document.FindString("{mm}", true, false);
                            TextSelection textD = document.FindString("{dd}", true, false);
                            textY.GetAsOneRange().Text = DateTime.Today.ToString("yyyy");
                            textM.GetAsOneRange().Text = DateTime.Today.ToString("MM");
                            textD.GetAsOneRange().Text = DateTime.Today.ToString("dd");
                            //进入表格查找并替换关键字
                            for (int i = 0; i < document.Sections.Count; i++)
                            {
                                //Section section = document.Sections[i];
                                
                                for (int j=0;j< document.Sections[i].Tables.Count; j++)
                                {
                                    //Table table = section.Tables[j] as Table;
                                    for (int k = 0; k < document.Sections[i].Tables[j].Rows.Count; k++)
                                    {
                                        //var rows = table.Rows[k];
                                        
                                        for(int l = 0; l < document.Sections[i].Tables[j].Rows[k].Cells.Count; l++)
                                        {
                                            var cells = document.Sections[i].Tables[j].Rows[k].Cells[l];
                                            foreach(Paragraph paragrphs in cells.Paragraphs)
                                            {
                                                string str = paragrphs.Text;
                                                str = WaveTxt(str, stationList);
                                                paragrphs.Text = str;
                                            }
                                            
                                            
                                        }
                                    }
                                }
                            }
                                //检查目录是否存在
                                if (!Directory.Exists(Path.GetDirectoryName(outDirectory + "//" + outFileName)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(outDirectory + "//" + outFileName));
                                }
                                document.SaveToFile(outDirectory + "//" + outFileName);
                            document.Close();
                        }
                        catch(Exception e)
                        {
                            document.Close();
                            CommonTools.WriteLog("doc产品生成出错：" + f.FullName + "\r\n" + e.Message);
                            return f.Name + "doc产品生成出错";
                        }
                    }
                                      
                }
            }
            catch (Exception e)
            {

            }

            return "产品生成成功";
        }


        /// <summary>
        /// 读取每日预报信息
        /// </summary>
        /// <returns></returns>
        public static List<StationData> DailyFileRead(MissionInfo missionInfo)
        {
            string stateStr = "";
            try
            {
                List<StationData> stationList = new List<StationData>();
                string fileName = missionInfo.forecastFilesHead + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\ForecastFiles\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
                stateStr = baseDirectory + fileName;
                StreamReader sr = new StreamReader(Path.Combine(baseDirectory, fileName));
                StringBuilder sb = new StringBuilder();
                while (sr.EndOfStream == false)
                {
                    sb.AppendLine(sr.ReadLine());
                }
                string jsonText = sb.ToString();
                JsonSerializerSettings jSetting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyy-MM-dd HH:mm:ss"
                };
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText, jSetting);
                //var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText);
                for (int i = 0; i < jsonObject.Count; i++)
                {
                    StationData stationData = new StationData(missionInfo);
                    //获取父对象的栏位
                    stationData.stationID = int.Parse(jsonObject[i]["stationID"].Value.ToString());
                    stationData.stationName = jsonObject[i]["stationName"].Value;
                    stationData.forecastValue1 = float.Parse(jsonObject[i]["forecastValue1"].Value.ToString());
                    stationData.forecastValue2 = float.Parse(jsonObject[i]["forecastValue2"].Value.ToString());
                    stationData.forecastValue3 = float.Parse(jsonObject[i]["forecastValue3"].Value.ToString());
                    stationData.forecastValue4 = float.Parse(jsonObject[i]["forecastValue4"].Value.ToString());
                    stationData.forecastValue5 = float.Parse(jsonObject[i]["forecastValue5"].Value.ToString());
                    stationData.forecastPrescription = int.Parse(jsonObject[i]["forecastPrescription"].Value.ToString());
                    stationData.coordinateX = double.Parse(jsonObject[i]["coordinateX"].Value.ToString());
                    stationData.coordinateY = double.Parse(jsonObject[i]["coordinateY"].Value.ToString());
                    stationData.photoHead = jsonObject[i]["photoHead"].Value;
                    stationList.Add(stationData);
                }
                sr.Close();
                return stationList;
            }
            catch (Exception e)
            {
                CommonTools.WriteLog("读取海浪每日预报出错：" + stateStr + "\r\n" + e.Message);
                return null;
            }
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

        /// <summary>
        /// 读取txt文档，并返回内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string TxtLoad(string path, Encoding enc)
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
        public static bool TxtWrite(string outPath, string modelText, Encoding enc)
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
        /// 根据关键字查询数据，并替换
        /// </summary>
        private static string ReplaceWave(string str, List<StationData> stationList)
        {
            //将关键字分为站名、生成类型、时效3组
            //例如：str=“大连，fw，1”
            var str3 = str.Split(',');
            //从站点列表中进行查询
            foreach(var s in stationList)
            {
                //查找到指定站名时进行下一步处理
                if (s.stationName == str3[0])
                {                    
                    //按时效要求提取信息，并转换格式，最后返回
                    if (str3[2] == "1")
                    {
                        return WaveTranslation(str3[1], s.forecastValue1.ToString("F1"));
                    }
                    if (str3[2] == "2")
                    {
                        return WaveTranslation(str3[1], s.forecastValue2.ToString("F1"));
                    }
                    if (str3[2] == "3")
                    {
                        return WaveTranslation(str3[1], s.forecastValue3.ToString("F1"));
                    }
                    if (str3[2] == "4")
                    {
                        return WaveTranslation(str3[1], s.forecastValue4.ToString("F1"));
                    }
                    if (str3[2] == "5")
                    {
                        return WaveTranslation(str3[1], s.forecastValue5.ToString("F1"));
                    }
                    //说明没找到所标记的时效，反馈错误信息，写入产品文件中
                    else return "validForecastPrescription";           
                }
            }
            //说明没找到相同的站名
            return "StationNotFound";
        }
        /// <summary>
        /// 数据转换，根据数据显示种类，将数据转换成所需格式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string WaveTranslation(string type,string str)
        {
            if (type == "fw")
            {
                return str;
            }
            if (type == "fwt")
            {
                //按规则将浪高转换为适宜海上游览\较适宜海上游览\不适宜海上游览
                float h = float.Parse(str);
                string s = "";
                if (h < 0) s = "缺报";
                if (h <= 1) s = "适宜海上游览";
                if (h > 1 & h <= 1.8) s = "较适宜海上游览";
                if (h > 1.8) s = "不适宜海上游览";
                return s;
            }
            if(type== "fwqq")
            {
                //按规则将浪高转换为轻浪、中浪、大浪等
                float h = float.Parse(str);
                string s = "";
                if (h < 0) s = "缺报";
                if (h < 0.1) s = "微浪";
                if (h >= 0.1 & h < 0.5) s = "小浪";
                if (h >= 0.5 & h < 1.25) s = "轻浪";
                if (h >= 1.25 & h < 2.5) s = "中浪";
                if (h >= 2.5 & h < 4) s = "大浪";
                if (h >= 4 & h < 6) s = "巨浪";
                if (h >= 6 & h < 9) s = "狂浪";
                if (h >= 9 & h < 14) s = "狂涛";
                if (h >= 14) s = "怒涛";
                return s;
            }
            if (type == "fwyc")
            {
                //按规则将浪高转换为适宜、较适宜、不适宜
                float h = float.Parse(str);
                string s = "";
                if (h < 0) s = "缺报";
                if (h <= 1) s = "适宜";
                if (h > 1 & h <= 1.8) s = "较适宜";
                if (h > 1.8) s = "不适宜";
                //modelText = ForecastReplace.Replace(modelText, "{fwyc}", s);
                //modelText = ForecastReplace.Replace(modelText, "{fw}", temp);
                return s;
            }
            //说明没找到所写的种类代码
            return "ErrType";
        }

        /// <summary>
        /// 用于替换txt的模板
        /// </summary>
        public static string WaveTxt(string modelText, List<StationData> stationList)
        {
            //替换海浪预报数据{fw}
            //查找{并截断
            var str1 = modelText.Split('{');
            string modelTextOut = str1[0];
            for(int i=1; i < str1.Length; i++)
            {
                //第二次截断}
                var str2 = str1[i].Split('}');
                //替换后加入
                modelTextOut = modelTextOut + ReplaceWave(str2[0], stationList);
                //加入后面的内容
                modelTextOut = modelTextOut+str2[1];
            }
            return modelTextOut;
            //var forecastList = ReshapeWave(stationList);
            //int i = 0;
            //try
            //{
            //    foreach (var temp in forecastList)
            //    {
            //        modelText = ForecastReplace.Replace(modelText, "{fw}", temp);
            //        i++;
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return modelText;
        }
    }        
}