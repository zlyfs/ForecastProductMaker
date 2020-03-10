using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ServerApi.Models.Wave;
using ServerApi.Controllers.Common;

namespace ServerApi.Controllers.Wave
{
    public class WavePictureController : ApiController
    {
        [HttpPost]
        //[Route("v1/file/{subpath}/{filename}")]
        public async Task<HttpResponseMessage> WavePicture( ReqWavePic reqWavePic)
        {
            string stateStr = "";
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\DailyData\\Wave\\Pictures\\" + DateTime.Today.ToString("yyyyMMdd") + "\\" + reqWavePic.forecastFilesHead+"\\"+ reqWavePic.photoHead + reqWavePic.photoTailStr() + ".png";
                stateStr = path;
                if (!File.Exists(path))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "NoPicture.png";
                    stateStr = path;
                }
                FileStream stream = new FileStream(path, FileMode.Open);

                HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };
                resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(path)

                };
                string contentType = MimeMapping.GetMimeMapping(path);
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                resp.Content.Headers.ContentLength = stream.Length;

                return await Task.FromResult(resp);

            }
            catch (Exception e)
            {
                CommonTools.WriteLog("获取图片异常："+ stateStr+"\r\n" +e.Message);

            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}