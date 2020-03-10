using Newtonsoft.Json;
using ServerApi.Models.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ServerApi.Controllers.Common;

namespace ServerApi.Controllers.Wave
{
    public class ChartProcess
    {
        public static float NullValue = -999999;//空值
        /// <summary>
        /// 初始化当天的表格,当天没有文件就新建一个
        /// </summary>
        /// <returns></returns>
        public static bool ChartPreparation(MissionInfo missionInfo)
        {
            string fileName = missionInfo.forecastFilesHead+ DateTime.Today.ToString("yyyyMMdd") + ".txt";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\ForecastFiles\\"+ DateTime.Today.ToString("yyyyMMdd")+"\\";
            //检查目录是否存在
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            //检查今日文件是否存在
            if (!File.Exists(baseDirectory+ fileName))
            {
                //不存在就读取模板并新建
                List<StationData> stationList = StationInfoRead(missionInfo);
                if (stationList == null) return false;
                if (!ChartWrite(stationList, baseDirectory + fileName))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 读取StationInfo，初始化站点信息，并加入数值预报
        /// </summary>
        /// <returns></returns>
        public static List<StationData> StationInfoRead(MissionInfo missionInfo)
        {
            string stateStr = "";
            try
            {
                List<StationData> stationList = new List<StationData>();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\BaseInfo";
                stateStr = baseDirectory + missionInfo.stationInfoFile;
                StreamReader sr = new StreamReader(Path.Combine(baseDirectory, missionInfo.stationInfoFile));
                StringBuilder sb = new StringBuilder();
                while (sr.EndOfStream == false)
                {
                    sb.AppendLine(sr.ReadLine());
                }
                string jsonText = sb.ToString();
                sr.Close();
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
                    //获取对象的栏位
                    stationData.stationID = int.Parse(jsonObject[i]["stationID"].Value.ToString());
                    stationData.stationName = jsonObject[i]["stationName"].Value;
                    stationData.forecastPrescription = int.Parse(jsonObject[i]["forecastPrescription"].Value.ToString());
                    stationData.coordinateX = double.Parse(jsonObject[i]["coordinateX"].Value.ToString());
                    stationData.coordinateY = double.Parse(jsonObject[i]["coordinateY"].Value.ToString());
                    stationData.photoHead = jsonObject[i]["photoHead"].Value;                    
                    stationList.Add(stationData);
                }
                var stationList1 = NFDataRead(missionInfo, stationList);
                if (stationList1 != null) stationList = stationList1;
                return stationList;
            }
            catch (Exception e)
            {
                CommonTools.WriteLog("读取海浪StationInfo出错：" + stateStr + "\r\n" + e.Message);
                return null;
            }
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
                    stationData.forecastValue1= float.Parse(jsonObject[i]["forecastValue1"].Value.ToString());
                    stationData.forecastValue2 = float.Parse(jsonObject[i]["forecastValue2"].Value.ToString());
                    stationData.forecastValue3 = float.Parse(jsonObject[i]["forecastValue3"].Value.ToString());
                    stationData.forecastValue4 = float.Parse(jsonObject[i]["forecastValue4"].Value.ToString());
                    stationData.forecastValue5 = float.Parse(jsonObject[i]["forecastValue5"].Value.ToString());
                    stationData.forecastPrescription= int.Parse(jsonObject[i]["forecastPrescription"].Value.ToString());
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
                CommonTools.WriteLog("读取海浪每日预报出错：" + stateStr + "\r\n"+e.Message);
                return null;
            }
        }

