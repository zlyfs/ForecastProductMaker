using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerApi.Models.Meteorological;

namespace MeteoClientApp
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
            Image img;
            try
            {
                ReqMeteoPic reqWavePic = new ReqMeteoPic();
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
                img = Image.FromStream(stm);
                stm.Close();
            }
            catch
            {
                return null;
            }
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
            catch { return null; }
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
                    stationData.visibility = float.Parse(jsonObject[i]["visibility"].Value.ToString());
                    stationData.coordinateX = double.Parse(jsonObject[i]["coordinateX"].Value.ToString());
                    stationData.coordinateY = double.Parse(jsonObject[i]["coordinateY"].Value.ToString());
                    stationData.visibilityPrescription = int.Parse(jsonObject[i]["visibilityPrescription"].Value.ToString());
                    stationData.photoHead= jsonObject[i]["photoHead"].Value;
                    stationList.Add(stationData);
                }
                return stationList;
            }
            catch { return null; }
        }
        /// <summary>
        /// 向服务器推送保存信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static string PostMeteoData(string url, StationData sd,MissionInfo mi)
        {
            try
            {
                MeteoDataInputModel waveData = new MeteoDataInputModel();
                waveData.stationID = sd.stationID;
                waveData.visibility = sd.visibility;
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
            catch { return null; }
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
            catch { return null; }
        }

        /// <summary>
        /// 生成预报产品文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static string MeteoProducGeneration(string url, MissionInfo mi)
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
            catch { return null; }
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
            catch { return null; }
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
                //WriteLog(DateTime.Now.ToString() + "  重置客观数据异常  " + e.Message);
                return null;
            }
        }
    }
}
