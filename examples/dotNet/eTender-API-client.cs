using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
// using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using System.Runtime.Serialization.Json;
// using System.Web.Script.Serialization;


namespace APIclient
{
    public class APIclient
    {        
        public static void Main(string[] args)
        {
            String ApiOutput = EtrApiTest().Result;
            Console.WriteLine(ApiOutput);
        }

        /// Call the eTender API for a list of Planned Procurements, and loop through them to find related tenders and contracts 
        public static async Task<String> EtrApiTest()
        {
// Title Case

//            using (var client = new HttpClient()){}  // redundant?
            APIclient apiClient = new APIclient();
            {
                var baseUrl = "http://etr-aws.gruden.com/";

                var result = "<table><tr><th>Planned Procurement</th><th>Tenders</th><th>Contracts</th></tr>";

// valid search result              ?event=public.api.planning.search&ResultsPerPage=99
// fail with "errors"               ?event=public.api.tender.view&RFTUUID=DCBEFDDE-CFF9-430D-4DFAE339EA77430
                eTenderAPIResponse jsonResponse = await apiClient.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.search&ResultsPerPage=99");

// Console.WriteLine("jsonResponse : " + jsonResponse);

                foreach (Release release in jsonResponse.releases)
                {
                    var tender = release.tender;
                    String ppUuid = tender.PlannedProcurementUUID;
                    eTenderAPIResponse plannedProcurement = await apiClient.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid);
                    var rftArray = tender.relatedRFT;
                    var rftCount = rftArray?.Count ?? 0;
                    var ppTender = plannedProcurement.releases[0].tender;
                    result += "<tr><td rowspan='"+ rftCount  + "'><a href=\"" + baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid + "\">";
                    result += ppTender.id + "</a><br/>Estimated Date of Approach to Market: " + ppTender.estimatedDateToMarket + "</td>";

                    if (rftCount > 0)  
                    {
                        var rftIndex = 0;
                        foreach (var rftUuid in rftArray)
                        {
                            result += "<td>";
                            eTenderAPIResponse rft = await apiClient.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.tender.view&RFTUUID=" + rftUuid);
                            var rftTender = rft.releases[0].tender;
                            result += "<a href=\"" + baseUrl + "?event=public.rft.show&RFTUUID=" + rftUuid + "\">";
                            result += rftTender.title + "</a><br/>Published: "+ rftTender.tenderPeriod.startDate + "</td>";
                            result += "<td>";
                            if (rftTender.relatedCN != null)
                            {
                               
                                foreach(var cnUuid in rftTender.relatedCN)
                                {
                                    eTenderAPIResponse contract = await apiClient.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&CNUUID=" + cnUuid);
                                    result += "<a href='" + baseUrl + "?event=public.cn.view&CNUUID=" + cnUuid + "'>";
                                    result += contract.releases[0].awards[0].title + "</a><br/>";
                                    // the server will block your IP address if you make too many responses in a short time
                                    await Task.Delay(500);
                                }
                            }
                            if (rftTender.relatedSON != null)
                            {
                                foreach (var sonUuid in rftTender.relatedSON)
                                {
                                    eTenderAPIResponse contract = await apiClient.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&SONUUID=" + sonUuid);
                                    result += "<a href='" + baseUrl + "?event=public.SON.view&SONUUID=" + sonUuid + "'>";
                                    result += contract.releases[0].awards[0].title + "</a><br/>";
                                    await Task.Delay(500);
                                }
                               
                            }
                            result += "</td>";
                            
                            if (rftIndex != rftCount - 1)
                            {
                                result += "</tr><tr>";
                            }
                            rftIndex++;

                        }
                    }
                    else
                    {
                        result += "<td>No Tenders</td><td></td>";
                    }
                    result += "</tr>";
                }
                
                result += "</table>";


                return result;
            }
        }



/// A generic HTTP request which checks for HTTP and API errors and returns the JSON object 
        private static HttpClient httpClient = new HttpClient();

