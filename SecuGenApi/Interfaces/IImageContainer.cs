namespace WebAPI1toN.Interfaces
{
    public interface IImageContainer
    {
        void storeBMPBase64Image(string strPath, string strSessionID, string strbmpbase64Image);

        List<string> getBMPBase64Image(string strPath, string strSessionID);
    }
}
