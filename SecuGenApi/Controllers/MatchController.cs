using Dapper_ORM.Services;
using Microsoft.AspNetCore.Mvc;
using SecuGenApi.Models;
using System.Diagnostics.CodeAnalysis;
using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace SecuGenApi.Controllers {
    //[Route("api/[controller]")]
    [Route("api/")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        CommonClasses common = new CommonClasses();

        private readonly ILogger<MatchController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISecuSearch3 _secuSearch3;
        private IImageContainer _imageContainer;
        private IDataPartitioning _dataPartitioning;
        private readonly IDapper _dapper;

        public MatchController(ILogger<MatchController> logger
            , ISecuSearch3 secuSearch3
            , IWebHostEnvironment webHostEnvironment
            , IImageContainer imageContainer
            , IDataPartitioning dataPartitioning
            , [NotNull] IDapper dapper)

        {
            _logger = logger;
            _secuSearch3 = secuSearch3;
            _hostingEnvironment = webHostEnvironment;
            _imageContainer = imageContainer;
            _dataPartitioning = dataPartitioning;
            _dapper = dapper;
        }

        [HttpGet(nameof(Match))]
        public JsonResult Match(string isotemplatebase64/*, string sessionid*/)
        {
            string sessionid = "";
            string strError = "Identify Person Match successfully";
            bool bSuccess = false;
            var matchedPersonTemplateId = 0;
            SearchResultsModel searchResultsModel = new SearchResultsModel();
            string ourSession = common.OUR_SESSIONID;

            sessionid = ourSession;
            _dataPartitioning.SetClientSessionInfo(sessionid);

            var error = _secuSearch3.LoadFPDB(common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment));
            if (error == SSError.NONE) {
                byte[] sg400Template = new byte[SSConstants.TEMPLATE_SIZE];
                byte[] webApiFingerprintData = Convert.FromBase64String(isotemplatebase64);
                error = _secuSearch3.ExtractTemplate(webApiFingerprintData, SSTemplateType.ISO19794, 0, sg400Template);
                if (error == SSError.NONE) {
                    searchResultsModel.SearchIdentity = true;

                    // File down template that is used as matcher
                    // FileDownTemplate("Identify", ref sg400Template);

                    //CRUD back to MySql db
                    //_dapper.

                    UInt32 foundMatchTemplateId = 0;
                    error = _secuSearch3.IdentifyFP(sg400Template, SSConfLevel.NORMAL, ref foundMatchTemplateId);
                    if (error == SSError.NONE) {
                        matchedPersonTemplateId = (Int32)foundMatchTemplateId;
                        searchResultsModel.SearchResultIdentity = (Int32)foundMatchTemplateId;
                        bSuccess = true;
                    }
                    else {
                        strError = string.Format("IdentifyFP failure; error [{0}];", error.ToString());
                    }
                }
                else {
                    strError = string.Format("ExtractTemplate failure; error [{0}];", error.ToString());
                }
            } // end if LoadFPDB failure
            else {
                strError = string.Format("Load FP DB failure; error [{0}];", error.ToString());
            }

            //ViewData["AllImages"] = _imageContainer.getBMPBase64Image(common.GetPathToDataDirectory(), sessionid);
            return new JsonResult(new {
                found = (bSuccess ? "true" : "false")
                ,
                match = searchResultsModel.SearchResultIdentity
                ,
                error = strError
            });
        }

    }
}
