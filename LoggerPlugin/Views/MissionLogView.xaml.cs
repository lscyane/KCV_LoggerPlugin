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
    public partial class MissionLogView : UserControl
    {
        public MissionLogView()
        {
            InitializeComponent();
        }
    }


	/// <summary>
	/// 結果用コンバータ。数値を文字列に変換します。
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	public class MissionResultConverter : IValueConverter
	{

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int val = System.Convert.ToInt32(value);
			switch (val)
			{
				case 0: return "×";
				case 1: return "◯";
				case 2: return "◎";
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