using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace KCVLoggerPlugin.Models
{
	/*
	/// <summary>
	/// グラフに関する設定を表す静的プロパティを公開します。
	/// </summary>
	public static class ChartSettings
	{
		/// <summary>
		/// グラフの表示するデータの期間を示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<DisplayedPeriod> DisplayedPeriod { get; }
			= new SerializableProperty<DisplayedPeriod>(GetKey(), SettingsProviders.Roaming, Models.DisplayedPeriod.OneDay) { AutoSave = true };

		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return $"{nameof(ChartSettings)}.{propertyName}";
		}
	}*/

	public class LogInterval
	{
		public int Interval { get; set; }
		public string Text { get; set; }
	}
}
