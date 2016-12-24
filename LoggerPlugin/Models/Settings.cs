using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace KCVLoggerPlugin.Models
{
	/// <summary>
	/// 資源推移ログの設定
	/// </summary>
	public class LogInterval
	{
		public int Interval { get; set; }	// 設定値[分]
		public string Text { get; set; }	// 表示テキスト
	}
}
