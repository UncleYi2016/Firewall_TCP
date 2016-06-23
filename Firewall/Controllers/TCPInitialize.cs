using Firewall.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Firewall.Controllers {
    class TCPInitialize {
        IPAddress hostIp = IPAddress.Any;
        int port;
        public LogTable lt = new LogTable();
        public DenyTable dt = new DenyTable();
        ArrayList threads = new ArrayList();

        public TCPInitialize() {
            port = 80;
        }

        public TCPInitialize(int port) {
            this.port = port;
        }

        public void listenThreadInit() {
            Thread listen = new Thread(listenThread);
            threads.Add(listen);
            listen.Start();
        }

        private void listenThread() {
            Socket hostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            hostSocket.Bind(new IPEndPoint(hostIp, port));
            hostSocket.Listen(10);
            while (true) {
                Socket clientSocket = hostSocket.Accept();
                try {
                    ArrayList denies = dt.read();
                    IPEndPoint clientEnd = clientSocket.RemoteEndPoint as IPEndPoint;
                    lt.write("********************************");
                    lt.write("Time: " + DateTime.Now.ToString());
                    lt.write("Connect IP: " + clientEnd.Address.ToString());
                    lt.write("Connect port: " + clientEnd.Port.ToString());
                    if (denies.Contains(clientEnd.Address.ToString())) {
                        string rejectStr = "Your IP has been rejected./r/n";
                        clientSocket.Send(Encoding.ASCII.GetBytes(rejectStr), rejectStr.Length, SocketFlags.None);
                        clientSocket.Close();
                        lt.write("Operation: Reject");
                        lt.write("********************************");
                        continue;
                    }
                    lt.write("Operation: Accept");
                    lt.write("********************************");
                    IPAddress ip = IPAddress.Parse(/*你的IP地址*/);
                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try {
                        serverSocket.Connect(new IPEndPoint(ip, /*你的端口*/)); //配置服务器IP与端口  
                    } catch {
                        Console.WriteLine("连接服务器失败，请按回车键退出！");
                        Thread.CurrentThread.Abort();
                    }
                    Socket[] socketPair = { clientSocket, serverSocket };
                    Thread receiveThread = new Thread(receiveMessage);
                    Thread sendThread = new Thread(sendMessage);
                    receiveThread.Start(socketPair);
                    sendThread.Start(socketPair);

                } catch (Exception ex) {
                    if (clientSocket != null && clientSocket.Connected) {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                    }
                }
            }

        }

        private void sendMessage(object socketPair) {
            Socket[] socekts = (Socket[])socketPair;
            Socket clientSocket = socekts[0];
            Socket serverSocket = socekts[1];
            IPEndPoint point = clientSocket.RemoteEndPoint as IPEndPoint;
            int bufferSize = 131072;
            //if (point.Address.ToString().Equals("127.0.0.1")) {
            //    bufferSize = 4096;
            //}
            byte[] clientRecv = new byte[bufferSize];
            byte[] serverRecv = new byte[bufferSize];

            //设定服务器IP地址  
            
            try {
                int receiveNumber = 0;
                int sendNumber = 0;
                do {
                    receiveNumber = clientSocket.Receive(clientRecv);
                    sendNumber = serverSocket.Send(clientRecv, receiveNumber, SocketFlags.None);
                } while (receiveNumber != 0);
            } catch (Exception ex) {
                Console.Write(ex.Message);
            } finally {
                if(clientSocket != null && clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            Thread.CurrentThread.Abort();

        }
    //通过clientSocket接收数据  

        private void receiveMessage(object socketPair) {
            Socket[] socekts = (Socket[])socketPair;
            Socket clientSocket = socekts[0];
            Socket serverSocket = socekts[1];
            IPEndPoint point = clientSocket.RemoteEndPoint as IPEndPoint;
            int bufferSize = 131072;
            //if (point.Address.ToString().Equals("127.0.0.1")) {
            //    bufferSize = 4096;
            //}
            byte[] clientRecv = new byte[bufferSize];
            byte[] serverRecv = new byte[bufferSize];
            
            //设定服务器IP地址  
            try {
                int receiveNumber = 0;
                int sendNumber = 0;
                do {
                    receiveNumber = serverSocket.Receive(serverRecv);
                    sendNumber = clientSocket.Send(serverRecv, receiveNumber, SocketFlags.None);
                } while (receiveNumber != 0);
            } catch (Exception ex) {
                Console.Write(ex.Message);
            } finally {
                if(clientSocket != null && clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            Thread.CurrentThread.Abort();
        }
        private bool testConnecting(Socket testSocket) {
            if (testSocket == null || !testSocket.Connected) {
                return false;
            }
            try {
                byte[] testBuffer = new byte[10];
                testSocket.Send(testBuffer, 0, SocketFlags.None);
                return true;
            } catch {
                return false;
            }
        }
    }

}
