using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using KCVLoggerPlugin.Models;
using System.Threading.Tasks;

namespace KCVLoggerPlugin.ViewModels
{
	public class ToolViewModel : ViewModel
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
		/// ログ管理クラス
		/// </summary>
		private CreateItemLogger ciLogManager { get; set; }
		private CreateShipLogger csLogManager { get; set; }
		private BattleLogger bLogManager { get; set; }
		private MissionLogger mLogManager { get; set; }
		private MaterialLogger maLogManager { get; set; }
		private AachievementLogger aLogManager { get; set; }
        private ExtraOperationLogger eoLogManager { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plugin"></param>
        public ToolViewModel(LoggerPlugin plugin)
		{
			this.plugin = plugin;
			this.ciLogManager = new CreateItemLogger(plugin);
			this.csLogManager = new CreateShipLogger(plugin);
			this.bLogManager = new BattleLogger(plugin);
			this.mLogManager = new MissionLogger(plugin);
			this.maLogManager = new MaterialLogger(plugin);
			this.aLogManager = new AachievementLogger(plugin);
            this.eoLogManager = new ExtraOperationLogger(plugin);
        }


		/// <summary>
		/// 初期化
		/// プラグイン本体から呼ばれる
		/// </summary>
		public async void Initialize()
		{
			Task cilogTask;
			Task cslogTask;
			Task blogTask;
			Task mlogTask;
			Task malogTask;
			Task alogTask;
            Task eologTask;

            #region CreateItemLogger開始
            {
				// プロパティの変化を監視
				CreateItemLog cilog = CreateItemLog.Instance;
				var cilogChangedListener = new PropertyChangedEventListener(cilog)
				{
                    // DB更新時の通知設定
                    nameof(cilog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!cilog.HistoryWriting) {
							this.RefleshCreateItemLog();
						}
					},
				};
				// DB初回読み込み開始
				cilogTask = this.ciLogManager.LoadAsync();
			}
			#endregion

			#region CreateShipLogger開始
			{
				// プロパティの変化を監視
				CreateShipLog cslog = CreateShipLog.Instance;
				var cslogChangedListener = new PropertyChangedEventListener(cslog)
				{
                    // DB更新時の通知設定
                    nameof(cslog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!cslog.HistoryWriting) {
							this.RefleshCreateShipLog();
						}
					}
				};
				// DB初回読み込み開始
				cslogTask = this.csLogManager.LoadAsync();
			}
			#endregion

