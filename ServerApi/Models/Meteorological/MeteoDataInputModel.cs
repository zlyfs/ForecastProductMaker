using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Meteorological
{
    public class MeteoDataInputModel
    {
        public int stationID { get; set; }
        public float visibility { get; set; }
        public MissionInfo missionInfo { get; set; }
    }
}