        private async Task<T> GetAndDecode<T>(string url) where T : eTenderAPIResponse

        {
            {
                var HttpResult = "";
 Console.WriteLine("Calling : " + url);      

                HttpResponseMessage response = await httpClient.GetAsync(url);
                // Throw an exception if the request fails 
                // (the eTender API returns a 404 with a json message for a missing object / bad parameter, so this will catch API errors as well as network/server errors

                try {
                    if (response.IsSuccessStatusCode) {
                        HttpResult = await response.Content.ReadAsStringAsync();  
                    } else {
                        Console.WriteLine("Exception Message: {0}", "Error getting http response " + HttpResult);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception Message: {0}",e.Message);
                }               

// Console.WriteLine("HttpResult : " + HttpResult);       // HttpResult is a long json string.  good.
   
                T apiResponse = JsonConvert.DeserializeObject<T>(HttpResult);

                // not neccessary - already caught by EnsureSuccessStatusCode above
                if (HttpResult.IndexOf("\"errors\":") > 0) {  
                        Console.WriteLine(apiResponse.errors + " from " + url);
                    }
                return apiResponse;

            }
            
        }
    }
}



// C# classes generated from json2csharp.com
// put all this in a model folder

public class IndustrialRelationsDetails
{
    public string subContractorNames { get; set; }
    public string applicableIndustrialInstruments { get; set; }
    public string locationOfWork { get; set; }
}

public class Value
{
    public double amount { get; set; }
    public string currency { get; set; }
}

public class Identifier
{
    public string scheme { get; set; }
    public string id { get; set; }
    public string legalName { get; set; }
}

public class AdditionalIdentifier
{
    public string scheme { get; set; }
    public string id { get; set; }
}

public class Supplier
{
    public Identifier identifier { get; set; }
    public List<AdditionalIdentifier> additionalIdentifiers { get; set; }
    public string name { get; set; }
    public string ABNExempt { get; set; }
    public string ABNExemptionReason { get; set; }
}

public class Classification
{
    public string scheme { get; set; }
    public string id { get; set; }
    public string description { get; set; }
}

public class Item
{
    public string description { get; set; }
    public Classification classification { get; set; }
}

public class ContractPeriod
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}

public class TotalContractGroupPeriod
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}

public class ContractGroup
{
    public TotalContractGroupPeriod totalContractGroupPeriod { get; set; }
    public double totalContractGroupValue { get; set; }
    public List<string> relatedCN { get; set; }
}

public class Award
{
    public object multiAgencyAccess { get; set; }
    public string id { get; set; }
    public string publishedDate { get; set; }
    public string CNUUID { get; set; }
    public string SONUUID { get; set; }
    public string title { get; set; }
    public string status { get; set; }
    public string date { get; set; }
    public string evaluationCriteria { get; set; }
    public IndustrialRelationsDetails industrialRelationsDetails { get; set; }
    public Value value { get; set; }
    public string valueDescription { get; set; }
    public List<Supplier> suppliers { get; set; }
    public List<Item> items { get; set; }
    public ContractPeriod contractPeriod { get; set; }
    public string procurementMethod { get; set; }
    public string eTenderProcurementMethod { get; set; }
    public List<string> relatedRFT { get; set; }
    public List<string> relatedSON { get; set; }
    public string costBenefitAnalysis { get; set; }
    public string descriptionContractor { get; set; }
    public string descriptionRenegotiated { get; set; }
    public string descriptionServices { get; set; }
    public int exemptionsProvisions { get; set; }
    public string futureTransfersToContractor { get; set; }
    public string futureTransfersToState { get; set; }
    public string otherInformation { get; set; }
    public string otherKeyElements { get; set; }
    public string otherPrivateSector { get; set; }
    public string provisionsNotProvided { get; set; }
    public string publicSectorComparator { get; set; }
    public string publishIntention { get; set; }
    public string reasons { get; set; }
    public string summaryOfInfo { get; set; }
    public string risk { get; set; }
    public string significantGuarantee { get; set; }
    public ContractGroup contractGroup { get; set; }
}

