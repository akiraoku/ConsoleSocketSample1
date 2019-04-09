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
	/// Constants
	/// </summary>
	public partial class Constants
	{
		public const string CN_AST_MSGS = "Enter C(=Continue) or F(=Freeze) or I(=Get Information), press CTRL+Z to exit and Return:";

		public const string RS_TGT_IPAD = "192.168.2.221";
		public const int RS_TGT_PRTN = 1300;

		public const string RS_CMD_GTST = "#$01rA00000";
		public const string RS_CMD_GTOD = "#$01oD00000";
		public const string RS_CMD_GTOC = "#$01oC00000";
		public const string RS_CMD_GTOE = "#$01oE00000";
		public const string RS_CMD_GTOG = "#$01oG00000";
		public const string RS_CMD_FZON = "#$01Ob+0001";
		public const string RS_CMD_FZOF = "#$01Ob00000";
	}

	/// <summary>
	/// @brief Program
	/// details
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// @fn static void Main(string[] args)
		/// @brief Main
		/// @param name="args"
		/// @return void
		/// @details
		/// </summary>
		private static void Main(string[] args)
		{
			MainTask mainTask = new MainTask();
			Program program = new Program();
			// 情報取得
			mainTask.checkSocket();
#pragma warning disable
			program.waitAsync("Initialising...");
#pragma warning restore
			// メインループ
			string line;
			Console.WriteLine(Constants.CN_AST_MSGS);
			Console.WriteLine();
			do
			{
				line = Console.ReadLine();
				if (line != null)
					Console.WriteLine(">" + line);
				switch (line)
				{
					case "C":
					case "c":
						Console.Write("Un-Freeze>");
						mainTask.controlSocket(Constants.RS_CMD_FZOF);
						break;

					case "F":
					case "f":
						Console.Write("Freeze>");
						mainTask.controlSocket(Constants.RS_CMD_FZON);
						break;

					case "I":
					case "i":
						Console.Write("Information>");
						mainTask.controlSocket(Constants.RS_CMD_GTST);
						break;

					default:
						Console.WriteLine("Enter C or F or I! or Wait a second.");
						break;
				}
			} while (line != null);
		}

#pragma warning disable

		/// <summary>
		/// 時間待ち
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private async Task<string> waitAsync(string message)
		{
			System.Threading.Thread.Sleep(5000);
			return "message:" + message;
		}

#pragma warning restore
	}

	public class MainTask
	{
		/// <summary>
		/// コンストラクタ
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
		/// <returns></returns>
		public static MainTask GetInstance()
		{
			return _instance;
		}

		// ソケット変数
		private Sock sock { get; set; }

		/// <summary>
		/// Create Socket
		/// </summary>
		public void CreateSocket()
		{
			try
			{
				if (sock != null)
					sock.Disconnect();
				sock = new Sock(
				Cb[1],
				Constants.RS_TGT_IPAD,
				Constants.RS_TGT_PRTN);
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
		/// ベースのコールバック
		/// </summary>
		/// <param name="s"></param>
		private void bseCallbacks(string s)
		{
			Debug.WriteLine(s);
		}

		/// <summary>
		/// ソケットのコールバック
		/// </summary>
		/// <param name="s"></param>
		private void netCallbacks(string s)
		{
			Debug.WriteLine(s);
			Console.WriteLine(s);
		}

		/// <summary>
		/// ソケット制御
		/// </summary>
		/// <param name="s"></param>
		public void controlSocket(string s)
		{
			// TCPソケット生成
			var cph = s + "\r";
			CreateSocket();
			if (sock != null)
				sock.SendText(cph);
			// ソケットのタイムアウトTASK
			Task.Run(async () =>
			{
				await Task.Delay(1500);
				sock.Disconnect();
				Console.WriteLine(Constants.CN_AST_MSGS);
			});
		}

		/// <summary>
		/// ソケットのチェック
		/// </summary>
		/// <returns></returns>
		public bool checkSocket()
		{
			CreateSocket();
			Task.Run(async () =>
			{
				await Task.Delay(5000);
				sock.Disconnect();
			});
			return sock.IsConnected();
		}
	}
}