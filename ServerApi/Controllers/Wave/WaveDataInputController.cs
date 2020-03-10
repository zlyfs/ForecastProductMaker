using ServerApi.Controllers.Common;
using ServerApi.Models.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ServerApi.Controllers.Wave
{
    public class WaveDataInputController : ApiController
    {
        /// <summary>
        /// 接收站点ID和预报数值
        /// </summary>
        /// <param name="sendModel"></param>
        /// <returns></returns>
        [HttpPost]
        public bool WaveDataInput(WaveDataInputModel waveDataInputModel)
        {
            string stateStr = "";
            // 初始化当天的表格,当天没有文件就新建一个
            if (!ChartProcess.ChartPreparation(waveDataInputModel.missionInfo)) return false;
            //读取当天的文件
            List<StationData> stationList = ChartProcess.DailyFileRead(waveDataInputModel.missionInfo);
            if (stationList == null) return false;
            //寻找相同id的修改预报值
            try
            {
                stationList.ForEach(s =>
                {
                    s.forecastValue1 = s.stationID == waveDataInputModel.stationID ? waveDataInputModel.forecastValue1 : s.forecastValue1;
                    s.forecastValue2 = s.stationID == waveDataInputModel.stationID ? waveDataInputModel.forecastValue2 : s.forecastValue2;
                    s.forecastValue3 = s.stationID == waveDataInputModel.stationID ? waveDataInputModel.forecastValue3 : s.forecastValue3;
                    s.forecastValue4 = s.stationID == waveDataInputModel.stationID ? waveDataInputModel.forecastValue4 : s.forecastValue4;
                    s.forecastValue5 = s.stationID == waveDataInputModel.stationID ? waveDataInputModel.forecastValue5 : s.forecastValue5;
                });
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\ForecastFiles\\" + DateTime.Today.ToString("yyyyMMdd") + "\\";
                string fileName = waveDataInputModel.missionInfo.forecastFilesHead + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                stateStr = fileName;
                if (!ChartProcess.ChartWrite(stationList, baseDirectory + fileName)) return false;
                
            }
            catch (Exception e)
            {
                CommonTools.WriteLog("更新海浪预报信息异常："+ stateStr +"\r\n"+ e.Message);
                return false;
            }
            return true;
        }


    }
}