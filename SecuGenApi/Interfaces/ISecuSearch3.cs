
using WebAPI1toN.SecuSearchSDK3;

namespace WebAPI1toN.Interfaces {
    public interface ISecuSearch3
    {
        SSError InitializeEngine(ref SSParam param);

        SSError TerminateEngine();

        SSError GetEngineParam(ref SSParam param);

        SSError RegisterFP(Byte[] sgTemplate, UInt32 templateId);

        SSError RegisterFPBatch(SSIdTemplatePair[] pairs, UInt64 count);

        SSError RemoveFP(UInt32 templateId);

        SSError RemoveFPBatch(UInt32[] templateIds, UInt64 count);

        SSError SearchFP(Byte[] sgTemplate, ref SSCandList candList);

        SSError IdentifyFP(Byte[] sgTemplate, SSConfLevel seculevel, ref UInt32 templateId);

        SSError SaveFPDB(String filename);

        SSError LoadFPDB(String filename);

        SSError ClearFPDB();

        SSError GetFPCount(ref UInt64 count);

        SSError GetIDList(UInt32[] idList, Int32 maxCount, ref Int32 count);

        SSError GetTemplate(UInt32 templateId, Byte[] sgTemplate);

        SSError ExtractTemplate(Byte[] standardTemplate, SSTemplateType templateType, UInt32 indexOfView, Byte[] sgTemplate);

        SSError GetNumberOfView(Byte[] standardTemplate, SSTemplateType templateType, ref UInt32 numberOfView);

        IntPtr GetVersion();
    }
}
