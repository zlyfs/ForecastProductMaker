using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Meteorological
{
    public class MissionInfo
    {
        public int missionID { get; set; }
        public string missionName { get; set; }
        public string forecastFilesHead { get; set; }
        public string stationInfoFile { get; set; }
        public string outPutModel { get; set; }
    }
}