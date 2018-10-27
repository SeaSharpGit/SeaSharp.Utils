using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SeaSharp.Utils
{
    public static class BaseUtils
    {
        #region 字符串相关
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToJsonObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Join<T>(this IEnumerable<T> list, string split)
        {
            return string.Join(split, list);
        }

        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }

        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }

        public static string ToUpperFirst(this string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            return str[0].ToUpper().ToString() + str.Substring(1);
        }
        #endregion

        #region 判断整数是奇数/偶数
        /// <summary>
        /// 判断一个整数是否为奇数
        /// </summary>
        public static bool IsOdd(this int n)
        {
            return n % 2 == 1;
        }

        /// <summary>
        /// 判断一个整数是否为偶数
        /// </summary>
        public static bool IsEven(this int n)
        {
            return n % 2 == 0;
        }
        #endregion

        #region Object转换为基础类型
        public static bool ToBoolean(this object obj)
        {
            return Convert.ToBoolean(obj);
        }

        public static short ToInt16(this object obj)
        {
            return Convert.ToInt16(obj);
        }

        public static short ToInt16(this object obj, short defaultValue)
        {
            return short.TryParse(obj.ToString(), out short result) ? result : defaultValue;
        }

        public static int ToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static int ToInt32(this object obj, int defaultValue)
        {
            return int.TryParse(obj.ToString(), out int result) ? result : defaultValue;
        }

        public static long ToInt64(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        public static long ToInt64(this object obj, long defaultValue)
        {
            return long.TryParse(obj.ToString(), out long result) ? result : defaultValue;
        }

        public static DateTime ToDateTime(this object obj)
        {
            return Convert.ToDateTime(obj);
        }

        public static DateTime ToDateTime(this object obj, DateTime defaultValue)
        {
            return DateTime.TryParse(obj.ToString(), out DateTime result) ? result : defaultValue;
        }

        public static decimal ToDecimal(this object obj)
        {
            return Convert.ToDecimal(obj);
        }

        public static decimal ToDecimal(this object obj, decimal defaultValue)
        {
            return decimal.TryParse(obj.ToString(), out decimal result) ? result : defaultValue;
        }
        #endregion

        #region 单词变成单数形式
        /// <summary>
        /// 单词变成单数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToSingular(this string word)
        {
            var plural1 = new Regex("(?<keep>[^aeiou])ies$");
            var plural2 = new Regex("(?<keep>[aeiou]y)s$");
            var plural3 = new Regex("(?<keep>[sxzh])es$");
            var plural4 = new Regex("(?<keep>[^sxzhyu])s$");

            return plural1.IsMatch(word) ? plural1.Replace(word, "${keep}y")
                : plural2.IsMatch(word) ? plural2.Replace(word, "${keep}")
                : plural3.IsMatch(word) ? plural3.Replace(word, "${keep}")
                : plural4.IsMatch(word) ? plural4.Replace(word, "${keep}")
                : word;
        }
        #endregion

        #region 单词变成复数形式
        /// <summary>
        /// 单词变成复数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToPlural(this string word)
        {
            var plural1 = new Regex("(?<keep>[^aeiou])y$");
            var plural2 = new Regex("(?<keep>[aeiou]y)$");
            var plural3 = new Regex("(?<keep>[sxzh])$");
            var plural4 = new Regex("(?<keep>[^sxzhy])$");

            return plural1.IsMatch(word) ? plural1.Replace(word, "${keep}ies")
                : plural2.IsMatch(word) ? plural2.Replace(word, "${keep}s")
                : plural3.IsMatch(word) ? plural3.Replace(word, "${keep}es")
                : plural4.IsMatch(word) ? plural4.Replace(word, "${keep}s")
                : word;
        }
        #endregion

        #region 深拷贝
        public static T CloneDeep<T>(this T obj)
        {
            if (obj == null || obj is string || obj.GetType().IsValueType)
            {
                return obj;
            }

            var model = Activator.CreateInstance(obj.GetType());
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(model, CloneDeep(field.GetValue(obj)));
                }
                catch
                {
                }
            }
            return (T)model;
        }
        #endregion





    }
}
