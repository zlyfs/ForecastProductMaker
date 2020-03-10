using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApi.Controllers.Common
{
    public class ForecastReplace
    {
        public static string Replace(string source, string match, string replacement)
        {
            char[] sArr = source.ToCharArray();
            char[] mArr = match.ToCharArray();
            char[] rArr = replacement.ToCharArray();
            int idx = IndexOf(sArr, mArr);
            if (idx == -1)
            {
                throw new Exception("模板格式or预报参数有误");
                //return source;
            }
            else
            {
                string r = new string(sArr.Take(idx).Concat(rArr).Concat(sArr.Skip(idx + mArr.Length)).ToArray());
                return r;
            }
        }
        /// <summary>
        /// 查找字符数组在另一个字符数组中匹配的位置
        /// </summary>
        /// <param name="source">源字符数组</param>
        /// <param name="match">匹配字符数组</param>
        /// <returns>匹配的位置，未找到匹配则返回-1</returns>
        private static int IndexOf(char[] source, char[] match)
        {
            int idx = -1;
            for (int i = 0; i < source.Length - match.Length; i++)
            {
                if (source[i] == match[0])
                {
                    bool isMatch = true;
                    for (int j = 0; j < match.Length; j++)
                    {
                        if (source[i + j] != match[j])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        idx = i;
                        break;
                    }
                }
            }
            return idx;
        }
    }
}