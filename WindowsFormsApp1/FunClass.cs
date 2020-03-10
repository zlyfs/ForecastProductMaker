using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerApi.Models.Wave;

namespace ClientApp
{
    class FunClass
    {

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Image GetPicture(string url,string photoHead,int photoTail,string forecastFilesHead)
        {
            ReqWavePic reqWavePic = new ReqWavePic();
            reqWavePic.photoHead = photoHead;
            reqWavePic.photoTail = photoTail;
            reqWavePic.forecastFilesHead = forecastFilesHead;
            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.Method = "POST";
            req.ContentType = "application/json;charset=UTF-8";
            string jsonStr = JsonConvert.SerializeObject(reqWavePic);
            byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
            req.ContentLength = payload.Length;
            Stream writer = req.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            var response = req.GetResponse() as HttpWebResponse;
            Stream stm = response.GetResponseStream();
            Image img = Image.FromStream(stm);
            stm.Close();
            return img;
        }

        /// <summary>
        /// 获取所有站点数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<StationData> GetStationData(string url,MissionInfo missionInfo)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                string jsonStr = JsonConvert.SerializeObject(missionInfo);
                byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
                req.ContentLength = payload.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return Str2StationData(strValue, missionInfo);
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  获取所有站点数据异常  " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 将字符串转换成StationData格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<StationData> Str2StationData(string str,MissionInfo missionInfo)
        {
            List<StationData> stationList = new List<StationData>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(str);
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
                    stationData.coordinateX = double.Parse(jsonObject[i]["coordinateX"].Value.ToString());
                    stationData.coordinateY = double.Parse(jsonObject[i]["coordinateY"].Value.ToString());
                    stationData.forecastPrescription = int.Parse(jsonObject[i]["forecastPrescription"].Value.ToString());
                    stationData.photoHead= jsonObject[i]["photoHead"].Value;
                    stationList.Add(stationData);
                }
                return stationList;
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  将字符串转换成StationData格式异常  " + e.Message);
                return null;
            }
        }
        /// <summary>
        /// 向服务器推送保存信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static string PostWaveData(string url, StationData sd,MissionInfo mi)
        {
            try
            {
                WaveDataInputModel waveData = new WaveDataInputModel();
                waveData.stationID = sd.stationID;
                waveData.forecastValue1 = sd.forecastValue1;
                waveData.forecastValue2 = sd.forecastValue2;
                waveData.forecastValue3 = sd.forecastValue3;
                waveData.forecastValue4 = sd.forecastValue4;
                waveData.forecastValue5 = sd.forecastValue5;
                waveData.missionInfo = mi;
                string jsonStr = JsonConvert.SerializeObject(waveData);
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
                req.ContentLength = payload.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return strValue;
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  向服务器推送保存信息异常  " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取所有任务数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<MissionInfo> GetMissionList(string url)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "GET";
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return Str2MissionList(strValue);
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  获取所有任务数据异常  " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 生成预报产品文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static string WaveProducGeneration(string url, MissionInfo mi)
        {
            try
            {                
                string jsonStr = JsonConvert.SerializeObject(mi);
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
                req.ContentLength = payload.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return strValue;
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  生成预报产品文件异常  " + e.Message);
                return null;
            }
        }


        /// <summary>
        /// 将字符串转换成StationData格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<MissionInfo> Str2MissionList(string str)
        {
            List<MissionInfo> missionList = new List<MissionInfo>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(str);
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
                    MissionInfo missionInfo = new MissionInfo();
                    //获取父对象的栏位
                    missionInfo.missionID = int.Parse(jsonObject[i]["missionID"].Value.ToString());
                    missionInfo.missionName = jsonObject[i]["missionName"].Value;
                    missionInfo.stationInfoFile = jsonObject[i]["stationInfoFile"].Value;
                    missionInfo.forecastFilesHead = jsonObject[i]["forecastFilesHead"].Value;
                    missionInfo.outPutModel = jsonObject[i]["outPutModel"].Value;
                    missionList.Add(missionInfo);
                }
                return missionList;
            }
            catch(Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  将字符串转换成Mission格式异常  " + e.Message);
                return null;
            }
        }

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

        /// <summary>
        /// 重载客观预报信息，并保存
        /// </summary>
        /// <param name="url"></param>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        public static List<StationData> ReloadStationNFData(string url, MissionInfo missionInfo)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                string jsonStr = JsonConvert.SerializeObject(missionInfo);
                byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
                req.ContentLength = payload.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return Str2StationData(strValue, missionInfo);
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  重置客观数据异常  " + e.Message);
                return null;
            }
        }


        /// <summary>
        /// 仅获取客观预报信息，失败则为null
        /// </summary>
        /// <param name="url"></param>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        public static List<StationData> LoadStationNFData(string url, MissionInfo missionInfo)
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                string jsonStr = JsonConvert.SerializeObject(missionInfo);
                byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
                req.ContentLength = payload.Length;
                Stream writer = req.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var response = req.GetResponse() as HttpWebResponse;
                Stream stm = response.GetResponseStream();
                StreamReader reader = new StreamReader(stm, Encoding.UTF8);
                string strDate = "";
                string strValue = "";
                while ((strDate = reader.ReadLine()) != null)
                {
                    strValue += strDate + "\r\n";
                }
                reader.Close();
                stm.Close();
                return Str2StationData(strValue, missionInfo);
            }
            catch (Exception e)
            {
                WriteLog(DateTime.Now.ToString() + "  获取客观数据异常  " + e.Message);
                return null;
            }
        }
    }
}
