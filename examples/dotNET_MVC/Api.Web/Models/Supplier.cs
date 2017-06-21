using System.Collections.Generic;

namespace Api.Web.Models
{
    public class Supplier
    {
        public Identifier identifier { get; set; }
        public List<AdditionalIdentifier> additionalIdentifiers { get; set; }
        public string name { get; set; }
        public string ABNExempt { get; set; }
        public string ABNExemptionReason { get; set; }
    }
}