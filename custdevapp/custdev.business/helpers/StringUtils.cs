
using shortid;
using shortid.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace custdev.business.services
{
    public static class StringUtils
    {

        public static string GenerateId(int length = 10, bool usePrefix = true, bool upperCase = true)
        {
            var datetimePrefix = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss-f");
            var options = new GenerationOptions(useNumbers: true, useSpecialCharacters: true, length: length);
            string id = ShortId.Generate(options);
            if (usePrefix)
                id = $"{datetimePrefix}-{id}";
            else
                id = $"{id}";
            return upperCase ? id.ToUpper() : id;
        }

        public static string GenerateId(string requestId)
        {
            var datetimePrefix = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss-f");
            string id = $"{datetimePrefix}-{requestId}";
            return id;
        }

    }
}
