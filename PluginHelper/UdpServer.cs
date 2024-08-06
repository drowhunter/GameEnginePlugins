using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginHelper
{
    public class UdpServer :IDisposable
    {
        public IPAddress _ipAddress = IPAddress.Loopback;

        private int _listenPort;

        public delegate void ClientConnectionDelegate(bool connected);

        public event ClientConnectionDelegate OnClientConnected;



        private Timer _timer;
        private bool _clientConnected;

        public bool ClientConnected
        {
            get => _clientConnected;
            private set
            {
                

                if (_clientConnected != value)
                {
                    _clientConnected = value;

                    if (_listenPort > 0)
                    {
                        if (value)
                        {
                            _timer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
                        }
                        else
                        {
                            _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                        if (OnClientConnected != null)
                            OnClientConnected(value);
                    }
                }
            }
        }

        UdpClient _udpClient;
        

        

        public UdpServer(int listenPort = 0)
        {
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            _listenPort = listenPort;
        }

        public void TimerCallback(object state)
        {
           _timer.Change(Timeout.Infinite, Timeout.Infinite);
           ClientConnected = false;
        }

        public async Task StartListenerAsync(CancellationToken cancellationToken = default)
        {
            if (_listenPort > 0)
            {
                _udpClient = new UdpClient(new IPEndPoint(_ipAddress, _listenPort));
                _clientConnected = true;
                return;
            }

            try
            {
                while (true)
                {
                    if(cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                    
                    Console.WriteLine("Waiting for packet ...");
                    var result = await _udpClient.ReceiveAsync();
                    
                    if(result.Buffer.Length == 0)
                    {
                        Console.WriteLine("Received empty packet");
                        continue;
                    }

                    byte[] bytes = result.Buffer;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Recieved packet: {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                    Console.ResetColor();
                                        
                    ClientConnected = true;                    
                    
                }
            }
            catch (SocketException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();

            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// send dat to client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] data, int port)
        {
            if (_udpClient != null & ClientConnected)
            {
                await _udpClient.SendAsync(data, data.Length, new IPEndPoint(_ipAddress, port));
            }
            
        }

        

        public void Dispose()
        {
            try
            {
                if (_udpClient != null)
                    _udpClient.Close();
            }
            finally
            {
                Console.WriteLine("UdpServer disposed");
            }
        }
    }
}
