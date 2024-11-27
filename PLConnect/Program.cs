// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Runtime.InteropServices;
using EasyModbus;
using PLConnect;
using System.Net.Security;

class Program
{
    static void Main(string[] args)
    {

        var PLCService  = new PLCService("192.168.0.199", 102);

        //PLCService.Connect();
        PLCService.ConnectSiementPLC("192.168.0.199",0,1);

        PLCService.SendCommand();
        PLCService.ReadArea(1,1);

        PLCService.DisConnectSiemensPLC();



    }
}
