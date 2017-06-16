using System.Collections.Generic;

namespace Api.Web.Models
{
    public class Gazetteer
    {
        public string scheme { get; set; }
        public List<string> Identifiers { get; set; }
    }
}