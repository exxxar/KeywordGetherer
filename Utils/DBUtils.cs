using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class DBUtils       
    {
        
        string tableName { get; set; }       

        public Dictionary<string, string> serialize()
        {
            Type myType = this.GetType();
            this.tableName = myType.Name.ToLower();
            FieldInfo[] fields = myType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            
            fields
            .ToList()
            .ForEach(f => dict.Add(f.Name, Convert.ToString(f.GetValue(this))));
            return dict;
        }

        public object deseriallize(Dictionary<string, string> dict)
        {
            Type myType = this.GetType();
            this.tableName = myType.Name.ToLower();
            FieldInfo[] fields = myType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);

            fields
            .ToList()
            .ForEach(f => f.SetValue(this, dict[f.Name]));

            return this;
        }

        public string insert()
        {           
           StringBuilder _params= new StringBuilder(),
                         _value = new StringBuilder(),
                          query = new StringBuilder();           
            
            foreach(var k in this.serialize())
            {
                _params.AppendFormat("`{0}`,",k.Key);         
                _value.AppendFormat("`{0}`,", k.Value); 
            }

            query.AppendFormat("INSERT INTO `{0}` ({1}) VALUES ({2})",
               this.tableName,
                _params.Remove(_params.ToString().LastIndexOf(","),1),
                _value.Remove(_value.ToString().LastIndexOf(","),1)
                );

            return query.ToString();
        }

        public string count()
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT Count(*) FROM `{0}`", this.tableName);
            return stb.ToString();
        }

      


    }
}
