using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Livet;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using MetroTrilithon.Mvvm;

namespace KCVLoggerPlugin.Models
{
    /// <summary>
    /// ログ管理クラス共通処理
    /// </summary>
    public class LoggerBase : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        /// <summary>
        /// デフォルトのログの保存先
        /// </summary>
        protected static readonly string localDirectoryPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),      // LocalApplicationData
            "KanColleViewerPlugin",                                                         // 共通プラグインフォルダ
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString()     // プラグイン名
            );


        /// <summary>
        /// ログファイルの名前 (継承先のクラス名 + ,dat)
        /// </summary>
        private String saveFileName => this.GetType().Name + ".dat";


        /// <summary>
        /// ログファイルのフルパス
        /// </summary>
        private String SaveFilePath => System.IO.Path.Combine(
			(Properties.Settings.Default.SaveLogPath != "" 
				? Properties.Settings.Default.SaveLogPath
				: localDirectoryPath),
			saveFileName);


        /// <summary>
        /// プラグイン本体のインスタンス
        /// </summary>
        private LoggerPlugin plugin;


		/// <summary>
		/// 派生クラスで保持しているLogインスタンスのインターフェース
		/// </summary>
        private ILogBase logInstance;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public LoggerBase(LoggerPlugin plg, ILogBase log)
        {
            this.plugin = plg;
            this.logInstance = log;
        }


        /// <summary>
        /// ログDBの読み込み
        /// </summary>
        public async Task LoadAsync()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    await this.logInstance.LoadAsync(SaveFilePath, null);
                }
                catch (Exception ex)
                {
#if false
					this.plugin.InvokeNotifyRequested(new Grabacr07.KanColleViewer.Composition.NotifyEventArgs(
                        this.GetType().Name + ".LoadFailed", this.GetType().Name + "の読み込み失敗", ex.Message));
#else
					System.Windows.MessageBox.Show(ex.Message, 
						this.GetType().Name + "の読み込み失敗",
						System.Windows.MessageBoxButton.OK, 
						System.Windows.MessageBoxImage.Error);
#endif
				}
            }
        }


        /// <summary>
        /// ログDBの保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            // 初回ロギング・ディレクトリ作成
            if (!Directory.Exists(localDirectoryPath))
            {
                Directory.CreateDirectory(localDirectoryPath);
            }

            try
            {
                await this.logInstance.SaveAsync(SaveFilePath, null);
            }
            catch (Exception ex)
            {
                this.plugin.InvokeNotifyRequested(new Grabacr07.KanColleViewer.Composition.NotifyEventArgs(
                    this.GetType().Name + ".SaveFailed", this.GetType().Name + "の保存失敗", ex.Message));
            }
        }

    }
}
