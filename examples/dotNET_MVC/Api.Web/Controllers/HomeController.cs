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
            //var cn = new cn() { CNUUID = "F7EABE30-D193-79E8-4EA6EC13105DE46C" };
            //db.SaveChanges();
            var requestHelper = new RequestHelper();
            //var baseUrl = "http://ec2-54-252-210-13.ap-southeast-2.compute.amazonaws.com/";
            var baseUrl = "http://tenders.nsw.gov.au/";
            var result = "<table><tr><th>Planned Procurement</th><th>Tenders</th><th>Contracts</th></tr>";

            // valid search result              ?event=public.api.planning.search&ResultsPerPage=99
            // fail with "errors"               ?event=public.api.tender.view&RFTUUID=DCBEFDDE-CFF9-430D-4DFAE339EA77430
            eTenderAPIResponse jsonResponse = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.search&ResultsPerPage=99");

            foreach (Release release in jsonResponse.releases)
            {
                var tender = release.tender;
                string ppUuid = tender.PlannedProcurementUUID;
                eTenderAPIResponse plannedProcurement = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid);
                await Task.Delay(3000);

                var rftArray = tender.relatedRFT;
                var rftCount = rftArray?.Count ?? 0;
                var ppTender = plannedProcurement.releases[0].tender;
                result += "<tr><td rowspan='" + rftCount + "'><a href=\"" + baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid + "\">";
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