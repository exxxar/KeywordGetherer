using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KeywordGetherer
{
    public static class  YandexUtils
    {
        public static string divideAndPrecede(this string keyword)
        {
            if (keyword.Trim().IndexOf("[") != -1
                && keyword.Trim().IndexOf("]") != -1
                && keyword.Trim().IndexOf("!") != -1)
                return keyword;

            string buf = "\"[";
            keyword.Split(' ')
                .ToList()
                .ForEach(b =>
                {
                    b = b.Trim();
                    if (b.Length > 0)
                    {
                        buf += "!" + b +" " ;
                    }
                });
            buf += "]\"";
            return buf;
        }


        public static string checkLenAndSlice(this string keyword)
        {
            Regex rgx = new Regex(@"[!@#$%&*_+=-\\]");
            keyword = rgx.Replace(keyword, " ");

            rgx = new Regex(@"[\s]+");
            string newKeyword = "";
            Int16 index = 7;
            rgx.Split(keyword)
                .ToList()
                .ForEach(word =>
                {
                    if (word.Trim().Length > 0 && word.Trim().Length < 35 && index != 0)
                    {
                        newKeyword += word.Trim() + " ";
                        index--;
                    }
                });
            return newKeyword.Trim();
        }

        public static string restoringPrecede(this string keyword)
        {
            keyword = keyword.Replace("\"", "");

            Regex rgx = new Regex(@"[!\[\]]");
            keyword = rgx.Replace(keyword, " ");

            StringBuilder str = new StringBuilder();
            keyword.Split(' ')
                .ToList()
                .ForEach(kw=> {
                    if (kw.Trim().Length > 0)
                        str
                        .Append(kw.Trim())
                        .Append(" ");
                });
       
            return str.ToString();
        }

    }
}
