using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    class TestSocketServer
    {
        private Socket _serverSocket;//socket对象
        private IPEndPoint _point;//网络节点
        private int _listenCount = 10;//同时监听等待人数

        private const int _bufferLength = 8*8;//缓存大小
        //private byte[] _buffer;//缓存区
        private Dictionary<string, Socket> _allClients;//所有连接的客户端
        private List<string> _allNames;//所有连接的客户端的名字
        private Dictionary<string, string> _allPosition;//所有对象的位置
        //private List<Thread> _threadPool;//线程池
        private Dictionary<string, ConcurrentQueue<string>> _allSendMessage;
        public TestSocketServer(string ip,string port)
        {
            InitServer(ip, port);
        }


        public void InitServer(string ipaddr,string port)
        {
            //转换IP地址
            IPAddress ipaddress = IPAddress.Parse(ipaddr);
            //转换端口号
            _point= new IPEndPoint(ipaddress, int.Parse(port));
            //创建监听的Socket
            _serverSocket = new Socket(AddressFamily.InterNetwork, //IPV4地址
                SocketType.Stream, //流式连接（长连接）
                ProtocolType.Tcp //TCP传输协议
                );
            //初始化客户端字典
            _allClients = new Dictionary<string, Socket>();
            _allNames = new List<string>();
            _allPosition = new Dictionary<string, string>();
            //初始化缓存区
            //_buffer = new byte[_bufferLength];
            //初始化线程池
            //_threadPool = new List<Thread>();
            _allSendMessage = new Dictionary<string, ConcurrentQueue<string>>();
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        public void StartListen()
        {
            try
            {
                //绑定网络结点
                _serverSocket.Bind(_point);
                //监听队列长度（等待人数）
                _serverSocket.Listen(_listenCount);
                //开始监听
                Console.WriteLine("服务器开始监听");
                Thread thread = new Thread(AcceptConnection);
                thread.IsBackground = true;
                thread.Start(_serverSocket);
                //_threadPool.Add(thread);
                //Thread trans = new Thread(TransmitThread);
                //trans.IsBackground = true;
                //trans.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// 接收连接
        /// </summary>
        /// <param name="socket">服务器socket</param>
        private void AcceptConnection(object socket)
        {
            Socket server = socket as Socket;
            Socket t_socket=null;
            while (true)
            {
                try
                {
                    //接收一个连接
                    t_socket = server.Accept();
                    //获取客户端信息
                    string ipname = t_socket.RemoteEndPoint.ToString();
                    //消息提示
                    Console.WriteLine(ipname);
                    //添加进客户端列表
                    if (!_allNames.Contains(ipname))
                    {
                        _allNames.Add(ipname);
                        _allClients.Add(ipname, t_socket);
                        _allPosition.Add(ipname, "(0,0,0)");
                        _allSendMessage.Add(ipname, new ConcurrentQueue<string>());
                    }
                    
                    //开启线程单独接收消息
                    //Thread thread = new Thread(RecieveMsg);
                    //thread.IsBackground = true;
                    //thread.Start(t_socket);//传入参数
                    //_threadPool.Add(thread);
                    ThreadPool.QueueUserWorkItem(RecieveMsg, t_socket);
                    //同步客户端
                    //ThreadPool.QueueUserWorkItem(RequestSyncPos);
                    SynchronizingAllClient();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
        }


        public void RequestSyncPos(object state)
        {
            while (true)
            {
                Thread.Sleep(20);
                TransmitMsg(null, _serverSocket.LocalEndPoint + "#Sync#");
            }
        }
        /// <summary>
        /// 同步所有客户端
        /// </summary>
        public void SynchronizingAllClient()
        {
            for(int i = 0; i < _allNames.Count; i++)
            {
                TransmitMsg(null, _serverSocket.LocalEndPoint+"#player#" + _allNames[i]
                    +"#"+_allPosition[_allNames[i]]);
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client_socket"></param>
        private void RecieveMsg(object client_socket)
        {
            Socket client = client_socket as Socket;
            //开启发送线程
            Thread thread = new Thread(TransmitThread);
            thread.IsBackground = true;
            thread.Start(client.RemoteEndPoint.ToString());
            bool isSync = false;
            byte[] _buffer = new byte[_bufferLength];
            while (true)
            {
                try
                {
                    //接收字节数据并返回长度
                    int n = client.Receive(_buffer);
                    //Console.WriteLine(BitConverter.ToString(_buffer));
                    MyParser.ConstructPacket(_buffer,n);
                    byte[] temp=null;
                    while ((temp = MyParser.GetSingleData())!= null)
                    {
                        isSync = false;
                        //将消息转化为字符串
                        string msg =MyParser.DecoderData(temp);
                        string[] data = msg.Split('#');
                        switch (data[1])
                        {
                            case "synPos":
                                {
                                    _allPosition[data[0]] = data[2];
                                    isSync = true;
                                }break;
                            case "destroy":
                                {
                                    _allNames.Remove(client.RemoteEndPoint.ToString());
                                    _allClients.Remove(client.RemoteEndPoint.ToString());
                                    _allPosition.Remove(client.RemoteEndPoint.ToString());
                                    _allSendMessage.Remove(client.RemoteEndPoint.ToString());
                                }
                                break;
                        }
                        //显示消息
                        
                        //转发给其他用户
                        if (!isSync)
                        {
                            //TransmitMsg(client.RemoteEndPoint.ToString(), msg);
                            _allSendMessage[client.RemoteEndPoint.ToString()].Enqueue(msg);

                        }
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    _allNames.Remove(client.RemoteEndPoint.ToString());
                    _allClients.Remove(client.RemoteEndPoint.ToString());
                    _allPosition.Remove(client.RemoteEndPoint.ToString());
                    break;//断开连接
                }
            }
        }
        /// <summary>
        /// 转发线程
        /// </summary>
        private void TransmitThread(object name)
        {
            string _name = name as string;
            string msg = null;
            while (true)
            {
                if (!_allSendMessage[_name].IsEmpty)
                {
                    _allSendMessage[_name].TryDequeue(out msg);
                    TransmitMsg(null, msg);
                }
                
                
  
            }
        }
        /// <summary>
        /// 转发消息
        /// </summary>
        /// <param name="ipname">自己的名字</param>
        /// <param name="msg">字节消息</param>
        /// <param name="len">长度</param>
        private void TransmitMsg(string ipname,string msg)
        {
            try
            {
                for (int i = 0; i < _allNames.Count; i++)
                {
                   
                    _allClients[_allNames[i]].Send(MyParser.EncoderData((ushort)1, msg));
                    
                    ////将消息转发给其他用户
                    //if (_allNames[i] != ipname)
                    //{
                    //    //_allClients[_allNames[i]].Send(msg);
                    //    _allClients[_allNames[i]].Send(MyParser.EncoderData((ushort)1, msg));
                    //}
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void CloseServer()
        {
            try
            {
                //先关闭子连接
                for (int i = 0; i < _allNames.Count; i++)
                {
                    _allClients[_allNames[i]].Close();
                }
                //关闭主连接
                _serverSocket.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
