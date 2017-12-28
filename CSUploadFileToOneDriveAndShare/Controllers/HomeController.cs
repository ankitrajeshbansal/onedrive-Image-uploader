using CSOneDriveAccess;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace CSUploadFileToOneDriveAndShare.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// clientId of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string ClientId = "<client ID>";
        /// <summary>
        /// Password/Public Key of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string Secret = "Secret key";
        /// <summary>
        /// Authentication callback url, you can set it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string CallbackUri = "http://localhost:1438/Home/OnAuthComplate";

        /// <summary>
        /// OfficeAccessSession object in session
        /// </summary>
        public O365RestSession OfficeAccessSession
        {
            get
            {
                var officeAccess = Session["OfficeAccess"];
                if (officeAccess == null)
                {
                    officeAccess = new O365RestSession(ClientId, Secret, CallbackUri);
                    Session["OfficeAccess"] = officeAccess;
                }
                return officeAccess as O365RestSession;
            }
        }

        public ActionResult Index()
        {
            //if user is not login, redirect to office 365 for authenticate
            if (string.IsNullOrEmpty(OfficeAccessSession.AccessToken))
            {
                string url = OfficeAccessSession.GetLoginUrl("onedrive.readwrite");

                return new RedirectResult(url);
            }
            return View();
        }

        //when user complate authenticate, will be call back this url with a code
        public async Task<ActionResult> OnAuthComplate(string access_token)
        {
            if (string.IsNullOrEmpty(access_token))
                return View();
            await OfficeAccessSession.RedeemTokensAsync(access_token);
            
            return new RedirectResult("Index");
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileAndGetShareUri(HttpPostedFileBase file)
        {
            //save upload file to temp dir in local disk
            var path = Path.GetTempFileName();
            file.SaveAs(path);

            //upload the file to oneDrive and get a file id
            string oneDrivePath = file.FileName;

            string result = await OfficeAccessSession.UploadFileAsync(path, oneDrivePath);

            JObject jo = JObject.Parse(result);
            string fileId = jo.SelectToken("id").Value<string>();

            //request oneDrive REST API with this file id to get a share link
            string shareLink = await OfficeAccessSession.GetShareLinkAsync(fileId, OneDriveShareLinkType.view, OneDrevShareScopeType.anonymous);

            ViewBag.ShareLink = shareLink;

            return View();
        }
    }
}