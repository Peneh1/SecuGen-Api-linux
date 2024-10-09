namespace WebAPI1toN.Interfaces {
    public interface IDataPartitioning
    {
        string GetClientSessionInfo();
        void setSessionInfo();
        void SetClientSessionInfo(string sessionid);

    }
}
