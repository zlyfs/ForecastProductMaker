using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Models.Wave
{
    public class ReqWavePic
    {
        public string photoHead { get; set; }
        public int photoTail { get; set; }
        public string forecastFilesHead { get; set; }
        public string photoTailStr()
        {
            return photoTail.ToString();
        }
    }
}