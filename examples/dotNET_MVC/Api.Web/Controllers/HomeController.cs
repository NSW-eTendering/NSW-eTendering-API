using System.Web.Mvc;
using System.Threading.Tasks;
using Api.Web.Models;
using Api.Web.Utilities;

namespace Api.Web.Controllers
{
    public class HomeController : Controller
    {
        //private EtrDbContext db = new EtrDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ParsePlannedProcurements()
        {
            var requestHelper = new RequestHelper();
            var baseUrl = "https://tenders.nsw.gov.au/";
            var result = "<table class=\"table table-bordered\" ><tr><th>Planned Procurement</th><th>Tenders</th><th>Contracts</th></tr>";

            eTenderAPIResponse jsonResponse = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.search&ResultsPerPage=5");

            foreach (Release release in jsonResponse.releases)
            {
                var tender = release.tender;
                string ppUuid = tender.PlannedProcurementUUID;
                eTenderAPIResponse plannedProcurement = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid);
                await Task.Delay(3000);

                var rftArray = tender.relatedRFT;
                var rftCount = rftArray?.Count ?? 0;
                var ppTender = plannedProcurement.releases[0].tender;
                result += "<tr><td><a href=\"" + baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid + "\">";
                result += ppTender.id + "</a><br/>Estimated Date of Approach to Market: " + ppTender.estimatedDateToMarket + "</td>";

                if (rftCount > 0)
                {
                    var rftIndex = 0;
                    foreach (var rftUuid in rftArray)
                    {
                        result += "<td>";
                        eTenderAPIResponse rft = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.tender.view&RFTUUID=" + rftUuid);
                        var rftTender = rft.releases[0].tender;
                        result += "<a href=\"" + baseUrl + "?event=public.rft.show&RFTUUID=" + rftUuid + "\">";
                        result += rftTender.title + "</a><br/>Published: " + rftTender.tenderPeriod.startDate + "</td>";
                        result += "<td>";
                        if (rftTender.relatedCN != null)
                        {

                            foreach (var cnUuid in rftTender.relatedCN)
                            {
                                eTenderAPIResponse contract = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&CNUUID=" + cnUuid);
                                result += "<a href='" + baseUrl + "?event=public.cn.view&CNUUID=" + cnUuid + "'>";
                                result += contract.releases[0].awards[0].title + "</a><br/>";
                                // the server will block your IP address if you make too many responses in a short time
                                await Task.Delay(3000);
                            }
                        }
                        if (rftTender.relatedSON != null)
                        {
                            foreach (var sonUuid in rftTender.relatedSON)
                            {
                                eTenderAPIResponse contract = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&SONUUID=" + sonUuid);
                                result += "<a href='" + baseUrl + "?event=public.SON.view&SONUUID=" + sonUuid + "'>";
                                result += contract.releases[0].awards[0].title + "</a><br/>";
                                await Task.Delay(3000);
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

            ViewBag.Result = result;
            return View();
        }
    }
}