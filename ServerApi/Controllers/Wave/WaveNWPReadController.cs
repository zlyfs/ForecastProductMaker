using ServerApi.Models;
using ServerApi.Models.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Wave
{
    public class WaveNWPReadController : ApiController
    {
        /// <summary>
        /// 预读数值预报的值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<StationData> NWPRead(MissionInfo missionInfo)
        {
            if (missionInfo == null & missionInfo.stationInfoFile == null & missionInfo.forecastFilesHead == null & missionInfo.missionID == 0) return null;
            //读取站点每日数据
            List<StationData> stationDataList = ChartProcess.DailyFileRead(missionInfo);
            //读取客观数据，若成功则返回更新后的数据，若失败则返回null
            stationDataList = ChartProcess.NFDataRead(missionInfo, stationDataList);
            return stationDataList;
        }


    }
}