public class Change
{
    public string property { get; set; }
    public string former_value { get; set; }
}

public class Amendment
{
    public string date { get; set; }
    public List<Change> changes { get; set; }
    public string rationale { get; set; }
}

public class ContactPoint
{
    public string name { get; set; }
    public string email { get; set; }
    public string telephone { get; set; }
    public string mobile { get; set; }
    public string faxNumber { get; set; }
    public string url { get; set; }
}

public class Address
{
    public string streetAddress { get; set; }
    public string locality { get; set; }
    public string region { get; set; }
    public string postalCode { get; set; }
    public string countryName { get; set; }
}

public class Buyer
{
    public string name { get; set; }
    public ContactPoint contactPoint { get; set; }
    public Address address { get; set; }
}

public class Gazetteer
{
    public string scheme { get; set; }
    public List<string> Identifiers { get; set; }
}

public class DeliveryLocation
{
    public Gazetteer gazetteer { get; set; }
}

public class MinValue
{
    public double amount { get; set; }
    public string currency { get; set; }
}

public class HardCopyCost
{
    public double amount { get; set; }
    public string currency { get; set; }
}

public class SoftCopyCost
{
    public double amount { get; set; }
    public string currency { get; set; }
}

public class TenderPeriod
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}

public class Prequalification
{
    public string title { get; set; }
    public string description { get; set; }
}

public class Questionnaire
{
    public string title { get; set; }
    public string description { get; set; }
}

public class Tenderer
{
    public Identifier identifier { get; set; }
    public string name { get; set; }
    public Address address { get; set; }
}

public class Document
{
    public string id { get; set; }
    public string format { get; set; }
    public string title { get; set; }
    public string documentType { get; set; }
    public string url { get; set; }
}


public class Amendments
{
    public string date { get; set; }
    public List<Document> documents { get; set; }
    public string rationale { get; set; }
}

public class Tender
{
    public string id { get; set; }
    public string RFTUUID { get; set; }
    public List<string> relatedRFT { get; set; }
    public List<string> relatedCN { get; set; }
    public List<string> relatedSON { get; set; }
    public string title { get; set; }
    public string multiAgencyAccess { get; set; }
    public string multiAgencyAccessType { get; set; }
    public string description { get; set; }
    public string tenderType { get; set; }
    public string inheritanceType { get; set; }
    public string status { get; set; }
    public string eTenderStatus { get; set; }
    public string RFTAccess { get; set; }
    public DeliveryLocation deliveryLocation { get; set; }
    public List<Item> items { get; set; }
    public string PlannedProcurementUUID { get; set; }
    public MinValue minValue { get; set; }
    public Value value { get; set; }
    public HardCopyCost hardCopyCost { get; set; }
    public string hardCopyLink { get; set; }
    public SoftCopyCost softCopyCost { get; set; }
    public string softCopyLink { get; set; }
    public string procurementMethod { get; set; }
    public string eTenderProcurementMethod { get; set; }
    public List<string> submissionMethod { get; set; }
    public string submissionMethodDetails { get; set; }
    public string addressForLodgement { get; set; }
    public string lodgeResponse { get; set; }
    public string conditionsForParticipation { get; set; }
    public string timeframeDelivery { get; set; }
    public TenderPeriod tenderPeriod { get; set; }
    public List<Prequalification> prequalifications { get; set; }
    public List<string> capabilities { get; set; }
    public List<Questionnaire> questionnaires { get; set; }
    public int numberOfTenderers { get; set; }
    public List<Tenderer> tenderers { get; set; }
    public List<Document> documents { get; set; }
    public Amendments amendments { get; set; }
    public string estimatedDateToMarket { get; set; }
}

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
public class Error
{
    public string message { get; set; }
}

public class eTenderAPIResponse
{
    public List<Release> releases { get; set; }
    public List<Error> errors { get; set; }
}

