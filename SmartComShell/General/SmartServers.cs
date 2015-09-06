using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    
    public class SmartServer
    {
        public string Address { set; get; }
        public ushort Port { set; get; }
    }


    public static class SmartServers
    {
        
        static SmartServers()
        {
            Matrix = new SmartServer() { Address = "aaa", Port = 1234 };
            Demo = new SmartServer() { Address = "aaa", Port = 1234 };
            Reserve1 = new SmartServer() { Address = "aaa", Port = 1234 };
            Reserve2 = new SmartServer() { Address = "aaa", Port = 1234 };
        }   
        
        public static SmartServer Matrix { private set; get; }
        public static SmartServer Demo { private set; get; }
        public static SmartServer Reserve1 { private set; get; }
        public static SmartServer Reserve2 { private set; get; }

        public static SmartServer GetServer(eSmartServer server)
        { 
            if (server == eSmartServer.Matrix)
            {
                return SmartServers.Matrix;
            }
            else if (server == eSmartServer.Demo)
            {
                return SmartServers.Demo;
            }
            else if (server == eSmartServer.Reserve1)
            {
                return SmartServers.Reserve1;
            }
            else if (server == eSmartServer.Reserve2)
            {
                return SmartServers.Reserve2;
            }
            return null;
        }


    }


}
