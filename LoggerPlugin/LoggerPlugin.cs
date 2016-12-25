using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;
using MetroTrilithon.Lifetime;

namespace KCVLoggerPlugin
{
    /// <summary>
    /// Main Control
    /// </summary>
    [Export(typeof(IPlugin))]       // ExportMetadataは設定→プラグインタブに表示される情報
    [ExportMetadata("Guid", "7FAADD01-7818-42F4-BC3B-005BEF6FAC3C")]
    [ExportMetadata("Title", "KCVLogger")]
    [ExportMetadata("Description", "各種記録を保存・表示します。")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "@lscyane")]
    [Export(typeof(ITool))]
    [Export(typeof(ISettings))]
//  [Export(typeof(INotifier))]
    [Export(typeof(IRequestNotify))]
    public class LoggerPlugin :
        IPlugin,            // 必須。 プラグインを表します。
        ITool,              // [ツール] タブに表示される画面を用意したい場合に実装します。
        ISettings,          // プラグインの設定画面を用意したい場合に実装します。
//      INotifier,          // 通知要求を受けて、ユーザーに通知を行いたい場合に実装します。
        IRequestNotify,     // プラグイン側から本体に通知を要求したい場合に実装します。
		MetroTrilithon.Lifetime.IDisposableHolder
	{
        private ViewModels.ToolViewModel tool_VM;
        private ViewModels.SettingViewModel setting_VM;


		/// <remarks>
		/// 静的コンストラクターは、静的データの初期化、または 1 回だけ実行する必要がある特定の処理の実行に使用されます。 
		/// 静的コンストラクターは、最初のインスタンスを作成する前、または静的メンバーが参照される前に自動的に呼び出されます。
		/// </remarks>    
		static LoggerPlugin()
		{
			try
			{
				// このクラスの何らかのメンバーにアクセスされたら読み込み。
				// 読み込みに失敗したら例外が投げられてプラグインだけが死ぬ（はず）
				System.Reflection.Assembly.LoadFrom("protobuf-net.dll");
				System.Reflection.Assembly.LoadFrom("Xceed.Wpf.Toolkit.dll");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw;
			}
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoggerPlugin()
		{
			this.tool_VM = new ViewModels.ToolViewModel(this);
			this.setting_VM = new ViewModels.SettingViewModel(this);
		}

	
		#region 各インターフェースの実装
		/// <summary>
		/// IPluginのインターフェイスを実装
		/// </summary>
		public void Initialize()
        {
			// Pluginの構成上ContentRenderedが発生しないせいかInitializeの呼び出しはされないのでここから呼ぶ
			this.tool_VM.Initialize();
            this.setting_VM.Initialize();
			
			// アプリケーションの終了時に保存を行う
			Disposable.Create(() =>
			{
				Properties.Settings.Default.Save();	// 設定
				this.tool_VM.Dispose();				// 各種ログ
			}).AddTo(this);
		}


        /// <summary>
        /// IToolのインターフェースを実装(画面を出すために必要)
        /// </summary>
        public String Name => "KCVLogger";
        object ITool.View => new Views.ToolView() { DataContext = this.tool_VM };


		/// <summary>
		/// ISettingsのインターフェースを実装(画面を出すために必要)
		/// </summary>
		object ISettings.View => new Views.SettingView() { DataContext = this.setting_VM };


		/// <summary>
		/// IRequestNotifyのインターフェースを実装(プラグイン側から本隊に通知を要求する）
		/// </summary>
		public event EventHandler<NotifyEventArgs> NotifyRequested;
        public void InvokeNotifyRequested(NotifyEventArgs e) => this.NotifyRequested?.Invoke(this, e);


		/// <summary>
		/// IRequestNotifyインターフェースの実装
		/// </summary>
		/// <param name="notification"></param>
		//      public void Notify(INotification notification)
		//      {
		//          throw new NotImplementedException();
		//      }


		/// <summary>
		/// IDisposableHolderインターフェースの実装
		/// </summary>
		private readonly StatefulModel.MultipleDisposable compositDisposable = new StatefulModel.MultipleDisposable();
		public void Dispose() => this.compositDisposable.Dispose();
		public ICollection<IDisposable> CompositeDisposable => this.compositDisposable;

		#endregion

	}
}
