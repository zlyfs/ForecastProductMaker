using ServerApi.Models.Meteorological;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ServerApi.Controllers.Meteorological
{
    public class MeteoChartReadController : ApiController
    {

        /// <summary>
        /// 读取当天指定预报任务的记录表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<StationData> ChartRead( MissionInfo missionInfo)
        {
            if (missionInfo==null &missionInfo.stationInfoFile == null&missionInfo.forecastFilesHead==null&missionInfo.missionID==0) return null;
            // 初始化当天的表格,当天没有文件就新建一个
            if (!ChartProcess.ChartPreparation(missionInfo))
            {
                return null;
            }
            List<StationData> resultList = ChartProcess.DailyFileRead(missionInfo);
            
            return resultList;
        }






    }
}