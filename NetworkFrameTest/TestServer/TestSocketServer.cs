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
    class RoomInfo
    {
        public string roomId;
        public string roomMaster;
        public bool isPlaying;
        public List<string> members;
        public override string ToString()
        {
            return roomId + "#" + roomMaster + "#" + members.Count + "#" + isPlaying;
        }
    }

    class TestSocketServer
    {
        
        private Socket _serverSocket;//socket对象
        private IPEndPoint _point;//网络节点
        private int _listenCount = 10;//同时监听等待人数

        private const int _bufferLength = 8*8;//缓存大小
        //private byte[] _buffer;//缓存区
        private Dictionary<string, Socket> _allClients;//所有连接的客户端
        private List<string> _allNames;//所有连接的客户端的IP
        private Dictionary<string, string> _allPosition;//所有对象的位置
        private Dictionary<string, byte[]> _allBuffers;//所有客户端的接收缓冲区
        private Dictionary<string, MyParser> _allParser;//所有客户端的解析类
        private List<string> _toBeRemoveNames;//要删除的客户端名字
        private Dictionary<string, string> _allPlayerName;//所有玩家的昵称
        private Dictionary<string, RoomInfo> _allRooms;//所有房间信息
        private Dictionary<string, string> _allPlayerRoomId;//所有玩家所在的房间号
        private static int _roomId = 0;
        //private List<Thread> _threadPool;//线程池
        //private Dictionary<string, ConcurrentQueue<string>> _allSendMessage;
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
            _allBuffers = new Dictionary<string, byte[]>();
            _allParser = new Dictionary<string, MyParser>();
            _toBeRemoveNames = new List<string>();
            _allPlayerName = new Dictionary<string, string>();
            _allRooms = new Dictionary<string, RoomInfo>();
            _allPlayerRoomId = new Dictionary<string, string>();
            //初始化缓存区
            //_buffer = new byte[_bufferLength];
            //初始化线程池
            //_threadPool = new List<Thread>();
            //_allSendMessage = new Dictionary<string, ConcurrentQueue<string>>();
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
                        //_allSendMessage.Add(ipname, new ConcurrentQueue<string>());
                    }

                    //开启线程单独接收消息
                    Thread thread = new Thread(RecieveMsg);
                    thread.IsBackground = true;
                    thread.Start(t_socket);//传入参数
                    //_threadPool.Add(thread);
                    //ThreadPool.QueueUserWorkItem(RecieveMsg, t_socket);
                    //同步客户端
                    //ThreadPool.QueueUserWorkItem(RequestSyncPos);
                    SendData(ipname,
                    new MyParser().EncoderData(
                        1, _serverSocket.LocalEndPoint + "#name#" + ipname
                        ));
                    //SynchronizingAllClient();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
        }

        public void SendRoomMembers(string ip,MyParser parser)
        {
            SendData(ip, parser.EncoderData(1,
                    _serverSocket.LocalEndPoint.ToString() + "#"
                    + "roomMembers#" + _allPlayerName[_allRooms[_allPlayerRoomId[ip]].members[0].ToString()]+"#房主"));
            for (int i = 1; i < _allRooms[_allPlayerRoomId[ip]].members.Count; i++)
            {
                SendData(ip, parser.EncoderData(1,
                    _serverSocket.LocalEndPoint.ToString() + "#"
                    + "roomMembers#" + _allPlayerName[ _allRooms[_allPlayerRoomId[ip]].members[i].ToString()]+"#"));
            }
        }
        //public void RequestSyncPos(object state)
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(20);
        //        TransmitMsg( null, new MyParser().EncoderData(1,_serverSocket.LocalEndPoint + "#Sync#"));
        //    }
        //}
        /// <summary>
        /// 同步客户端
        /// </summary>
        public void SynchronizingAllClient(string ip,List<string> room)
        {
            for(int i = 0; i < room.Count; i++)
            {
                SendData(ip, new MyParser().EncoderData(1,_serverSocket.LocalEndPoint+"#player#" + room[i]
                    +"#"+_allPosition[room[i]]+"#" + _allPlayerName[room[i]]));
            }
        }
        public void SendAllRoomInfo(string ip)
        {
            //for(int i = 0; i < _allRooms[_allPlayerRoomId[ip]].members.Count; i++)
            //{
            //    SendData(ip, new MyParser().EncoderData(1, _serverSocket.LocalEndPoint 
            //        + "#refresh#" +
            //        _allRooms[_allRooms[ip].members[i]].ToString()));
            //}
            foreach(KeyValuePair<string,RoomInfo> pair in _allRooms)
            {
                SendData(ip, new MyParser().EncoderData(1, _serverSocket.LocalEndPoint
                    + "#refresh#" +
                    pair.Value.ToString()));
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client_socket"></param>
        private void RecieveMsg(object client_socket)
        {
            Socket client = client_socket as Socket;
            MyParser parser = new MyParser();
            //开启发送线程
            //Thread thread = new Thread(TransmitThread);
            //thread.IsBackground = true;
            //thread.Start(client.RemoteEndPoint.ToString());
            bool isSync = false;
            byte[] _buffer = new byte[_bufferLength];
            while (true)
            {
                try
                {
                    //接收字节数据并返回长度
                    int n = client.Receive(_buffer);
                    //Console.WriteLine(BitConverter.ToString(_buffer));
                    parser.ConstructPacket(_buffer,n);
                    byte[] temp=null;
                    while ((temp = parser.GetSingleData())!= null)
                    {
                        isSync = false;
                        //将消息转化为字符串
                        
                        string msg =parser.DecoderData(temp);
                        //显示消息
                        Console.WriteLine(msg);
                        string[] data = msg.Split('#');
                        //if (data.Length < 3)
                        //{
                        //    continue;
                        //}
                        switch (data[1])
                        {
                            case "startGame":
                                {
                                    _allRooms[_allPlayerRoomId[data[0]]].isPlaying = true;
                                    data[0] = "";
                                }
                                break;
                            case "synPos":
                                {
                                    _allPosition[data[0]] = data[2];
                                    isSync = true;
                                }break;
                            case "destroy":
                                {
                                    //_allNames.Remove(client.RemoteEndPoint.ToString());
                                    //_allClients.Remove(client.RemoteEndPoint.ToString());
                                    //_allPosition.Remove(client.RemoteEndPoint.ToString());
                                    //_allSendMessage.Remove(client.RemoteEndPoint.ToString());
                                }
                                break;
                            case "login":
                                {
                                    _allPlayerName.Add(client.RemoteEndPoint.ToString(), data[2]);
                                }
                                continue;
                            case "createRoom":
                                {
                                    RoomInfo room = new RoomInfo();
                                    room.roomId = _roomId+"";
                                    _roomId++;
                                    room.isPlaying = false;
                                    room.roomMaster = _allPlayerName[data[0]];
                                    room.members = new List<string>();
                                    room.members.Add(client.RemoteEndPoint.ToString());
                                    _allRooms.Add(room.roomId,room);
                                    _allPlayerRoomId.Add(client.RemoteEndPoint.ToString(), room.roomId);
                                    SendData(client.RemoteEndPoint.ToString(),
                                        parser.EncoderData(1,
                                        _serverSocket.LocalEndPoint + "#createRoom#" + room.roomId));
                                    Console.WriteLine(room.roomId);
                                }
                                continue;
                            case "joinRoom":
                                {
                                    _allRooms[data[2]].members.Add(client.RemoteEndPoint.ToString());
                                    if (!_allPlayerRoomId.ContainsKey(data[0]))
                                    {
                                        _allPlayerRoomId.Add(data[0], data[2]);
                                    }
                                    else
                                    {
                                        _allPlayerRoomId[data[0]]=data[2];
                                    }
                                    data[0] = "";
                                    //TransmitMsg(_allRooms[data[0]].members, temp);
                                }
                                break;
                            case "roomMembers":
                                {
                                    SendRoomMembers(data[0], parser);
                                }
                                continue;
                            case "syncGame":
                                {
                                    SynchronizingAllClient(data[0],
                                        _allRooms[_allPlayerRoomId[data[0]]].members);
                                }continue;
                            case "delMember":
                                {
                                    //data[0]为房主
                                    TransmitMsg(_allRooms[_allPlayerRoomId[data[0]]].members,
                                        temp, null);
                                    string ip = _allPlayerName.FirstOrDefault
                                        (
                                            (p) =>
                                            {
                                                //data[2]为要踢的玩家昵称
                                                return p.Value == data[2];
                                            }
                                        ).Key;
                                    _allRooms[_allPlayerRoomId[ip]].members.Remove(ip);

                                }
                                continue;

                            case "detachRoom":
                                {
                                    //_allPlayerName.
                                    TransmitMsg(_allRooms[_allPlayerRoomId[data[0]]].members,
                                        temp, null);
                                    string roomId = _allPlayerRoomId[data[0]];
                                    for (int i=0;i< _allRooms[roomId].members.Count; i++)
                                    {
                                        Console.WriteLine(_allRooms[roomId].members[i]);
                                        _allPlayerRoomId.Remove(
                                            _allRooms[roomId].members[i]);
                                    }
                                    _allRooms.Remove(roomId);
                                    
                                }
                                continue;
                            case "leaveRoom":
                                {
                                    TransmitMsg(_allRooms[_allPlayerRoomId[data[0]]].members,
                                        temp, null);
                                    //string roomId = _allPlayerRoomId[data[0]];
                                    _allRooms[_allPlayerRoomId[data[0]]].members.Remove(data[0]);
                                    _allPlayerRoomId.Remove(data[0]);
                                }
                                continue;
                            case "refresh":
                                {
                                    SendAllRoomInfo(data[0]);
                                }continue;
                            case "enemy":
                            case "boss":
                            case "BeDamaged":
                                {
                                    TransmitMsg(_allRooms[_allPlayerRoomId[client.RemoteEndPoint.ToString()]].members,
                                        temp, null);
                                }
                                continue;
                        }
                       
                        //转发给其他用户
                        if (!isSync)
                        {
                            TransmitMsg(_allRooms[_allPlayerRoomId[client.RemoteEndPoint.ToString()]].members,
                                temp,data[0]);
                            //_allSendMessage[client.RemoteEndPoint.ToString()].Enqueue(msg);

                        }
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //_allNames.Remove(client.RemoteEndPoint.ToString());
                    //_allClients.Remove(client.RemoteEndPoint.ToString());
                    //_allPosition.Remove(client.RemoteEndPoint.ToString());
                    Disconnected(client.RemoteEndPoint.ToString());
                    break;//断开连接
                }
            }
        }
        ///// <summary>
        ///// 转发线程
        ///// </summary>
        //private void TransmitThread(object name)
        //{
        //    string _name = name as string;
        //    string msg = null;
        //    while (true)
        //    {
        //        if (!_allSendMessage[_name].IsEmpty)
        //        {
        //            _allSendMessage[_name].TryDequeue(out msg);
        //            TransmitMsg(new MyParser (),null, msg);
        //        }
        //    }
        //}
        /// <summary>
        /// 转发消息
        /// </summary>
        /// <param name="room">房间成员</param>
        /// <param name="msg">字节消息</param>
        /// <param name="len">长度</param>
        private void TransmitMsg(List<string> room, byte[] msg,string self)
        {
            try
            {
                //if(ipname!=null)_allClients[ipname].Send(parser.EncoderData((ushort)1, msg));
                for (int i = 0; i < room.Count; i++)
                {
                    //将消息转发给房间用户
                    if (room[i] != self)
                    {
                        SendData(room[i], msg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            RemoveAllClients();
        }
        /// <summary>
        /// 转发消息
        /// </summary>
        /// <param name="ipname">自己的名字</param>
        /// <param name="msg">字节消息</param>
        /// <param name="len">长度</param>
        private void TransmitMsg(string ipname,byte[] msg)
        {
            try
            {
                //if(ipname!=null)_allClients[ipname].Send(parser.EncoderData((ushort)1, msg));
                for (int i = 0; i < _allNames.Count; i++)
                {
                    //将消息转发给其他用户
                    if (_allNames[i] != ipname)
                    {
                        //_allClients[_allNames[i]].Send(msg);
                        //_allClients[_allNames[i]].Send(msg);
                        SendData(_allNames[i], msg);
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            RemoveAllClients();
        }
        private void RemoveAllClients()
        {
            //Console.WriteLine("正在移除客户端");
            foreach (string ipname in _toBeRemoveNames)
            {
                Console.WriteLine("玩家(" + ipname + ")离线");
                Disconnected(ipname);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ipname">客户端ip</param>
        /// <param name="msg">消息</param>
        private void SendData(string ipname, byte[] msg)
        {
            try
            {
                //Console.WriteLine(msg);
                _allClients[ipname].Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Console.WriteLine("玩家("+ipname+")离线");
                _toBeRemoveNames.Add(ipname);//添加进移除名单
            }

        }
        private void Disconnected(string ipname)
        {
            try
            {
                _allClients[ipname].Close();
                _allNames.Remove(ipname);
                _allClients.Remove(ipname);
                _allPosition.Remove(ipname);
                _allBuffers.Remove(ipname);
                _allParser.Remove(ipname);
                _allPlayerName.Remove(ipname);
                if (_allPlayerRoomId.ContainsKey(ipname))
                {
                    if (_allRooms.ContainsKey(_allPlayerRoomId[ipname]))
                    {
                        _allRooms[_allPlayerRoomId[ipname]].members.Remove(ipname);
                    }
                    if (_allRooms[_allPlayerRoomId[ipname]].members.Count <= 0)
                    {
                        _allRooms.Remove(_allPlayerRoomId[ipname]);
                    }
                    _allPlayerRoomId.Remove(ipname);
                }
                Console.WriteLine(GC.GetTotalMemory(false));
                GC.Collect();
                Console.WriteLine(GC.GetTotalMemory(false));
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
