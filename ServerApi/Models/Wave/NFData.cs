using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Wave
{
    public class NFData
    {
        public NFData(MissionInfo missionInfo)
        {
            stationID = 0;
            stationName = "未知";
            forecastValue1 = 999;
            forecastValue2 = 999;
            forecastValue3 = 999;
            forecastValue4 = 999;
            forecastValue5 = 999;
            
            if (missionID != 0)
                missionID = missionInfo.missionID;
            else missionID = 0;

        }
        public int stationID { get; set; }
        public string stationName { get; set; }
        public float forecastValue1 { get; set; }
        public float forecastValue2 { get; set; }
        public float forecastValue3 { get; set; }
        public float forecastValue4 { get; set; }
        public float forecastValue5 { get; set; }
        public int missionID { get; set; }

    }
}