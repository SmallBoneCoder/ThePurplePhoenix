using System;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string serverip = null;
            string serverport = null;
            Console.WriteLine("请输入要创建的服务器的ip地址：");
            serverip = Console.ReadLine();
            Console.WriteLine("请输入要创建的服务器的端口号：");
            serverport = Console.ReadLine();
            TestSocketServer server = new TestSocketServer(serverip, serverport);
            //TestSocketAsync server = new TestSocketAsync(serverip, serverport);
            server.StartListen();
            while (true)
            {
                Console.WriteLine("请输入指令");
                string msg = Console.ReadLine();
                if (msg.Equals("exit"))
                {
                    server.CloseServer();
                    break;
                }
            }
        }
    }
}
