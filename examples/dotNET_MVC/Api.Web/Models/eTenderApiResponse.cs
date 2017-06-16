using System.Collections.Generic;

namespace Api.Web.Models
{
    public class eTenderAPIResponse
    {
        public List<Release> releases { get; set; }
        public List<Error> errors { get; set; }
    }
}