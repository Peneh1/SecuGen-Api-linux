using Dapper_ORM.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace SecuGenApi.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/")]
    [ApiController]
    public class UtilitiesController : ControllerBase {

        CommonClasses common = new CommonClasses();

        private readonly ILogger<UtilitiesController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISecuSearch3 _secuSearch3;
        //private IImageContainer _imageContainer;
        private IDataPartitioning _dataPartitioning;
        private readonly IDapper _dapper;

        public UtilitiesController(ILogger<UtilitiesController> logger
            , ISecuSearch3 secuSearch3
            , IWebHostEnvironment webHostEnvironment
            //, IImageContainer imageContainer
            , IDataPartitioning dataPartitioning
            , [NotNull] IDapper dapper)
        {
            _logger = logger;
            _secuSearch3 = secuSearch3;
            _hostingEnvironment = webHostEnvironment;
            //_imageContainer = imageContainer;
            _dataPartitioning = dataPartitioning;
            _dapper = dapper;
        }

        [HttpGet(nameof(Clear))]
        public JsonResult Clear()
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Clear Action success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);
            var dbFile = common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment);
            if (dbFile != null)
                error = _secuSearch3.ClearFPDB();
            if (error == SSError.NONE)
                bSuccess = true;

            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                error = strError
            });
        }

        [HttpGet(nameof(GetFPCount))]
        public JsonResult GetFPCount()
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Get FingerPrint Count success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);

            ulong count = 0;
            error = _secuSearch3.GetFPCount(ref count);
            if (error == SSError.NONE)
            {
                bSuccess = true;
            }
            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                FPCount = count.ToString()
                ,
                error = strError
            });
        }

        [HttpGet(nameof(GetIDList))]
        public JsonResult GetIDList()
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Get Pinger Print Id List success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);

            UInt64 fpCount = 0;
            Int32 returnedFPCount = 0;
            UInt32[] idlist = { };
            error = _secuSearch3.GetFPCount(ref fpCount);
            if (error == SSError.NONE)
            {
                returnedFPCount = 0;
                idlist = new UInt32[fpCount];
                error = _secuSearch3.GetIDList(idlist, (Int32)fpCount, ref returnedFPCount);
            }
            if (error == SSError.NONE)
            {
                bSuccess = true;
            }
            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            String arrTostr = "";
            if (returnedFPCount > 0)
            {
                int[] arr = (int[])(object)idlist;
                arrTostr = arr.Select(a => a.ToString()).Aggregate((i, j) => i + "," + j);
            }

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                FPCount = fpCount.ToString()
                ,
                fingerList = arrTostr
                ,
                error = strError
            });
        }

        [HttpGet(nameof(LoadFPDB))]
        public JsonResult LoadFPDB()
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Load FP Database success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);
            var dbFile = common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment);

            error = _secuSearch3.LoadFPDB(dbFile);
            if (error == SSError.NONE)
            {
                bSuccess = true;
            }
            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                error = strError
            });
        }

        [HttpGet(nameof(RemoveFP))]
        public JsonResult RemoveFP(uint templateId)
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Remove FP identity from Database success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);
            var dbFile = common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment);

            error = _secuSearch3.RemoveFP(templateId);
            if (error == SSError.NONE)
                bSuccess = true;
            else
                strError = error.ToString();

            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                error = strError
            });
        }

        [HttpGet(nameof(SaveFPDB))]
        public JsonResult SaveFPDB()
        {
            string ourSessionId = common.OUR_SESSIONID;
            string sessionid = "";
            string strError = "Utility Action success";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            sessionid = ourSessionId;
            _dataPartitioning.SetClientSessionInfo(sessionid);
            var dbFile = common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment);

            error = _secuSearch3.SaveFPDB(dbFile);
            if (error == SSError.NONE)
            {
                bSuccess = true;
            }
            _logger.LogDebug("Utilities Clear {0}", bSuccess);

            return new JsonResult(new
            {
                actionSuccess = (bSuccess ? "true" : "false")
                ,
                SessionID = sessionid
                ,
                error = strError
            });
        }

    }

}