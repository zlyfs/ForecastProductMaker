using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Wave
{
    public class WaveDataInputModel
    {
        public int stationID { get; set; }
        public float forecastValue1 { get; set; }
        public float forecastValue2 { get; set; }
        public float forecastValue3 { get; set; }
        public float forecastValue4 { get; set; }
        public float forecastValue5 { get; set; }
        public MissionInfo missionInfo { get; set; }
    }
}