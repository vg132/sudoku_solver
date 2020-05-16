using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sudoku_solver
{
	public class Server
	{
		private Mutex signal = new Mutex();
		private WebSocket _webSocket = null;

		public async Task ReceiveConnection()
		{
			while (true)
			{
				var httpListener = new HttpListener();
				httpListener.Prefixes.Add("http://127.0.0.1:8080/");
				httpListener.Start();
				while (signal.WaitOne())
				{
					try
					{
						var context = await httpListener.GetContextAsync();
						if (context.Request.IsWebSocketRequest)
						{
							var webSocketContext = await context.AcceptWebSocketAsync(null);
							_webSocket = webSocketContext.WebSocket;
							while (_webSocket.State == WebSocketState.Open)
							{
								var data = new byte[512];
								await _webSocket.ReceiveAsync(data, new CancellationToken());
								var str = Encoding.UTF8.GetString(data);

								var command = JsonConvert.DeserializeObject<NetCommand>(str);
								switch (command?.Command)
								{
									case "start":
										Console.WriteLine("Start!");
										break;
									default:
										Console.WriteLine("Unknown command");
										break;
								}
								//await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(str)), WebSocketMessageType.Text, true, CancellationToken.None);
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						Console.WriteLine(ex.StackTrace);
					}

					httpListener.Close();
					signal.ReleaseMutex();
				}
			}
		}

		public async Task SendData(string data)
		{
			await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, CancellationToken.None);
		}
		//public async Task OnRecieveData
	}

	public class NetCommand
	{
		public string Command { get; set; }
		public IList<int> Board { get; set; }
	}
}