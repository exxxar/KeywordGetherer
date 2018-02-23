using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KeywordGetherer
{
    public static class  YandexUtils
    {
        public static void sliceAndTakeVariants(this string keyword)
        {
            DBConection db = new DBConection();           
           // List<string> result = new List<string>();         

            string [] tmp = keyword.Split(' ');

            if (tmp.Length > 7)
                tmp = keyword
                        .checkLenAndSlice()
                        .Split(' ');
                     
            tmp.ToList().ForEach(i =>
            {

                if (!db.isKeywordExist((new StringBuilder()).AppendFormat("{0}", i).ToString()) && i.Length >= 3)
                { //result.Add((new StringBuilder()).AppendFormat("{0}", i).ToString());
                    db.Insert((new StringBuilder()).AppendFormat("{0}", i).ToString());
                    Console.WriteLine((new StringBuilder()).AppendFormat("====>{0}", i).ToString());
                }

                if (i.Length >= 3)
                    tmp.ToList().ForEach(j =>
                    {
                        if (!i.Equals(j) && !db.isKeywordExist((new StringBuilder()).AppendFormat("{0} {1}", i, j).ToString()) && j.Length >= 3)
                        {// result.Add((new StringBuilder()).AppendFormat("{0} {1}", i, j).ToString());
                           
                            db.Insert((new StringBuilder()).AppendFormat("{0} {1}", i, j).ToString());
                            Console.WriteLine((new StringBuilder()).AppendFormat("====>{0} {1}", i,j).ToString());
                        }
                    });
            });
           // return result;
        }

      
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
