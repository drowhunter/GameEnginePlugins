using PluginHelper;

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GT7Plugin
{
    /// <summary>
    /// This class is used to create a UDP server that listens for incoming packets.
    /// Tracks when a client is connected.
    /// And allows sending data to the client.
    /// </summary>
    internal class UdpListener : IDisposable
    {
        public delegate void ClientConnectionDelegate(bool connected);

        public event ClientConnectionDelegate OnClientConnected;

        private Timer _timer;
        private bool _clientConnected;
        UdpClient _listenSocket;
        private int _listenPort;
        private IPAddress _ipAddress;

        public UdpListener()
        {
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

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
        
        private void TimerCallback(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            ClientConnected = false;
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        /// <param name="listenPort"></param>
        /// <param name="ip">defaults to Loopback address (127.0.0.1)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task StartListenerAsync( int port,  IPAddress ip = null, CancellationToken cancellationToken = default)
        {
            _ipAddress = ip ?? IPAddress.Loopback;
            _listenPort = port;

            if (_listenPort > 0)
            {
                throw new ArgumentException("Invalid listen port number " + port, nameof(port));                
            }

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_listenSocket == null)
                    {
                        _listenSocket = new UdpClient(new IPEndPoint(_ipAddress, _listenPort));                        
                    }

                    Console.WriteLine("Waiting for packet ...");

                    var result = await _listenSocket.ReceiveAsync(cancellationToken);
                    Console.WriteLine($"r {result.RemoteEndPoint.ToString()}");
                    if (result.Buffer.Length == 0)
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
            catch (OperationCanceledException)
            {
                Console.WriteLine("listen socket cancelled");
                ClientConnected = false;
                
            }
            catch (SocketException e)
            {
                Console.WriteLine("listen socket exception" + e);
                ClientConnected = false;
            }
            finally
            {
                if (!ClientConnected)
                {
                    if (_listenSocket != null)
                    {
                        _listenSocket.Close();
                        _listenSocket = null;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Stop Listening");
                    Console.ResetColor();
                }

                
            }
        }

        /// <summary>
        /// send dat to client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] data, int port)
        {
            if (_listenSocket != null & ClientConnected)
            {
                await _listenSocket.SendAsync(data, data.Length, new IPEndPoint(_ipAddress, port));
            }

        }



        public void Dispose()
        {
            try
            {
                if (_listenSocket != null)
                {
                    _listenSocket.Close();
                    _listenSocket = null;
                }
            }
            finally
            {
                Console.WriteLine("UdpListener disposed");
            }
        }
    }
}