        /// <summary>
        /// 写入表格
        /// </summary>
        /// <returns></returns>
        public static bool ChartWrite(List<StationData> stationList, string path)
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(stationList);
                StreamWriter FileWriter = new StreamWriter(path,false); //写文件
                FileWriter.Write(jsonStr);//将字符串写入
                FileWriter.Close(); //关闭StreamWriter对象
            }
            catch (Exception e)
            {
                CommonTools.WriteLog("写入海浪预报结果出错：" + path + "\r\n"+e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取当天文件名
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            return DateTime.Today.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 读取baseinfo文件，获取任务列表
        /// </summary>
        /// <returns></returns>
        public static List<MissionInfo> MissionInfoRead()
        {
            try
            {
                List<MissionInfo> missionList = new List<MissionInfo>();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory+"\\DailyData\\Wave\\BaseInfo";
                StreamReader sr = new StreamReader(Path.Combine(baseDirectory, "MissionInfo.txt"));
                StringBuilder sb = new StringBuilder();
                while (sr.EndOfStream == false)
                {
                    sb.AppendLine(sr.ReadLine());
                }
                string jsonText = sb.ToString();
                sr.Close();
                JsonSerializerSettings jSetting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyy-MM-dd HH:mm:ss"
                };
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText, jSetting);
                //var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText);
                for (int i = 0; i < jsonObject.Count; i++)
                {
                    MissionInfo missionInfo = new MissionInfo();
                    //获取父对象的栏位
                    missionInfo.missionID = int.Parse(jsonObject[i]["missionID"].Value.ToString());
                    missionInfo.missionName = jsonObject[i]["missionName"].Value;
                    missionInfo.forecastFilesHead = jsonObject[i]["forecastFilesHead"].Value;
                    missionInfo.stationInfoFile = jsonObject[i]["stationInfoFile"].Value;
                    missionInfo.outPutModel = jsonObject[i]["outPutModel"].Value;
                    missionList.Add(missionInfo);
                }
                return missionList;
            }
            catch(Exception e)
            {
                CommonTools.WriteLog("读取海浪BaseInfo文件出错：" + "\r\n"+e.Message);
                return null;
            }
        }
        /// <summary>
        /// 读取数值预报并修改任务中所有站点的数据
        /// </summary>
        /// <param name="missionInfo"></param>
        /// <param name="stationList"></param>
        /// <returns></returns>
        public static List<StationData> NFDataRead(MissionInfo missionInfo, List<StationData> stationList)
        {
            string fileName = missionInfo.forecastFilesHead + ".txt";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\NumericalForecast\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
            try
            {
                StreamReader sr = new StreamReader(Path.Combine(baseDirectory, fileName));
                StringBuilder sb = new StringBuilder();
                while (sr.EndOfStream == false)
                {
                    sb.AppendLine(sr.ReadLine());
                }
                sr.Close();
                string jsonText = sb.ToString();
                JsonSerializerSettings jSetting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyy-MM-dd HH:mm:ss"
                };
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText, jSetting);
                //var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText);
                //List<StationData> NFStationList = new List<StationData>();
                for (int i = 0; i < jsonObject.Count; i++)
                {
                    StationData stationData = new StationData(missionInfo);
                    //获取父对象的栏位
                    //stationData.stationID = int.Parse(jsonObject[i]["stationID"].Value.ToString());//无需读取
                    stationData.stationName = jsonObject[i]["stationName"].Value;//无需读取
                    stationData.forecastValue1 = float.Parse(jsonObject[i]["forecastValue1"].Value.ToString());
                    if (float.IsNaN(stationData.forecastValue1)) stationData.forecastValue1 = NullValue;
                    stationData.forecastValue2 = float.Parse(jsonObject[i]["forecastValue2"].Value.ToString());
                    if (float.IsNaN(stationData.forecastValue2)) stationData.forecastValue2 = NullValue;
                    stationData.forecastValue3 = float.Parse(jsonObject[i]["forecastValue3"].Value.ToString());
                    if (float.IsNaN(stationData.forecastValue3)) stationData.forecastValue3 = NullValue;
                    stationData.forecastValue4 = float.Parse(jsonObject[i]["forecastValue4"].Value.ToString());
                    if (float.IsNaN(stationData.forecastValue4)) stationData.forecastValue4 = NullValue;
                    stationData.forecastValue5 = float.Parse(jsonObject[i]["forecastValue5"].Value.ToString());
                    if (float.IsNaN(stationData.forecastValue5)) stationData.forecastValue5 = NullValue;
                    stationData.coordinateX = double.Parse(jsonObject[i]["longitude"].Value.ToString());//更新一下
                    stationData.coordinateY = double.Parse(jsonObject[i]["latitude"].Value.ToString());//更新一下
                    stationList.ForEach(s =>
                    {
                        if (s.stationName == stationData.stationName)//或者用id{
                        {
                            s.forecastValue1 = stationData.forecastValue1;
                            s.forecastValue2 = stationData.forecastValue2;
                            s.forecastValue3 = stationData.forecastValue3;
                            s.forecastValue4 = stationData.forecastValue4;
                            s.forecastValue5 = stationData.forecastValue5;
                            s.coordinateX = stationData.coordinateX;
                            s.coordinateY = stationData.coordinateY;
                        }
                    });
                }
                return stationList;
            }
            catch(Exception e)
            {
                CommonTools.WriteLog("读取海浪数值预报出错："+baseDirectory+fileName+"\r\n"+ e.Message);
                return null;
            }
        }

    }
}