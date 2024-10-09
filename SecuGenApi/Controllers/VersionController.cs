using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace SecuGenApi.Controllers {
    //[Route("api/[controller]")]
    [Route("api/")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        CommonClasses common = new CommonClasses();

        private readonly ILogger<VersionController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISecuSearch3 _secuSearch3;
        //private IImageContainer _imageContainer;
        private IDataPartitioning _dataPartitioning;

        public VersionController([NotNull] ILogger<VersionController> logger
            , ISecuSearch3 secuSearch3
            , IWebHostEnvironment webHostEnvironment
            //, IImageContainer imageContainer
            , IDataPartitioning dataPartitioning
            ) {
            _logger = logger;
            _secuSearch3 = secuSearch3;
            _hostingEnvironment = webHostEnvironment;
            //_imageContainer = imageContainer;
            _dataPartitioning = dataPartitioning;

        }
        /// <summary>
        /// Version
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(Version))]
        public JsonResult Version()
        {
            Version? v = Assembly.GetExecutingAssembly().GetName().Version;
            var msg = v!.ToString();
            string ourSessionId = common.OUR_SESSIONID;

            _logger.LogInformation("Get - Version {0}", msg);

            string strError = "Fingerprint registered successfully";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            string sessionid = ourSessionId/*"0"*/;

            _secuSearch3.ClearFPDB();
            _dataPartitioning.setSessionInfo();
            //sessionid = _dataPartitioning.GetClientSessionInfo();
            var version = _secuSearch3.GetVersion();
            if (error != SSError.NONE) {
                strError = string.Format("Save Empty FPDB FAILURE, error[{0}]", error);
                return new JsonResult(new {
                    isOk = (bSuccess ? "true" : "false")
                    ,
                    Session = sessionid
                    ,
                    error = strError
                });
            }
            string ourVersion = "0";
            if (!string.IsNullOrEmpty(msg))
            {
                bSuccess = true;
                ourVersion = msg;
            }
            else
                ourVersion = "Unknown";

            return new JsonResult(new {
                isOk = (bSuccess ? "true" : "false")
                ,
                session = sessionid
                ,
                details = String.Format("SecuGenApi = {0}, SecuSearch = {1}", ourVersion, version)
            });

        }
    }
}
