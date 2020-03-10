using ServerApi.Controllers.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Wave
{
    public class StationData
    {
        public StationData(MissionInfo missionInfo)
        {
            stationID = 0;
            stationName = "未知";
            forecastValue1 = ChartProcess.NullValue;
            forecastValue2 = ChartProcess.NullValue;
            forecastValue3 = ChartProcess.NullValue;
            forecastValue4 = ChartProcess.NullValue;
            forecastValue5 = ChartProcess.NullValue;
            forecastPrescription = 0;
            if (missionID != 0)
                missionID = missionInfo.missionID;
            else missionID = 0;
            coordinateX = 0;
            coordinateY = 0;
            photoHead = "";
        }
        public int stationID { get; set; }
        public string stationName { get; set; }
        public float forecastValue1 { get; set; }
        public float forecastValue2 { get; set; }
        public float forecastValue3 { get; set; }
        public float forecastValue4 { get; set; }
        public float forecastValue5 { get; set; }
        public int forecastPrescription { get; set; }
        public double coordinateX { get; set; }
        public double coordinateY { get; set; }
        public int missionID { get; set; }
        public string photoHead { get; set; }
        public string forecastFilesHead(List<MissionInfo> infoList)
        {
            var temp = infoList.First(i => i.missionID == this.missionID);
            if (temp != null) return temp.forecastFilesHead;
            else return null;
        }


    }
}