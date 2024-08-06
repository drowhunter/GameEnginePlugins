using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginHelper
{
    internal class UdpServer :IDisposable
    {

        UdpClient listener;
        UdpClient sender;

        public IPAddress _ipAddress = IPAddress.Any;

        private int _listenPort;

        public UdpServer(int listenPort)
        {
            _listenPort = listenPort;
        }

        private async Task StartListener(  CancellationToken cancellationToken = default)
        {
            
            listener = new UdpClient(_listenPort);
            IPEndPoint groupEP = new IPEndPoint(_ipAddress, _listenPort);

            try
            {
                while (true)
                {
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
                    //await Task.Delay(16, cancellationToken);
                    await Start(cancellationToken);
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

        //a method to start sending data to client
        public async Task Start(CancellationToken cancellationToken = default)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var b = Encoding.ASCII.GetBytes("Hello World");
                var i = await listener.SendAsync(b, b.Length);
            }

        }

        public void Dispose()
        {
            if (listener != null)
                listener.Close();
        }
    }
}
