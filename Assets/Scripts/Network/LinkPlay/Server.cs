using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

namespace Dremu.Network.LinkPlay
{
    public class Server
    {

        Socket linkSocket;
        string? errorInfo = null;
        int port = -1;

        public Server(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// 该函数用于创建并侦听
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>是否成功创建</returns>
        /// 一键点击，即刻联机游玩！
        public bool StartServer(int port) 
        {
            try
            {
                IPAddress localIp = IPAddress.Any;
                IPEndPoint localEndPoint = new IPEndPoint(localIp, port);
                Socket linkSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                linkSocket.Bind(localEndPoint);
                linkSocket.Listen(port);
                return true;
            }
            catch (Exception ex) 
            {
                errorInfo = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">无需调用该方法</exception>
        public string TryGetErrorInfo(bool confirm) 
        {
            if (!confirm)
            {
                return "";
            }
            else
            {
                try
                {
                    return errorInfo;
                }
                catch 
                {
                    throw new FormatException("网络创建成功无需调用该方法");
                }
            }
        }

    } 
}
