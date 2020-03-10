using ServerApi.Models.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using ServerApi.Classes;
using System.Text;
using Spire.Doc;

namespace ServerApi.Controllers.Common
{
    public class WaveProducGenerationController : ApiController
    {
        /// <summary>
        /// 用于生成产品文件
        /// </summary>
        /// <param name="missionInfo"></param>
        [HttpPost]
        public string WaveProducGeneration(MissionInfo missionInfo)
        {
            string fileName;
            int workType;
            string modelText;
            Encoding enc;
            try
            {
                //获取文件名            
                fileName = missionInfo.outPutModel.Split(';')[0];
                //获取处理方式
                workType = int.Parse(missionInfo.outPutModel.Split(';')[1]);
            }
            catch
            {
                return "outPutModel格式异常";
            }

            //模型目录
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\OutPutModel\\"+missionInfo.forecastFilesHead;
            var path = Path.Combine(baseDirectory, fileName);
            
            //变为输出文件名
            fileName = WaveGeneratingMethod.DateReplace(fileName);            
            //产品输出目录
            string outDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Products\\" + DateTime.Today.ToString("yyyyMMdd") + "\\" + missionInfo.forecastFilesHead;
            var outPath = Path.Combine(outDirectory, fileName);
            try
            {
                switch (workType)
                {
                    case 0:
                        return "该产品不由本组生成";
                    //只包含{fw}的txt产品生成
                    case 1:
                        enc = WaveGeneratingMethod.GetEncoding(path);
                        modelText = WaveGeneratingMethod.TxtLoad(path,enc);
                        modelText = WaveGeneratingMethod.WaveFW(modelText, missionInfo);
                        if(WaveGeneratingMethod.TxtWrite(outPath, modelText, enc))return "产品生成成功";
                        else return "产品存储失败";

                    //教育台19城市产品生成
                    case 2:
                        //用到了气象的信息
                        Models.Meteorological.MissionInfo meteoMissionInfo = new Models.Meteorological.MissionInfo();
                        meteoMissionInfo.forecastFilesHead = missionInfo.forecastFilesHead;
                        meteoMissionInfo.stationInfoFile = missionInfo.stationInfoFile;
                        enc = WaveGeneratingMethod.GetEncoding(path);
                        modelText = WaveGeneratingMethod.TxtLoad(path,enc);
                        modelText = WaveGeneratingMethod.WaveFW(modelText, missionInfo);//替换{fw}
                        modelText = WaveGeneratingMethod.WaveFWT(modelText, missionInfo);//替换{fwt}
                        modelText = MeteoGeneratingMethod.MeteoFMV(modelText, meteoMissionInfo);//替换{fm}
                        if (WaveGeneratingMethod.TxtWrite(outPath, modelText, enc)) return "产品生成成功";
                        else return "产品存储失败";
                    //海水浴场docx文件生成，修改的模板是docx解压文件，修改后需压缩为docx文件    
                    case 3:
                        WaveGeneratingMethod.WaveHSYC(path, outPath, missionInfo);
                        return "产品生成成功";
                    //全球产品生成
                    case 4:
                        enc = WaveGeneratingMethod.GetEncoding(path);
                        modelText = WaveGeneratingMethod.TxtLoad(path, enc);
                        modelText = WaveGeneratingMethod.WaveFWQQ(modelText, missionInfo);
                        if (WaveGeneratingMethod.TxtWrite(outPath, modelText, enc)) return "产品生成成功";
                        else return "产品存储失败";
                    default:

                        return "未找到所选产品样式";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
            
            
        }
        
        
    }
}