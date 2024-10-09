using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace SecuGenApi {
    public class CommonClasses
    {
        public readonly string SS3_FP_DATA = "FingerprintData_{0}.tdb";
        public readonly string DIRECTORY_LOCATION = "Data";
        public readonly string TEMPLATE_FILENAME_BASE = "Template_";
        public const string SG400_TEMPLATE_FILE_EXTENSION = ".min";
        public const string ISO_TEMPLATE_FILE_EXTENSION = ".iso";

        public readonly string OUR_SESSIONID = "63814060294766.48";

        public string GetPathToDataDirectory(IWebHostEnvironment? _hostingEnvironment) {
            string contentRootPath = _hostingEnvironment!.ContentRootPath;
            string dataContent = Path.Combine(contentRootPath, DIRECTORY_LOCATION);

            //BREAK THE CHAIN AND FIX THE POSITION
            string path = "C:\\WebSites\\Data";
            dataContent = path;
            if (!Directory.Exists(dataContent)) {
                Directory.CreateDirectory(dataContent);
            }

            return dataContent;
        } // end GetFingerprintTbdFileWithPath

        public string GetFingprintTdbFilewithPath(string strSessionID, IWebHostEnvironment? _hostingEnvironment) {
            string dataContent = GetPathToDataDirectory(_hostingEnvironment);
            return Path.Combine(dataContent, string.Format(SS3_FP_DATA, strSessionID));
        } // end GetFingerprintTbdFileWithPath

        public void FileDownTemplate(string userid, ref byte[] sg400Template, string strTemplateExtension, IWebHostEnvironment? _hostingEnvironment) {
            string dataPath = GetPathToDataDirectory(_hostingEnvironment);

            string strTemplateName = Path.Combine(dataPath, TEMPLATE_FILENAME_BASE + userid + strTemplateExtension);
            using (FileStream fs = System.IO.File.Create(strTemplateName)) {
                fs.Write(sg400Template, 0, SSConstants.TEMPLATE_SIZE);
            }
        }

    }
}
