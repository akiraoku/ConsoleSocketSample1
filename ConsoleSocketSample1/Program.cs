/// <summary>
/// @file Program.cs
/// @brief Main
/// @author Oku, IMAGENICS Co.,Ltd.
/// @date
/// @details
/// @license MIT
/// </summary>
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleSocketSample1
{
	/// <summary>
	/// @brief Constants
	/// details
	/// </summary>
	public partial class Constants
	{
		public const string CN_AST_MSGS = "Enter '1 to 8' or , press 'CTRL+Z' to exit and Return:\n>";

		public const string RS_TGT_IPAD = "192.168.2.254";
		public const int RS_TGT_PRTN = 1300;
		public const int CHECK_TIMEOUT = 1500;
		public const int SOCK_TIMEOUT = 5000;

		public const string RS_CMD_GTST = "#$01rA00000";
		public const string RS_CMD_GTOD = "#$01oD00000";
		public const string RS_CMD_GTOC = "#$01oC00000";
		public const string RS_CMD_GTOE = "#$01oE00000";
		public const string RS_CMD_GTOG = "#$01oG00000";
		public const string RS_CMD_FZON = "#$01Ob+0001";
		public const string RS_CMD_FZOF = "#$01Ob00000";

		public const string SW_CMD_GTST = "#rA00000";

		public const string SW_CMD_SELECT_1_1 = "1,1";
		public const string SW_CMD_SELECT_2_1 = "2,1";
		public const string SW_CMD_SELECT_3_1 = "3,1";
		public const string SW_CMD_SELECT_4_1 = "4,1";
		public const string SW_CMD_SELECT_0_1 = "q,1";

		public const string SW_CMD_SELECT_1_2 = "1,2";
		public const string SW_CMD_SELECT_2_2 = "2,2";
		public const string SW_CMD_SELECT_3_2 = "3,2";
		public const string SW_CMD_SELECT_4_2 = "4,2";
		public const string SW_CMD_SELECT_0_2 = "q,2";

		public const string SW_CMD_W = "w";

	}

	/// <summary>
	/// @brief Program
	/// details
	/// </summary>
	internal class Program
	{

		/// <summary>
		/// Main
		/// </summary>
		private static void Main(string[] args)
		{
		
			Console.Write("Initialising... ");
			var adrs = string.Empty;
			if (args.Length > 0)
			{
				Console.WriteLine("Connect to " + args[0].ToString());
				try
				{
					adrs = args[0].ToString();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Bad Parameters in command args. " + ex.ToString());
				}
			}
			else
			{
				adrs = Constants.RS_TGT_IPAD;
			}

			MainTask mainTask = new MainTask();
			mainTask.DstAdrs = adrs;
			mainTask.DstPrt = Constants.RS_TGT_PRTN;
			Program program = new Program();
			
			// check socket
			mainTask.checkSocket();
#pragma warning disable
			program.waitAsync("...");
#pragma warning restore
			
			// main loop
			string line;
			Console.Write(Constants.CN_AST_MSGS);
			do
			{
				line = Console.ReadLine();
				var curretConsoleColor = Console.ForegroundColor;
				if (line != null)
					Console.ForegroundColor = ConsoleColor.Green;
				switch (line)
				{
					case "C":
					case "c":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.RS_CMD_FZOF);
						break;

					case "F":
					case "f":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.RS_CMD_FZON);
						break;

					case "I":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_GTST);
						break;
					case "i":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.RS_CMD_GTST);
						break;

					case "1":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_1_1);
						break;

					case "2":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_2_1);
						break;
					
					case "3":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_3_1);
						break;
					
					case "4":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_4_1);
						break;

					case "o":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_0_1);
						break;

					case "5":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_1_2);
						break;
					
					case "6":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_2_2);
						break;
					
					case "7":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_3_2);
						break;
					
					case "8":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_4_2);
						break;

					case "O":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_SELECT_0_2);
						break;
					
					case "w":
						Console.Write("==> " + line + " ");
						mainTask.controlSocket(Constants.SW_CMD_W);
						break;

					default:
						Console.Write(Constants.CN_AST_MSGS);
						break;
				}
				Console.ForegroundColor = curretConsoleColor;
			} while (line != null);
		}

#pragma warning disable

		/// <summary>
		/// 時間待ち
		/// </summary>
		private async Task<string> waitAsync(string message)
		{
			System.Threading.Thread.Sleep(5000);
			return "message:" + message;
		}

#pragma warning restore
	}

	/// <summary>
	/// @brief MainTask
	/// details
	/// </summary>
	public class MainTask
	{
		public string DstAdrs { get; set; }
		public int DstPrt { get; set; }

		/// <summary>
		/// constructor
		/// </summary>
		public MainTask()
		{
			_instance = this;
		}

		//
		private static MainTask _instance { get; set; }

		/// <summary>
		/// Own Instance
		/// </summary>
		public static MainTask GetInstance()
		{
			return _instance;
		}

		// ソケット変数
		private Sock sock { get; set; }

		/// <summary>
		/// Create Socket
		/// </summary>
		public void CreateSocket(string a, int p)
		{
			try
			{
				if (sock != null)
					sock.Disconnect();
				sock = new Sock(
				Cb[1],
				DstAdrs,
				DstPrt);
			}
			catch (Exception e)
			{
				Debug.Write(e.ToString());
				sock = null;
			}
		}

		/// <summary>
		/// delegate
		/// </summary>
		public static ToCallback[] Cb = {
			new ToCallback( (s) => {
			GetInstance().bseCallbacks(s);
			}),
			new ToCallback( (s) => {
			GetInstance().netCallbacks(s);
			}),
		};

		/// <summary>
		/// base callback
		/// </summary>
		/// <param name="s"></param>
		private void bseCallbacks(string s)
		{
			Debug.WriteLine(s);
		}

		/// <summary>
		/// callback from socket
		/// </summary>
		private void netCallbacks(string s)
		{
			Debug.WriteLine(s);
			Console.WriteLine(s);
		}

		/// <summary>
		/// control socket
		/// </summary>
		public void controlSocket(string s)
		{
			// TCPソケット生成
			var cph = s + "\r";
			CreateSocket(DstAdrs, DstPrt);
			if (sock != null)
				sock.SendText(cph);
			// ソケットのタイムアウトTASK
			Task.Run(async () =>
			{
				await Task.Delay(Constants.SOCK_TIMEOUT);
				sock.Disconnect();
				Console.Write(Constants.CN_AST_MSGS);
			});
		}

		/// <summary>
		/// checke socket
		/// </summary>
		public bool checkSocket()
		{
			CreateSocket(DstAdrs, DstPrt);
			Task.Run(async () =>
			{
				await Task.Delay(Constants.CHECK_TIMEOUT);
				sock.Disconnect();
			});
			return sock.IsConnected();
		}
	}
}
