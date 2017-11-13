using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class MyParser
    {
        //private const string _head = "HH";
        //private const string _tail = "TT";

        private  Queue<List<byte>> _packets=new Queue<List<byte>> ();
        private  List<byte> _currentPacket=new List<byte> ();
        //private static byte[] _cmdType_byte;
        private  byte[] _cmdContent_byte;
        private  byte[] _head_byte;
        //private byte[] _tail_byte;
        private void initProtocol()
        {
            
            
        }

        /// <summary>
        /// 将消息转化为字节数组
        /// </summary>
        /// <param name="cmdType">消息类型</param>
        /// <param name="cmdContent">消息内容</param>
        /// <returns></returns>
        public byte[] EncoderData(ushort cmdType, string cmdContent)
        {
            //_cmdType_byte = BitConverter.GetBytes(cmdType);
            _cmdContent_byte = Encoding.UTF8.GetBytes(cmdContent);
            //获取全部指令长度
            int len = _cmdContent_byte.Length;//_cmdType_byte.Length + 
            //将长度作为数据头转化为字节数组（int32—四个字节）
            _head_byte = BitConverter.GetBytes(len);
            //创建一个大小为头+内容大小的字节数组
            byte[] buffer = new byte[
                _head_byte.Length+len];
            //将头字节放入buffer
            _head_byte.CopyTo(buffer, 0);
            //将指令类型字节放入buffer
            //_cmdType_byte.CopyTo(buffer, _head_byte.Length);
            //将指令内容字节放入buffer
            _cmdContent_byte.CopyTo(buffer,  _head_byte.Length);//+_cmdType_byte.Length
            return buffer;
        }
        /// <summary>
        /// 将字节数组转化为消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>字符串首位为消息类型</returns>
        public string DecoderData(byte[] buffer)
        {
            string msg = "";
            //msg += BitConverter.ToInt32(buffer, 0);
            //msg += BitConverter.ToUInt16(buffer, 4);
            msg += Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
            return msg;
        }
        int _restLength=0;
        int _pointer = 0;
        //static bool isHeadFinish = false;
        int _restHead = 4;
        bool isBodyFinish = true;
        public void ConstructPacket(byte[] buffer,int length)
        {
            while (true)
            {
                //身体读完了
                if (isBodyFinish)
                {
                    //读头
                    for (int i = 0; i < _restHead; i++)
                    {
                        //头没读完
                        if (_pointer >= length)
                        {
                            //isHeadFinish = false;
                            _restHead = 4 - i;
                            _pointer = 0;
                            return;
                        }
                        _currentPacket.Add(buffer[_pointer++]);
                    }
                    _restHead = 4;
                }
                int len = 0;
                //还有没读完的数据
                if (_restLength != 0)
                {
                    len = _restLength;
                }else//读新数据
                {
                    len = BitConverter.ToInt32(_currentPacket.ToArray(), 0);
                }
                //读身体数据
                for (int i = 0; i < len; i++)
                {
                    if (_pointer >= length)
                    {
                        isBodyFinish = false;
                        _restLength = len - i;
                        _pointer = 0;
                        return;
                    }
                    _currentPacket.Add(buffer[_pointer++]);
                }
                //将当前数据包添加到队列
                _packets.Enqueue(_currentPacket);
                _currentPacket = new List<byte>();//重置包
                isBodyFinish = true;
                _restLength = 0;
                //顺利读到末尾
                if (_pointer == length)
                {
                    _pointer = 0;
                    
                    return;
                }
                #region
                //if (!isBodyFinish)
                //{
                //    for(int i = 0; i < _restLength; i++)
                //    {
                //        //没读完
                //        if (_pointer >= length)
                //        {
                //            isBodyFinish = false;
                //            _restLength = _restLength - i;
                //            _pointer = 0;
                //            return;
                //        }
                //        _currentPacket.Add(buffer[_pointer++]);
                //    }
                //    //将当前数据包添加到队列
                //    _packets.Enqueue(_currentPacket);
                //    _currentPacket = new List<byte>();//重置包
                //    isBodyFinish = true;
                //    _restLength = 0;
                //    //顺利读到末尾
                //    if (_pointer == length)
                //    {
                //        _pointer = 0;
                //        return;
                //    }
                //}
                //else
                //{

                //}
                #endregion
            }

        }
        public byte[] GetSingleData()
        {
            if (_packets.Count <= 0) return null;
            byte[] single= _packets.Dequeue().ToArray();

            return single;
        }
    }
}
