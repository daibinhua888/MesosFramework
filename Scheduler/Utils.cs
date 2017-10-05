using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Scheduler
{
    public static class Utils
    {
        public static bool IsDigital(this char c)
        {
            if (c >= '0' && c <= '9')
                return true;

            return false;
        }

        public static string ToJson(this object obj)
        {
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            return jsonSerialize.Serialize(obj);
        }
        // 反序列化
        public static T ToObject<T>(this string jsonStr)
        {
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            return jsonSerialize.Deserialize<T>(jsonStr);
        }
    }
}
