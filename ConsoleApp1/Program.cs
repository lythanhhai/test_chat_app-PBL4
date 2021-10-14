﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        private const int BUFFER_SIZE = 1024;
        private const int PORT_NUMBER = 9999;

        static ASCIIEncoding encoding = new ASCIIEncoding();

        // chứa danh sách remote socket
        static List<EndPoint> remote = new List<EndPoint>();

        // chứa danh sách socket kết nối đến
        static List<Socket> Socket_client = new List<Socket>();

        // chứa danh sách userId kết nối
        static List<string> userId = new List<string>();
        public static void Main()
        {
            try
            {
                IPAddress address = IPAddress.Parse("127.0.0.1");

                TcpListener listener = new TcpListener(address, PORT_NUMBER);
                // 1. listen
                listener.Start();

                Console.WriteLine("Server started on " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");
                Program p = new Program();
                while (true)
                {

                    Socket socket = listener.AcceptSocket();

                    Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

                    remote.Add(socket.RemoteEndPoint);
                    Socket_client.Add(socket);

                    // nhận userId
                    byte[] userId_load = new byte[BUFFER_SIZE];
                    int size_userIdLoad = socket.Receive(userId_load);

                    userId.Add(encoding.GetString(userId_load));

                      

                    Thread userThread = new Thread(new ThreadStart(() => p.User(socket)));
                    userThread.Start();

                    //Thread close = new Thread(new ThreadStart(() => p.IsSocketConnected(socket)));
                    //close.Start();
                  
                    // 4. close
                    //socket.Close();

                    // kiểm tra nếu hết người kết nối thì server tự động ngắt
                    //if (Socket_client.Count < 1)
                    //{
                    //    listener.Stop();
                    //}    

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
            Console.Read();
        }

        public void IsSocketConnected(Socket client)
        {
            if(!((client.Poll(1000, SelectMode.SelectRead) && (client.Available == 0)) || !client.Connected))
            {
                for (int i = 0; i < Socket_client.Count; i++)
                {
                    if (String.Compare(client.RemoteEndPoint.ToString(), remote[i].ToString()) == 0)
                    {
                        remote.RemoveAt(i);
                        Socket_client.RemoveAt(i);
                        userId.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        // xử lý gửi
        public void User(Socket client)
        {
            //if (IsSocketConnected(client))
            //{
            //    for (int i = 0; i < Socket_client.Count; i++)
            //    {
            //        if (String.Compare(client.RemoteEndPoint.ToString(), remote[i].ToString()) == 0)
            //        {
            //            remote.RemoveAt(i);
            //            Socket_client.RemoveAt(i);
            //            userId.RemoveAt(i);
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            try
            {
                while (true)
                {
                    byte[] userId_receive = new byte[BUFFER_SIZE];
                    int size_userId = client.Receive(userId_receive);
                    //Console.WriteLine(encoding.GetString(userId_receive));
                    byte[] data = new byte[BUFFER_SIZE];
                    int size = client.Receive(data);
                    string packetMes = encoding.GetString(userId_receive).Split(' ')[0] + " " + encoding.GetString(data);

                    //Console.WriteLine(packetMes);

                    for (int i = 0; i < userId.Count; i++)
                    {
                        if (String.Compare(encoding.GetString(userId_receive).Split(' ')[1], userId[i]) == 0)
                        {
                            // gửi cho người nhận id người gửi để check xem có đang nhắn tin cùng nhau không
                            //Socket_client[i].Send(data, 0, size, SocketFlags.None);
                            Socket_client[i].Send(encoding.GetBytes(packetMes), 0, size + size + size_userId, SocketFlags.None);

                        }
                    }
                    //client.Send(data, 0, size, SocketFlags.None);
                    //Socket_client[1].Send(data, 0, size, SocketFlags.None);
                    //Socket_client[1].Send(data, 0, size, SocketFlags.None);

                }
            }
            catch(Exception err)
            {
                //Console.WriteLine(err.ToString());
                //for (int i = 0; i < Socket_client.Count; i++)
                //{
                //    if (String.Compare(client.RemoteEndPoint.ToString(), remote[i].ToString()) == 0)
                //    {
                //        remote.RemoveAt(i);
                //        Socket_client.RemoveAt(i);
                //        userId.RemoveAt(i);
                //        break;
                //    }
                //}
            }

                         

          //  }                
            
        }

    }
}