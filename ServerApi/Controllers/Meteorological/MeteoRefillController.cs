using ServerApi.Models.Meteorological;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Meteorological
{
    public class MeteoRefillController : ApiController
    {
        [HttpPost]
        public List<StationData> MeteoRefill(MissionInfo missionInfo)
        {
            if (missionInfo == null & missionInfo.stationInfoFile == null & missionInfo.forecastFilesHead == null & missionInfo.missionID == 0) return null;
            //读取站点每日数据
            List<StationData> stationDataList = ChartProcess.DailyFileRead(missionInfo);
            //读取客观数据，若成功则返回更新后的数据，若失败则返回null
            var stationDataList1 = ChartProcess.NFDataRead(missionInfo, stationDataList);
            //若更新成功，则保存,并返回新数据，否则返回原数据
            if (stationDataList1 != null)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Meteorological\\ForecastFiles\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
                string fileName = missionInfo.forecastFilesHead + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                if (!ChartProcess.ChartWrite(stationDataList1, baseDirectory + fileName)) return null;
                return stationDataList1;
            }
            else return stationDataList;


        }
    }
}