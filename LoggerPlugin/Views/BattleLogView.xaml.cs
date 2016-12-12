using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KCVLoggerPlugin.Views
{
    /* 
	 * ViewModelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedWeakEventListenerや
     * CollectionChangedWeakEventListenerを使うと便利です。独自イベントの場合はLivetWeakEventListenerが使用できます。
     * クローズ時などに、LivetCompositeDisposableに格納した各種イベントリスナをDisposeする事でイベントハンドラの開放が容易に行えます。
     *
     * WeakEventListenerなので明示的に開放せずともメモリリークは起こしませんが、できる限り明示的に開放するようにしましょう。
     */

    /// <summary>
    /// ToolView.xaml の相互作用ロジック
    /// </summary>
    public partial class BattleLogView : UserControl
    {
        public BattleLogView()
        {
            InitializeComponent();
        }
    }

	
	/// <summary>
	/// 陣形コンバータ。数値を文字列に変換します。
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	public class FormationConverter : IValueConverter
	{

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int val = System.Convert.ToInt32(value);
			switch (val)
			{
				case -1:	return "";
				case 1:		return "単縦陣";
				case 2:		return "複縦陣";
				case 3:		return "輪形陣";
				case 4:		return "梯形陣";
				case 5:		return "単横陣";
				case 11:	return "連合対潜警戒";
				case 12:	return "連合前方警戒";
				case 13:	return "連合輪形陣";
				case 14:	return "連合戦闘隊形";
				default:	return val.ToString();
			}
		}


		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 編集できないので逆はサポートしない
			throw new NotImplementedException();
		}
	}


	/// <summary>
	/// 戦闘形態コンバータ。数値を文字列に変換します。
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	public class MatchTypeConverter : IValueConverter
	{

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int val = System.Convert.ToInt32(value);
			switch (val)
			{
				case -1: return "";
				case 1: return "同航戦";
				case 2: return "反航戦";
				case 3: return "Ｔ字有利";
				case 4: return "Ｔ字不利";
				default: return val.ToString();
			}
		}


		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 編集できないので逆はサポートしない
			throw new NotImplementedException();
		}
	}


	/// <summary>
	/// 制空権コンバータ。数値を文字列に変換します。
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	public class AirSuperityTypeConverter : IValueConverter
	{

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int val = System.Convert.ToInt32(value);
			switch (val)
			{
				case -1: return "";
				case 0: return "航空互角";
				case 1: return "制空権確保";
				case 2: return "航空優勢";
				case 3: return "航空劣勢";
				case 4: return "制空権喪失";
				default: return val.ToString();
			}
		}


		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// 編集できないので逆はサポートしない
			throw new NotImplementedException();
		}
	}
}