using ServerApi.Controllers.Meteorological;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Meteorological
{
    public class StationData
    {
        public StationData(MissionInfo missionInfo)
        {
            stationID = 0;
            stationName = "未知";
            visibility = ChartProcess.NullValue;
            visibilityPrescription = 0;
            if (missionID != 0)
                missionID = missionInfo.missionID;
            else missionID = 0;
            coordinateX = 0;
            coordinateY = 0;
            photoHead = "";
        }
        public int stationID { get; set; }
        public string stationName { get; set; }
        public float visibility { get; set; }
        public int visibilityPrescription { get; set; }
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