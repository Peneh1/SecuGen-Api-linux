using WebAPI1toN.Interfaces;

// MVC ASP.NET Core 6 Web API -> 9/16/22
// This service is now more about session management and the ability to generate a new sessionid.
// At this point, this doesn't need to be service, however, due to WebAPI1toN requirement
// that each user must have unique fingerprint DB, this can live on as a session id generator.

namespace WebAPI1toN.Services {
    public class DataPartitioning : IDataPartitioning
    {
        private string ClientSessionInfo { get; set; }

        public string GetClientSessionInfo() 
        { return ClientSessionInfo; } 

        public void SetClientSessionInfo(string sessionid)
        { ClientSessionInfo = sessionid; }

        public DataPartitioning(string clientSessionInfo)
        { ClientSessionInfo = clientSessionInfo; }

        public DataPartitioning()
        { ClientSessionInfo = string.Empty; }

        public void setSessionInfo()
        {
            // Here is the core ability of the Web Developer to either:
            // 1.  Have multiple SS3 template DB's 
            // 2.  Have a single SS3 template DB
            // WebAPI1toN requires that each user have a unique template DB.   SecuGen doesn't want to have 
            // user #1 templates get into user #2 template DB.   This would give user #1 a bad feeling about SS3.
            // However, most companies would prefer that there is a single template DB, that houses all templates
            // into a single DB and have all users read from that single DB.
            // WebAPI1toN installation
            DateTime dt1970 = new DateTime();
            DateTime current = DateTime.Now;
            TimeSpan span = current - dt1970;
            ClientSessionInfo = span.TotalMilliseconds.ToString();

            // Most companies need this implemenation (single template DB)
            // ClientSessionInfo = "-";
        }

    }
}
