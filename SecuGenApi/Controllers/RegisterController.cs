using Dapper_ORM.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace SecuGenApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        CommonClasses common = new CommonClasses();

        private readonly ILogger<RegisterController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISecuSearch3 _secuSearch3;
        private IImageContainer _imageContainer;
        private IDataPartitioning _dataPartitioning;
        private readonly IDapper _dapper;

        public RegisterController(ILogger<RegisterController> logger
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

        [HttpGet(nameof(Register))]
        public JsonResult Register(UInt32 userid, string isotemplatebase64)
        {
            string sessionid = "0";
            string ourSession = common.OUR_SESSIONID;
            
            //string dataPath = common.GetPathToDataDirectory(_hostingEnvironment);
            //string fullPath = dataPath + "/" + "SessionId";
            //if(File.Exists(fullPath)) {

            string strError = "Fingerprint registered successfully";
            bool bSuccess = false;
            SSError error = SSError.NONE;
            byte[] sg400Template = new byte[SSConstants.TEMPLATE_SIZE];
            byte[] isoTemplateData;
            try
            {
                isoTemplateData = Convert.FromBase64String(isotemplatebase64);
            }
            catch(FormatException whatsWrong)
            {
                return new JsonResult(new
                {
                    Register = (bSuccess ? "true" : "false")
                    ,
                    Session = sessionid
                    ,
                    error = whatsWrong.Message
                });

            }

            //Write the data to the datafile
            //common.FileDownTemplate(userid.ToString(), ref isoTemplateData, ISO_TEMPLATE_FILE_EXTENSION, _hostingEnvironment, /*DIRECTORY_LOCATION, TEMPLATE_FILENAME_BASE*/);
            //CRUD back to MySql db
            //_dapper.

            sessionid = ourSession;

            error = _secuSearch3.LoadFPDB(common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment));
            if (error == SSError.NONE) {
                error = _secuSearch3.ExtractTemplate(isoTemplateData, SSTemplateType.ISO19794, 0, sg400Template);
                if (error == SSError.NONE) {

                    // can file down individual templates
                    //common.FileDownTemplate(userid.ToString(), ref sg400Template, SG400_TEMPLATE_FILE_EXTENSION, _hostingEnvironment, DIRECTORY_LOCATION, TEMPLATE_FILENAME_BASE);
                    //CRUD back to MySql db
                    //_dapper.

                    error = _secuSearch3.RegisterFP(sg400Template, userid);
                    if (error != SSError.NONE) {
                        strError = string.Format("Failed to insert into SS3; Error[{0}]", error.ToString());
                    } // end if error != SSError.NONE
                    else {
                        error = _secuSearch3.SaveFPDB(common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment));
                        if (error != SSError.NONE) {
                            strError = string.Format("Failure to save SS3 DB[{0}]\nError[{1}]"
                                , common.GetFingprintTdbFilewithPath(sessionid, _hostingEnvironment), error.ToString());
                        } // if (error != SSError.NONE)
                        else {
                            // Write data to image container service
                            //_imageContainer.storeBMPBase64Image(common.GetPathToDataDirectory(), sessionid, bmpbase64image);
                            bSuccess = true;
                        }
                    } // end else error == SSError.NONE
                } // if error == SSError.NONE
                else {
                    strError = string.Format("Failed to extract template; Error[{0}]", error.ToString());
                }
            }
            else {
                strError = string.Format("LoadFPDB failure, error[{0}]", error);
            } // end else loadfpdb was successful
            return new JsonResult(new {
                Register = (bSuccess ? "true" : "false")
                ,
                Session = sessionid
                ,
                error = strError
            });
        }

        //[HttpGet(nameof(TestRegister))]
        //public JsonResult TestRegister(string filePath) {

        //    //using (StreamReader sr = new StreamReader(filePath)) {
        //    //    string json = sr.ReadToEnd();
                
        //    //    var parsed! = JsonObject.Parse(json);

        //    //    var alles = JsonNode.Parse(parsed)

        //    //    var dIdList = parsed["ISOTemplateBase64"];



        //    //    //JsonSerializer.Deserialize(json);

        //    //    //IList<string> lines = new List<string>();



        //    //}

        //    return new JsonResult(new {});
        //}


    }
}
