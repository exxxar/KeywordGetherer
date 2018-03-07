using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer.SiteParser
{
    public class Keyword
    {
        public enum SearchEngine
        {
            GOOGLE = 0,
            YANDEX = 1

        }

        public long id { get; set; }
        public long keyword_id { get; set; }
        public String description { get; set; }
        public byte position { get; set; }
        public byte search_engine { get; set; }
        public Boolean is_ad { get; set; }
        public long region_id { get; set; }
        public long site_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public string toString()
        {
            return String.Format("{0}[{1}/{2}][id=>{3},keyword_id=>{4},position=>{5},SearchEngine=>{6},urlId=>{7},description=>{8},region_id=>{9}]",
                (is_ad ? "реклама" : "поиск"),
                 this.created_at,
                this.updated_at,
                this.id,
                this.keyword_id,
                this.position,
                Enum.GetName(typeof(SearchEngine), this.search_engine),
                this.site_id,
                this.description,
                 this.region_id
                );
        }

    }
}
