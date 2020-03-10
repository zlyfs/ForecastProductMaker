using ServerApi.Controllers.Common;
using ServerApi.Models.Meteorological;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Meteorological
{
    public class MeteoDataInputController : ApiController
    {
        /// <summary>
        /// 接收站点ID和预报数值
        /// </summary>
        /// <param name="sendModel"></param>
        /// <returns></returns>
        [HttpPost]
        public bool MeteoDataInput(MeteoDataInputModel meteoDataInputModel)
        {
            string stateStr = "";
            // 初始化当天的表格,当天没有文件就新建一个
            if (!ChartProcess.ChartPreparation(meteoDataInputModel.missionInfo)) return false;
            //读取当天的文件
            List<StationData> stationList = ChartProcess.DailyFileRead(meteoDataInputModel.missionInfo);
            if (stationList == null) return false;
            //寻找相同id的修改预报值
            try
            {
                stationList.ForEach(s =>
                {
                    s.visibility = s.stationID == meteoDataInputModel.stationID ? meteoDataInputModel.visibility : s.visibility;
                });
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Meteorological\\ForecastFiles\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
                string fileName = meteoDataInputModel.missionInfo.forecastFilesHead + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                stateStr = fileName;
                if (!ChartProcess.ChartWrite(stationList, baseDirectory + fileName)) return false;
                
            }
            catch (Exception e)
            {
                CommonTools.WriteLog("更新气象预报信息异常：" + stateStr + "\r\n" + e.Message);
                return false;
            }
            return true;
        }


    }
}