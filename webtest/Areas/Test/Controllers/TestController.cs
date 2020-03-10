using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webtest.Areas.Test.Controllers
{
    public class TestController
    {
        public ActionResult Index()
        {

        }
    }

    public void CheckCookieInfo()
    {
        //1 判断当前的请求中是否包含用户名以及密码的cookie
        if (Request.Cookies["sms_UserName"] != null && Request.Cookies["sms_UserPwd"] != null)
        {
            //2 取出两个cookie
            string userName = Request.Cookies["sms_UserName"].Value;
            string userPwd = Request.Cookies["sms_UserPwd"].Value;

            //3 从数据库中查询指定用户名的对象
            if (userInfoBLL.CheckPwdByUser(userName, userPwd))
            {
                Response.Redirect("/Admin/Home/Index");
            }

            #region 已封装至UserBLL层中的CheckPwdByUser方法
            //var userInfo = userInfoBLL.GetListBy(u => u.UName == userName).FirstOrDefault();
            //if (userInfo != null)
            //{
            //    //判断当前cookie中传入的密码与数据库中的密码是否相同
            //    if (userInfo.UPwd == userPwd)
            //    {
            //        Response.Redirect("/Admin/Home/Index");
            //    }

            //}
            #endregion

        }
    }
}