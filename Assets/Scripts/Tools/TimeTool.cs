using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTool 
{  
    /// 获取当前utc时间戳
    /// </summary>
    /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
    /// <returns></returns>
    public static long GetTimeStamp(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    } 

    /// <summary>
    /// utc日期时间转utc时间戳
    /// </summary>
    /// <param name="_dateTime">utc日期时间</param>
    /// <returns></returns>
    public static long UtcDateToTimeStamp(DateTime _utcDateTime) {
        TimeSpan st = _utcDateTime - new DateTime(1970, 1, 1, 0, 0, 0);
        return Convert.ToInt64(st.TotalSeconds);
    }
    /// <summary>
    /// 本地日期时间转utc时间戳
    /// </summary>
    /// <param name="_dateTime">本地日期时间</param>
    /// <returns></returns>
    public static long DateToTimeStamp(DateTime _DateTime)
    {
        TimeSpan st = _DateTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return Convert.ToInt64(st.TotalSeconds);
    }

    /// <summary>
    /// utc时间戳转本地日期时间
    /// </summary>
    /// <param name="_timeStamp">时间戳</param>
    /// <returns></returns>
    public static DateTime UtcStampToDateTime(double _timeStamp) {
        //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0);
        startTime = startTime.AddSeconds(_timeStamp);
        startTime = TimeZone.CurrentTimeZone.ToLocalTime(startTime);
        return startTime;
    }

    /// <summary>
    /// 字符串转日期时间
    /// </summary>
    /// <param name="date_str">时间字符串</param>
    /// <returns></returns>
    public static DateTime StringToDateTime(string date_str) {
        return Convert.ToDateTime(date_str);
    }

    /// <summary>
    /// 时钟式倒计时
    /// </summary>
    /// <param name="second">秒</param>
    /// <returns></returns>
    public static string GetSecondString(int second)
    {
        if (second >= 3600)
            return string.Format("{0:D2}:{1:D2}:{2:D3}", second / 3600, second % 3600 / 60, second % 60);
        else
            return string.Format("{0:D2}:{1:D2}", second / 60, second % 60);
    }
}
