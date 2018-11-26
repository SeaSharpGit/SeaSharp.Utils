using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace SeaSharp.Utils.CSharp
{
    public static class IPUtils
    {
        #region 获取客户端IP地址（无视代理）,若失败则返回回送地址
        public static string GetIP()
        {
            if (HttpContext.Current.Request.IsLocal)
            {
                return "127.0.0.1";
            }
            var ip = HttpContext.Current.Request.ServerVariables["Remote_Addr"];
            return IsIP(ip) ? ip : HttpContext.Current.Request.UserHostAddress;
        }
        #endregion

        #region 检查是否是合法IP地址
        public static bool IsIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

        #region 判断IP是否在区间内
        public static bool IsIPAllow(string ip, string min, string max)
        {
            var isAllow = true;
            if (!IPAddress.TryParse(ip, out IPAddress p1)
                || !IPAddress.TryParse(min, out IPAddress p2)
                || !IPAddress.TryParse(max, out IPAddress p3))
            {
                return false;
            }

            //将点分十进制 转换成 4个元素的字节数组
            var startBytes = Get4Byte(min);
            var endBytes = Get4Byte(max);
            var ipBytes = Get4Byte(ip);

            for (int i = 0; i < 4; i++)
            {
                //从左到右 依次比较 对应位置的 值的大小  ，一旦检测到不在对应的范围 那么说明IP不在指定的范围内 并将终止循环
                if (ipBytes[i] > endBytes[i] || ipBytes[i] < startBytes[i])
                {
                    isAllow = false;
                    break;
                }
            }
            return isAllow;
        }


        /// <summary>
        /// 将IP四个值 转换成byte型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static byte[] Get4Byte(string ip)
        {
            string[] _i = ip.Split('.');

            List<byte> res = new List<byte>();
            foreach (var item in _i)
            {
                res.Add(Convert.ToByte(item));
            }

            return res.ToArray();
        }
        #endregion
    }
}
