using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IntelligenceCloud.Helpers
{
    public class ParseHelper
    {
        public static DateTime? ParseDateTime(string str)
        {
            string[] format = { "yyyy-MM-ddTHH:mm:ss",
                "yyyy/MM/dd tt hh:mm:ss",
                "yyyy/M/dd tt hh:mm:ss",
                "yyyy/M/d tt hh:mm:ss",
                "yyyy/MM/d tt hh:mm:ss"
            };//

            
            str = Regex.Replace(str, "\\(.+\\)","");//除掉後面(UCT+8)

            if (DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime))
            {
                return dateTime;
            }
            else if (DateTime.TryParseExact(str, format, new CultureInfo("zh-TW"), DateTimeStyles.AllowWhiteSpaces, out dateTime))
            {
                return dateTime;//針對culture中文  有上午下午字樣
            }
            else
            {
                return null;
            }


        }
    }
}