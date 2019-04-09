/// <summary>
/// @file Sock.cs
/// @brief TCP Socket Tasks.
/// @author Oku, IMAGENICS Co.,Ltd.
/// @date
/// @details
/// @license MIT
/// </summary>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ConsoleSocketSample1
{
	/// <summary>
	/// @brief Sock
	/// details
	/// </summary>
	public class Sock
	{
		#region fields & prioaties

		private TcpClient connection = new TcpClient();
		private byte[] buffer = new byte[2500];

		public event disconnectionEventHandler disconnected;

		public delegate void disconnectionEventHandler();

		public delegate void serverMessageEventHandler(List<string> runs);

		public delegate void serverTelnetEventHandler(string message);

		protected ToCallback _cb { get; set; }
		private NetworkStream ns { get; set; }
		private Encoding enc = Encoding.ASCII;

		#endregion fields & prioaties

		#region initialization, connection

		/// <summary>
		/// ソケット接続
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="address"></param>
		/// <param name="port"></param>
		public Sock(ToCallback cb, string address, int port)
		{
			_cb = cb;
			// try to connect
			this.connection.Connect(address, port);
			//if successful
			if (this.connection.Connected)
			{
				// NetworkStreamを取得する
				ns = connection.GetStream();
				// 受信開始
				connection.Client.BeginReceive(
						this.buffer,
						0,
						this.buffer.Length,
						SocketFlags.None,
						new AsyncCallback(this.handleServerMessage),
						null);
			}
		}

		public bool IsConnected()
		{
			return (this.connection.Connected) ? true : false;
		}

		#endregion initialization, connection

		#region incoming text handler

		/// <summary>
		/// 受信
		/// </summary>
		/// <param name="ar"></param>
		private void handleServerMessage(IAsyncResult ar)
		{
			// 受信エラー時
			try
			{
				var dt = ns.DataAvailable;
			}
			catch
			{
				return;
			}
			try
			{
				var cn = connection.Client.Available;
			}
			catch
			{
				return;
			}

			// get length of data in buffer
			int receivedCount;
			try
			{
				receivedCount = connection.Client.EndReceive(ar);
			}
			catch
			{
				return;
			}
			// 0 bytes received means the server disconnected
			if (receivedCount == 0)
			{
				this.Disconnect();
				return;
			}
			// 受信したデータを蓄積する
			MemoryStream ms = new MemoryStream();
			int cnt = 0;
			do
			{
				ms.Write(buffer, 0, receivedCount);
				// MemoryStream がbuffer overlowを抑止
				if (cnt > 8)//32)
				{
					ms.Flush();
					ms.Position = 0;
					cnt = 0;
				}
				cnt++;
				// エラー回避
				if (!connection.Client.Connected)
					break;
				try
				{
					var avaiable = ns.DataAvailable;
				}
				catch
				{
					return;
				}
				if (!ns.CanRead)
					return;
			} while (ns.DataAvailable);
			Debug.WriteLine("受信完了!");
			// 受信したデータを文字列に変換
			string receiveString = enc.GetString(ms.ToArray());
			ms.Close();
			_cb(receiveString);
			// now that we're done with this message, listen for the next message
			connection.Client.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.handleServerMessage), null);
		}

		#endregion incoming text handler

		#region outgoing text

		/// <summary>
		/// 電文送信
		/// </summary>
		/// <param name="text"></param>
		public void SendText(string text)
		{
			// if not connected, do nothing
			if (!this.connection.Connected) return;
			// add CR
			if (text.LastIndexOf('\r', 0) != 0)
				text += "\r";
			// convert from Unicode to ASCII
			Encoder encoder = System.Text.Encoding.ASCII.GetEncoder();
			char[] charArray = text.ToCharArray();
			int count = encoder.GetByteCount(charArray, 0, charArray.Length, true);
			byte[] outputBuffer = new byte[count];
			encoder.GetBytes(charArray, 0, charArray.Length, outputBuffer, 0, true);
			// send to server
			this.connection.Client.Send(outputBuffer);
		}

		#endregion outgoing text

		#region disconnect

		/// <summary>
		/// ソケット切断
		/// </summary>
		internal void Disconnect()
		{
			// if not connected, do nothing
			if (!this.connection.Connected)
				return;
			this.connection.Close();
			ns.Close();
			this.connection = new TcpClient();
			if (this.disconnected != null)
			{
#if true
				// for Console Apps.
				this.disconnected.Invoke();
#else
				// for UI Apps.
				App.Current.Dispatcher.BeginInvoke(
						new Action(delegate
						{
							this.disconnected.Invoke();
						}));
#endif
			}
		}

		#endregion disconnect
	}
}