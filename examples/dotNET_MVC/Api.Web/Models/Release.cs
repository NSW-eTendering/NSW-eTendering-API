using System.Collections.Generic;

namespace Api.Web.Models
{
    public class Release
    {
        public string language { get; set; }
        public string date { get; set; }
        public string ocid { get; set; }
        public string id { get; set; }
        public string initiationType { get; set; }
        public List<string> tag { get; set; }
        public List<Award> awards { get; set; }
        public Amendment amendment { get; set; }
        public Buyer buyer { get; set; }
        public Tender tender { get; set; }
    }
}