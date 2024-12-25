
namespace custdev.domain.enums
{
    public static class LangEnum
    {
        public static string Ru = "ru";
        public static string En = "en";
        public static string Spa = "es";
        public static string Port = "pt";

        public static List<string> Languages
        {
            get
            {
                var result = new List<string>() {
                    LangEnum.Ru,
                    LangEnum.En,
                    LangEnum.Spa
                };
                return result;
            }
        }

        public static Dictionary<string, string> LanguageFullNames
        {
            get
            {
                var result = new Dictionary<string, string>();
                result.Add(LangEnum.Ru, "Russian");
                result.Add(LangEnum.En, "English");
                result.Add(LangEnum.Spa, "Spanish");
                result.Add(LangEnum.Port, "Portugese");
                return result;
            }
        }

        public static Dictionary<string, string> Channels
        {
            get
            {
                var result = new Dictionary<string, string>();
                result.Add("ru", "gdetort");
                result.Add("en", "1MinuteCake");
                result.Add("es", "_PastelEnUnMinuto");
                result.Add("pt", "BoloDeUmMinuto");

                return result;
            }
        }

        public static Dictionary<string, int> ChannelIndex
        {
            get
            {
                var result = new Dictionary<string, int>();
                result.Add("gdetort", 0);
                result.Add("1MinuteCake", 1);
                result.Add("_PastelEnUnMinuto", 4);
                result.Add("BoloDeUmMinuto", 7);

                return result;
            }
        }

        public static Dictionary<string, int> ChannelDelays
        {
            get
            {
                var result = new Dictionary<string, int>();
                result.Add("ru", 0);
                result.Add("en", 0);
                result.Add("es", 5);
                result.Add("pt", 10);


                return result;
            }
        }
    }
}
