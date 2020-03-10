using ServerApi.Models.Meteorological;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Meteorological
{
    public class GetMeteoMissionListController : ApiController
    {
        /// <summary>
        /// 读取当天预报记录表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<MissionInfo> GetMissionList()
        {
            List<MissionInfo> resultList = ChartProcess.MissionInfoRead();
            return resultList;
        }
    }
}