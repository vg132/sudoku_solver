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
		private WebSocket _webSocket = null;

		public event EventHandler<StartEventArgs> Start;
		public event EventHandler Step;
		public event EventHandler Solve;

		public async Task ReceiveConnection()
		{
			var httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://127.0.0.1:8080/");
			httpListener.Start();
			while (true)
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
									OnStart(new StartEventArgs(command.Field));
									break;
								case "step":
									OnStep(new EventArgs());
									break;
								case "solve":
									OnSolve(new EventArgs());
									break;
								default:
									Console.WriteLine("Unknown command");
									break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			}
		}

		public async Task SendData(string data)
		{
			await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		private void OnStart(StartEventArgs e)
		{
			if (Start != null)
			{
				Start(this, e);
			}
		}

		private void OnSolve(EventArgs e)
		{
			if (Solve != null)
			{
				Solve(this, e);
			}
		}

		private void OnStep(EventArgs e)
		{
			if (Step != null)
			{
				Step(this, e);
			}
		}

		public async Task UpdateField(IList<int> field, IDictionary<int, IList<int>> candidates)
		{
			var response = new { field = field, candidates = candidates };
			await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response))), WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}

	public class StartEventArgs : EventArgs
	{
		public StartEventArgs(IList<int> field)
		{
			Field = field;
		}

		public IList<int> Field { get; set; }
	}
}