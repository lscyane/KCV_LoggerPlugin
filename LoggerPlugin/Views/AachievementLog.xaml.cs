using System;
using System.Collections.Generic;
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
    /// AachievementLog.xaml の相互作用ロジック
    /// </summary>
    public partial class AachievementLogView : UserControl
    {
        public AachievementLogView()
        {
            InitializeComponent();
        }
    }



	/// <summary>
	/// 経験値増分表示用コンバータ。数値を文字列に変換します。
	/// </summary>
	public class IncrementalExpConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int exp = (int)values[0];
			int inc = (int)values[1];
			return String.Format("{0} ({1})", exp, inc.ToString("+#;-#;#"));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			// 編集できないので逆はサポートしない
			throw new NotImplementedException();
		}
	}
}