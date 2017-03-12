using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heimdall.Server;

namespace Heimdall.ServerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            HeimdallServerConfig config = new HeimdallServerConfig()
            {
                Port = 11337,
                MaxConnections = 4,
            };

            HeimdallServer server = new HeimdallServer(config, new Logger());
            server.Run();
        }
    }
}
