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
    public partial class RankingLogView : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        public Visibility OptionVisible { get; set; }

        public RankingLogView()
        {
            InitializeComponent();
            this.OptionVisible = Visibility.Collapsed;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var vm = this.DataContext as ViewModels.ToolViewModel;
                if (vm != null)
                {
                    vm.RefleshRanking();
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt))
            {
                this.OptionVisible = (this.OptionVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("OptionVisible"));
            }
        }
    }
}