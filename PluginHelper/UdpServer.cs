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
        public bool ClientConnected { get; private set; }

        UdpClient listener;
        UdpClient sender;

        public IPAddress _ipAddress = IPAddress.Any;

        private int _listenPort;

        public UdpServer(int listenPort)
        {
            _listenPort = listenPort;
        }

        public async Task StartListener(  CancellationToken cancellationToken = default)
        {
            
            listener = new UdpClient(_listenPort);
            IPEndPoint groupEP = new IPEndPoint(_ipAddress, _listenPort);

            try
            {
                while (true)
                {
                    if(cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                    Console.WriteLine("Waiting for broadcast");
                    var result = await listener.ReceiveAsync();
                    
                    if(result.Buffer.Length == 0)
                    {
                        Console.WriteLine("Received empty message");
                        continue;
                    }
                    byte[] bytes = result.Buffer;

                    
                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

                    this.ClientConnected = true;
                    //await Task.Delay(16, cancellationToken);
                    //await Start(cancellationToken);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        /// <summary>
        /// send dat to client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] data)
        {
            if (listener == null || !ClientConnected)
            {
                await listener.SendAsync(data, data.Length, new IPEndPoint(_ipAddress, _listenPort));
            }
            else
            {
                throw new InvalidOperationException("Listener is not initialized or client not connected");
            }
        }

        //a method to start sending data to client
        public async Task Start(CancellationToken cancellationToken = default)
        {
            DateTime _lastTime = DateTime.Now;
            int MIN_DELAY = 1000;
            while (!cancellationToken.IsCancellationRequested)
            {
                var duration = DateTime.Now - _lastTime;
                if (duration.TotalMilliseconds < MIN_DELAY)
                {
                    await Task.Delay(MIN_DELAY - (int)duration.TotalMilliseconds);
                }
                var b = Encoding.ASCII.GetBytes("Hello World");
                var i = await listener.SendAsync(b, b.Length);
                _lastTime = DateTime.Now;
            }

        }

        public void Dispose()
        {
            if (listener != null)
                listener.Close();
        }
    }
}
