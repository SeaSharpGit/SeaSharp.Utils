using System.Text.RegularExpressions;

namespace SeaSharp.Utils.CSharp
{
    public static class RegexUtils
    {
        #region 电话
        /// <summary>
        /// 是否是手机
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPhone(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^1\d{10}$");
        }

        /// <summary>
        /// 是否是座机
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFixedPhone(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// 是否是座机或手机
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFixedOrPhone(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return IsPhone(value) || IsFixedPhone(value);
        }
        #endregion

        #region 身份证号
        /// <summary>
        /// 是否是身份证号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCardNo(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^(\d{3,4}-)?\d{6,8}$");
        }
        #endregion

        #region 邮箱
        /// <summary>
        /// 是否是邮箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        #endregion

        #region 字母和数字
        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^\d+$");
        }

        /// <summary>
        /// 是否是字母
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLetter(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[A-Za-z]+$");
        }

        /// <summary>
        /// 是否是字母或数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLetterOrNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 是否是大写字母
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUpperLetter(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[A-Z]+$");
        }

        /// <summary>
        /// 是否是小写字母
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLowerLetter(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[a-z]+$");
        }
        #endregion

        #region 汉字
        /// <summary>
        /// 是否是汉字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChinese(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[\u4e00-\u9fa5]{0,}$");
        }
        #endregion

        #region 网址
        /// <summary>
        /// 是否是网址
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsURL(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$");
        }
        #endregion

    }
}