			#region BattleLogger開始
			{
				// プロパティの変化を監視
				BattleLog blog = BattleLog.Instance;
				var blogChangedListener = new PropertyChangedEventListener(blog)
				{
                    // DB更新時の通知設定
                    nameof(blog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!blog.HistoryWriting) {
							this.RefleshBattleLog();
						}
					}
				};
				// DB初回読み込み開始
				blogTask = this.bLogManager.LoadAsync();
			}
			#endregion

			#region MissionLogger開始
			{
				// プロパティの変化を監視
				MissionLog mlog = MissionLog.Instance;
				var mlogChangedListener = new PropertyChangedEventListener(mlog)
				{
                    // DB更新時の通知設定
                    nameof(mlog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!mlog.HistoryWriting) {
							this.RefleshMissionLog();
						}
					}
				};
				// DB初回読み込み開始
				mlogTask = this.mLogManager.LoadAsync();
			}
			#endregion

			#region MaterialLogger開始
			{
				// プロパティの変化を監視
				MaterialLog malog = MaterialLog.Instance;
				var malogChangedListener = new PropertyChangedEventListener(malog)
				{
                    // DB更新時の通知設定
                    nameof(malog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!malog.HistoryWriting) {
							this.RefleshMaterialLog();
						}
					}
				};
				// DB初回読み込み開始
				malogTask = this.maLogManager.LoadAsync();
			}
			#endregion

			#region AachievementLogger開始
			{
				// プロパティの変化を監視
				AachievementLog alog = AachievementLog.Instance;
				var alogChangedListener = new PropertyChangedEventListener(alog)
				{
                    // DB更新時の通知設定
                    nameof(alog.HistoryWriting), (_, __) =>
					{
                        // データ非同期読み込み
                        if (!alog.HistoryWriting) {
							this.RefleshAachievementLog();
                            this.RefleshAachievement();
                        }
					}
				};
				// DB初回読み込み開始
				alogTask = this.aLogManager.LoadAsync();
			}
            #endregion

            #region ExtraOperationLogger開始
            {
                // プロパティの変化を監視
                ExtraOperationLog eolog = ExtraOperationLog.Instance;
                var eologChangedListener = new PropertyChangedEventListener(eolog)
                {
                    // DB更新時の通知設定
                    nameof(eolog.HistoryWriting), (_, __) =>
                    {
                        // データ非同期読み込み
                        if (!eolog.HistoryWriting) {
                            this.RefleshAachievement();
                        }
                    }
                };
                // DB初回読み込み開始
                eologTask = this.eoLogManager.LoadAsync();
            }
            #endregion

            await Task.WhenAll(cilogTask, cslogTask, blogTask, mlogTask, malogTask, alogTask, eologTask);
                        }


		/// <summary>
		/// 終了処理
		/// </summary>
		public new void Dispose()
		{
			List<Task> tasks = new List<Task>();

			// データが存在するログのみを保存する
			if (Models.CreateItemLog.Instance.History.Count > 0)
			{
				tasks.Add(this.ciLogManager.SaveAsync());
			}
			if (Models.CreateShipLog.Instance.History.Count > 0)
			{
				tasks.Add(this.csLogManager.SaveAsync());
			}
			if (Models.BattleLog.Instance.History.Count > 0)
			{
				tasks.Add(this.bLogManager.SaveAsync());
			}
			if (Models.MissionLog.Instance.History.Count > 0)
			{
				tasks.Add(this.mLogManager.SaveAsync());
			}
			if (Models.MaterialLog.Instance.History.Count > 0)
			{
				tasks.Add(this.maLogManager.SaveAsync());
			}
			if (Models.AachievementLog.Instance.History.Count > 0)
			{
				tasks.Add(this.aLogManager.SaveAsync());
			}
            if (Models.ExtraOperationLog.Instance.History.Count > 0)
            {
                tasks.Add(this.eoLogManager.SaveAsync());
            }

            // 保存するログがある場合、完了を待機
            if (tasks.Count > 0)
			{
				Task.WaitAll(tasks.ToArray());
			}

			base.Dispose();
		}


		/// <summary>
		/// リストのデータをリフレッシュします。
		/// </summary>
		public void RefleshCreateItemLog()
		{
			// 新しいデータを上部にするため反転して返す
			this.CreateItemLogList = new ObservableCollection<CreateItemLogStruct>(CreateItemLog.Instance.History.Reverse());
		}


		/// <summary>
		/// リストのデータをリフレッシュします。
		/// </summary>
		public void RefleshCreateShipLog()
		{
			// 新しいデータを上部にするため反転して返す
			this.CreateShipLogList = new ObservableCollection<CreateShipLogStruct>(CreateShipLog.Instance.History.Reverse());
		}


		/// <summary>
		/// リストのデータをリフレッシュします。
		/// </summary>
		public void RefleshBattleLog()
		{
			// 新しいデータを上部にするため反転して返す
			this.BattleLogList = new ObservableCollection<BattleLogStruct>(BattleLog.Instance.History.Reverse());
			// 5-5カウンター更新
			int count = 0;
			foreach (BattleLogStruct bls in this.BattleLogList)
			{
				if ((DateTime.Now.Year != bls.DateTime.Year)
				 || (DateTime.Now.Month != bls.DateTime.Month)
				)
				{
					break;
				}
				if ((bls.Area == "サーモン海域") && (bls.Foe.IndexOf("敵補給部隊本体") >= 0))
				{
					if (bls.Result.IndexOfAny(new char[] { 'S', 'A', 'B' }) >= 0)
					{
						count++;
					}
				}
			}
			this.Count55 = count.ToString();
		}


		/// <summary>
		/// リストのデータをリフレッシュします。
		/// </summary>
		public void RefleshMissionLog()
		{
			// 新しいデータを上部にするため反転して返す
			this.MissionLogList = new ObservableCollection<MissionLogStruct>(MissionLog.Instance.History.Reverse());
		}



		/// <summary>
		/// リストのデータをリフレッシュします。
		/// </summary>
		public void RefleshMaterialLog()
		{
			// 新しいデータを上部にするため反転して返す
			this.MaterialLogList = new ObservableCollection<MaterialLogStruct>(MaterialLog.Instance.History.Reverse());
		}


        /// <summary>
        /// リストのデータをリフレッシュします。
        /// </summary>
        public void RefleshAachievementLog()
        {
            // データがないときは処理しない
            if (AachievementLog.Instance.History.Count == 0)
            {
                return;
            }

            // 新しいデータを上部にするため反転して返す
            this.AachievementLogList = new ObservableCollection<AachievementLogStruct>(AachievementLog.Instance.History.Reverse());

            // 表示用値の計算
            for (int i = 0; i < this.AachievementLogList.Count - 1; ++i)
            {
                this.AachievementLogList[i].Incremental = this.AachievementLogList[i].AdmiralExp - this.AachievementLogList[i + 1].AdmiralExp;
            }
        }


        /// <summary>
        /// リストのデータをリフレッシュします。
        /// </summary>
        public void RefleshAachievement()
        {
            // データがないときは処理しない
            if (AachievementLog.Instance.History.Count == 0)
            {
                return;
            }

            int 前月EO = ExtraOperationLog.Instance.GetLastMonthEOAachievement();
            int 今月EO = ExtraOperationLog.Instance.GetThisMonthEOAachievement();

            // 現在戦果の計算
            AachievementLogStruct baseStruct;
            {
                // 引き継ぎ戦果の計算基準となるデータ
                DateTime baseDate = new DateTime(DateTime.Now.Year, 12, 31, 22, 0, 0);  // 今年末の22時
                if (DateTime.Now < baseDate)
                {
                    baseDate = baseDate.AddYears(-1);   // 繰越戦果の基準日は前年末の22時
                }
                baseStruct = this.AachievementLogList.ToList().Find(x =>
                {
                    return (x.DateTime < baseDate);
                });
                if (baseStruct == null) // もし見つからなかったら一番古いログを使用する
                {
                    baseStruct = AachievementLog.Instance.History[0];
                }
            }
            AachievementLogStruct pointStruct;
            {
                // 今月戦果の計算基準となるデータ
                DateTime pointDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 22, 0, 0).AddMonths(1).AddDays(-1); // 今月末の22時
                if (DateTime.Now < pointDate)
                {
                    pointDate = pointDate.AddDays(1).AddMonths(-1).AddDays(-1);    // 差分基準日は先月末の22時
                }
                else
                {
                    今月EO = 0;   // 経験値戦果は22時〆なのでEO戦果の計算も打ち切る
                }
                pointStruct = this.AachievementLogList.ToList().Find(x =>
                {
                    return (x.DateTime < pointDate);
                });
                if (pointStruct == null) // もし見つからなかったら一番古いログを使用する
                {
                    pointStruct = this.AachievementLogList.Last();
                }
            }
            AachievementLogStruct prevStruct;   // 前日時点の戦果
            {
                DateTime prevDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 0, 0);
                if (DateTime.Now < prevDate)
                {
                    prevDate = prevDate.AddDays(-1);
                }
                prevStruct = this.AachievementLogList.ToList().Find(x =>
                {
                    return (x.DateTime < prevDate);
                });
                if (prevStruct == null) // もし見つからなかったら一番古いログを使用する
                {
                    prevStruct = this.AachievementLogList.Last();
                }
            }

            float 引継戦果 = ((pointStruct.AdmiralExp - baseStruct.AdmiralExp) / 50000f) + (int)(前月EO / 35f);
            float 経験値戦果 = (this.AachievementLogList[0].AdmiralExp - pointStruct.AdmiralExp) / 1428f;
            float 前日経験値戦果 = (prevStruct.AdmiralExp - pointStruct.AdmiralExp) / 1428f;
            this.NowAachievement = 経験値戦果 + 引継戦果 + 今月EO;
            this.PrevDiffAachievement = 経験値戦果 - 前日経験値戦果 + ExtraOperationLog.Instance.GetTodayMonthEOAachievement();
            this.ThisMonthEOAach = 今月EO;
            this.LastMonthEOAach = 前月EO;
