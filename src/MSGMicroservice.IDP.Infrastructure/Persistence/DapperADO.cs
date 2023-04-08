using Microsoft.Extensions.Configuration;
using MSGMicroservice.IDP.Infrastructure.Common;

namespace MSGMicroservice.IDP.Infrastructure.Persistence
{
    public class DapperADO
    {
        public string ConnectionStringLog { get; set; }
        //public string ConnectStr_Fin { get; set; }
        //public string ConnectStr_LTR { get; set; }
        //public string ConnectionStringAbr { get; set; }

        public DapperADO(IConfiguration configuration)
        {
            ConnectionStringLog = configuration.GetConnectionString(ConnectionString.ConnectionStringLog);

            //ConnectStr_LTR = configuration.GetConnectionString(ConnectionString.ConnectStr_LTR);
            //ConnectStr_Fin = configuration.GetConnectionString(ConnectionString.ConnectStr_Fin);
            //ConnectionStringAbr = configuration.GetConnectionString(ConnectionString.ConnectionStringAbr);
        }
    }
}