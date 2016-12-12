using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using KCVLoggerPlugin.Models;

namespace KCVLoggerPlugin.ViewModels
{
    public class SettingViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */


        /// <summary>
        /// プラグイン本体のインスタンス
        /// </summary>
        private LoggerPlugin plugin;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plugin"></param>
        public SettingViewModel(LoggerPlugin plugin)
        {
            this.plugin = plugin;
        }


		/// <summary>
		/// 初期化
		/// プラグイン本体から呼ばれる
		/// </summary>
		public void Initialize()
        {

		}


		/// <summary>
		/// ButtonのClickイベントから開始のパターン。
		/// OpenFileSelectionMessageを受け付けるOpenCommand
		/// </summary>
		ListenerCommand<OpeningFileSelectionMessage> m_openCommand;
        public ListenerCommand<OpeningFileSelectionMessage> OpenCommand
        {
            get
            {
                if (this.m_openCommand == null)
                {
                    this.m_openCommand = new ListenerCommand<OpeningFileSelectionMessage>(OpenCreateItemLog, () => true);
                }
                return this.m_openCommand;
            }
        }
        public void OpenCreateItemLog(OpeningFileSelectionMessage m)
        {
            this.Open(m, CreateItemLog.Instance);
        }
        public void OpenCreateShipLog(OpeningFileSelectionMessage m)
        {
            this.Open(m, CreateShipLog.Instance);
        }
        public void OpenBattleLog(OpeningFileSelectionMessage m)
        {
            this.Open(m, BattleLog.Instance);
        }
        public void OpenMissionLog(OpeningFileSelectionMessage m)
        {
            this.Open(m, MissionLog.Instance);
        }
        public void OpenMaterialLog(OpeningFileSelectionMessage m)
        {
            this.Open(m, MaterialLog.Instance);
        }
		public void OpenAachievementLog(OpeningFileSelectionMessage m)
		{
			this.Open(m, AachievementLog.Instance);
		}
		private async void Open(OpeningFileSelectionMessage m, ILogBase logInstance)
        {
            if (m.Response == null)
            {
                // キャンセル時は無言のリターン
                //Messenger.Raise(new InformationMessage("Import error", "Import", System.Windows.MessageBoxImage.Error, "ExchangeResult"));
                return;
            }

            // インポート処理
            String path = m.Response[0];
            try
            {
                // 処理は拡張子で振り分け
                if (System.IO.Path.GetExtension(path) == ".dat")
                {
                    await logInstance.LoadAsync(path, () => {
                        // 成功を通知
                        this.RaiseInfoMessage("Import success!", "Import", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                    });
                }
                else if (System.IO.Path.GetExtension(path) == ".csv")
                {
                    logInstance.ImportCsv(path);
                    // 成功を通知
                    this.RaiseInfoMessage("Import success!", "Import", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                }
                else
                {
                    throw new Exception("ファイルがありません。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.RaiseInfoMessage(ex.Message, "Import", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                return;
            }
        }



        /// <summary>
        /// ButtonのClickイベントから開始のパターン。
        /// SavingFileSelectionMessageを受け付けるSaveCommand
        /// </summary>
        ListenerCommand<SavingFileSelectionMessage> m_saveCommand;
        public ListenerCommand<SavingFileSelectionMessage> SaveCommand
        {
            get
            {
                if (this.m_saveCommand == null)
                {
                    this.m_saveCommand = new ListenerCommand<SavingFileSelectionMessage>(SaveCreateItemLog, () => true);
                }
                return this.m_saveCommand;
            }
        }
        public void SaveCreateItemLog(SavingFileSelectionMessage m)
        {
            this.Save(m, CreateItemLog.Instance);
        }
        public void SaveCreateShipLog(SavingFileSelectionMessage m)
        {
            this.Save(m, CreateShipLog.Instance);
        }
        public void SaveBattleLog(SavingFileSelectionMessage m)
        {
            this.Save(m, BattleLog.Instance);
        }
        public void SaveMissionLog(SavingFileSelectionMessage m)
        {
            this.Save(m, MissionLog.Instance);
        }
        public void SaveMaterialLog(SavingFileSelectionMessage m)
        {
            this.Save(m, MaterialLog.Instance);
        }
		public void SaveAachievementLog(SavingFileSelectionMessage m)
		{
			this.Save(m, AachievementLog.Instance);
		}
		private async void Save(SavingFileSelectionMessage m, ILogBase logInstance)
        {
            if (m.Response == null)
            {
                // キャンセル時は無言のリターン
                //Messenger.Raise(new InformationMessage("Export error", "Export", System.Windows.MessageBoxImage.Error, "ExchangeResult"));
                return;
            }

            //  エクスポート処理
            String path = m.Response[0];
            try
            {
                // 処理は拡張子で振り分け
                if (System.IO.Path.GetExtension(path) == ".dat")
                {
                    await logInstance.SaveAsync(path, () => {
                        // 成功を通知
                        this.RaiseInfoMessage("Export success!", "Export", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                    });
                }
                else if (System.IO.Path.GetExtension(path) == ".csv")
                {
                    logInstance.ExportCsv(path);
                    // 成功を通知
                    this.RaiseInfoMessage("Export success!", "Export", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                }
                else
                {
                    throw new Exception("ファイルがありません。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.RaiseInfoMessage(ex.Message, "Export", System.Windows.MessageBoxImage.Information, "ExchangeResult");
                return;
            }
        }


        /// <summary>
        /// ダイアログの表示（UIスレッドへのDispatch付き）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        private void RaiseInfoMessage(String text, String caption, System.Windows.MessageBoxImage image, String messageKey)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            Messenger.Raise(new InformationMessage(text, caption, image, messageKey)))
            , System.Windows.Threading.DispatcherPriority.Background);
        }

    }
}
