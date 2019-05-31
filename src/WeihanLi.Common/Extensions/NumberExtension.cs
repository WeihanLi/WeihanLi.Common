using System;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class NumberExtension
    {
        #region 数字格式修改

        /// <summary>
        /// 数字转换为百分比显示
        /// </summary>
        /// <param name="number">数值</param>
        /// <param name="digits">精度，保留几位小数</param>
        /// <returns></returns>
        public static string Number2Percent(this double number, int digits = 2)
        {
            return (Math.Round(number, digits + 2) * 100).ToString("0.00") + "%";
        }

        /// <summary>
        /// 数字转换为百分比显示
        /// </summary>
        /// <param name="number">数值</param>
        /// <param name="digits">精度，保留几位小数</param>
        /// <returns></returns>
        public static string Number2Percent(this decimal number, int digits = 2)
        {
            return (Math.Round(number, digits + 2) * 100).ToString("0.00") + "%";
        }

        #endregion 数字格式修改
    }
}
