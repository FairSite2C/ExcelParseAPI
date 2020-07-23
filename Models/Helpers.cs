using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace OriginsRx.Models.Helpers
{
    public class Constants
    {
        public const string RegexZipCode = "^[0-9]{5}(?:-[0-9]{4})?$";
        public const int SkipLimit = 20;

    }

    public class Converters
    {
        public static string TimeStampToString(byte[] tstamp)
        {
            var val = GetTimeStampValue(tstamp);
            return "0x" + val.ToString("X").PadLeft(16, '0');
        }

        public static long GetTimeStampValue(byte[] tstamp)
        {

            if (tstamp == null || tstamp.Length == 0) return 0;

            byte[] buffer = new byte[tstamp.Length];
            tstamp.CopyTo(buffer, 0);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToInt64(buffer, 0);
        }
    }

    public class FindRealName
    {

        public string FromJson<T>(string sort)
        {

            var result = sort.ToLower();

            PropertyInfo[] props = typeof(T).GetProperties();

            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.ToLower() == result)
                {
                    return result;
                }

                foreach (object attr in prop.GetCustomAttributes(true))
                {

                    if (attr is JsonPropertyAttribute)
                    {
                        JsonPropertyAttribute jt = (JsonPropertyAttribute)attr;
                        if (jt.PropertyName.ToLower() == result)
                        {
                            return prop.Name.ToLower();
                        }
                    }
                }
            }

            return "";
        }
    }
}
