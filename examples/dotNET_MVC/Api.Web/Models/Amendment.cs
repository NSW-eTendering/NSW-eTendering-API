using System.Collections.Generic;

namespace Api.Web.Models
{
    public class Amendment
    {
        public string date { get; set; }
        public List<Change> changes { get; set; }
        public string rationale { get; set; }
    }
}