#if DEBUG
            this.DebugText = "\n【Debug】"
                + "\n  base :" + baseStruct.DateTime.ToString()
                + "\n  point:" + pointStruct.DateTime.ToString()
                + "\n  prev :" + prevStruct.DateTime.ToString();
#endif
            RaisePropertyChanged(nameof(NowAachievement));
            RaisePropertyChanged(nameof(PrevDiffAachievement));
            RaisePropertyChanged(nameof(ThisMonthEOAach));
            RaisePropertyChanged(nameof(LastMonthEOAach));
            RaisePropertyChanged(nameof(DebugText));
        }


        #region BindProperty

        /// <summary>
        /// CreateItemLogのリストビューデータ
        /// </summary>
        ObservableCollection<CreateItemLogStruct> _createItemLogList = new ObservableCollection<CreateItemLogStruct>();
		public ObservableCollection<CreateItemLogStruct> CreateItemLogList
		{
			get
			{
				return this._createItemLogList;
			}
			set
			{
				this._createItemLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(CreateItemLogList));
			}
		}

		ObservableCollection<CreateShipLogStruct> _createShipLogList = new ObservableCollection<CreateShipLogStruct>();
		public ObservableCollection<CreateShipLogStruct> CreateShipLogList
		{
			get
			{
				return this._createShipLogList;
			}
			set
			{
				this._createShipLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(CreateShipLogList));
			}
		}

		ObservableCollection<BattleLogStruct> _battleLogList = new ObservableCollection<BattleLogStruct>();
		public ObservableCollection<BattleLogStruct> BattleLogList
		{
			get
			{
				return this._battleLogList;
			}
			set
			{
				this._battleLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(BattleLogList));
			}
		}

		ObservableCollection<MissionLogStruct> _missionLogList = new ObservableCollection<MissionLogStruct>();
		public ObservableCollection<MissionLogStruct> MissionLogList
		{
			get
			{
				return this._missionLogList;
			}
			set
			{
				this._missionLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(MissionLogList));
			}
		}

		ObservableCollection<MaterialLogStruct> _materialLogList = new ObservableCollection<MaterialLogStruct>();
		public ObservableCollection<MaterialLogStruct> MaterialLogList
		{
			get
			{
				return this._materialLogList;
			}
			set
			{
				this._materialLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(MaterialLogList));
			}
		}

		ObservableCollection<AachievementLogStruct> _aachievementLogList = new ObservableCollection<AachievementLogStruct>();
		public ObservableCollection<AachievementLogStruct> AachievementLogList
		{
			get
			{
				return this._aachievementLogList;
			}
			set
			{
				this._aachievementLogList = value;
				// バインドしているViewに変更を通知
				RaisePropertyChanged(nameof(AachievementLogList));
			}
		}


		private String _count55 = "";
		public String Count55
		{
			get
			{
				return this._count55;
			}
			set
			{
				this._count55 = value;
				RaisePropertyChanged(nameof(Count55));
			}
		}
        public float NowAachievement { get; set; }
        public float PrevDiffAachievement { get; set; }
        public int ThisMonthEOAach { get; set; }
        public int LastMonthEOAach { get; set; }
        public string DebugText { get; set; }
        #endregion
    }

}
