using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KeywordGetherer
{
    class YandexUtils
    {
        public string divideAndPrecede(string keyword)
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
                        buf += b.IndexOf("+") != -1 ? "!" + b : b.Replace('+', ' ');
                    }
                });
            buf += "]\"";
            return buf;
        }


        public string checkLenAndSlice(string keyword)
        {
            Regex rgx = new Regex(@"/[-:+#*$\\\\\/]/i");
            keyword = rgx.Replace(keyword, " ");

            rgx = new Regex(@"[\s]+");
            string newKeyword = "";
            Int16 index = 7;
            rgx.Split(keyword)
                .ToList()
                .ForEach(word =>
                {
                    if (word.Trim().Length > 0 && index != 0)
                    {
                        newKeyword += word.Trim() + " ";
                        index--;
                    }
                });
            return newKeyword.Trim();
        }
    }
}
