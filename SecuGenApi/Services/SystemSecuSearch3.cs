using System.Runtime.InteropServices;
using WebAPI1toN.Interfaces;
using WebAPI1toN.SecuSearchSDK3;

namespace WebAPI1toN.Services
{
    public class SystemSecuSearch3 : ISecuSearch3
    {
        public SecuSearch _SS3 { get; set; }

        public SSParam _SSParam { get; set; }

        public SystemSecuSearch3()
        {
            _SS3 = new SecuSearch();
            SSParam param = new SSParam
            {
                CandidateCount = 10,
                Concurrency = 0,
                LicenseFile = "",
                EnableRotation = true
            };
            _SSParam = param;

            _SS3.InitializeEngine(param);
        }

        ~SystemSecuSearch3()
        {
            _SS3.TerminateEngine();
        }

        public SSError InitializeEngine(ref SSParam param)
        {
            return _SS3.InitializeEngine(param);
        }

        public SSError TerminateEngine()
        {
            return _SS3.TerminateEngine();
        }

        public SSError GetEngineParam(ref SSParam param)
        {
            return _SS3.GetEngineParam(ref param);
        }

        public SSError RegisterFP(Byte[] sgTemplate, UInt32 templateId)
        {
            return _SS3.RegisterFP(sgTemplate, templateId);
        }

        public SSError RegisterFPBatch(SSIdTemplatePair[] pairs, UInt64 count)
        {
            return _SS3.RegisterFPBatch(pairs, (int)count);
        }

        public SSError RemoveFP(UInt32 templateId)
        {
            return _SS3.RemoveFP(templateId);
        }

        public SSError RemoveFPBatch(UInt32[] templateIds, UInt64 count)
        {
            return _SS3.RemoveFPBatch(templateIds, (int)count);
        }

        public SSError SearchFP(Byte[] sgTemplate, ref SSCandList candList)
        {
            return _SS3.SearchFP(sgTemplate, ref candList);
        }

        public SSError IdentifyFP(Byte[] sgTemplate, SSConfLevel seculevel, ref UInt32 templateId)
        {
            return _SS3.IdentifyFP(sgTemplate, seculevel, ref templateId);
        }

        public SSError SaveFPDB(String filename)
        {
            return _SS3.SaveFPDB(filename);
        }

        public SSError LoadFPDB(String filename)
        {
            return _SS3.LoadFPDB(filename);
        }

        public SSError ClearFPDB()
        {
            return _SS3.ClearFPDB();
        }

        public SSError GetFPCount(ref UInt64 count)
        {
            SSError error = SSError.NONE;
            Int32 count32 = 0;
            _SS3.GetFPCount(ref count32);
            count = Convert.ToUInt64(count32);
            return error;
        }

        public SSError GetIDList(UInt32[] idList, Int32 maxCount, ref Int32 count)
        {
            return _SS3.GetIDList(idList, maxCount, ref count);
        }

        public SSError GetTemplate(UInt32 templateId, Byte[] sgTemplate)
        {
            return _SS3.GetTemplate(templateId, sgTemplate);
        }

        public SSError ExtractTemplate(Byte[] standardTemplate, SSTemplateType templateType, UInt32 indexOfView, Byte[] sgTemplate)
        {
            return _SS3.ExtractTemplate(standardTemplate, templateType, indexOfView, sgTemplate);
        }

        public SSError GetNumberOfView(Byte[] standardTemplate, SSTemplateType templateType, ref UInt32 numberOfView)
        {
            SSError error = SSError.NONE;
            error = _SS3.GetNumberOfView(standardTemplate, templateType, ref numberOfView);
            return error;
        }

        public IntPtr GetVersion()
        {
            string str = _SS3.GetVersion();
            return Marshal.StringToCoTaskMemAnsi(str);
        }
    } // end class SystemSecuSearch3
}
