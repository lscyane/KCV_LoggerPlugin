using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using MetroTrilithon.Mvvm;
using System.IO;
using ProtoBuf;
using System.ComponentModel;
using System.Reactive.Linq;

namespace KCVLoggerPlugin.Models
{
    /// <summary>
    /// ログ管理クラス
    /// </summary>
    /// <remarks>
    /// 共通処理はLoggerBaseにまとめています
    /// </remarks>
    public class AachievementLogger : LoggerBase
    {
        protected StatefulModel.MultipleDisposable compositeDisposable { get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public AachievementLogger(LoggerPlugin plg) : base(plg, AachievementLog.Instance)
        {
            this.compositeDisposable = new StatefulModel.MultipleDisposable();

            // KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
            {
                // 提督情報更新
                this.compositeDisposable.Add(new StatefulModel.EventListeners.PropertyChangedEventListener(KanColleClient.Current.Homeport)
                {
                    nameof(Homeport.Admiral), async (sender, args) => {
                        this.updateAachievement(((Homeport)sender).Admiral);
                        await this.SaveAsync();
                    },
                });
            }, false);
        }


        /// <summary>
        /// 開発結果
        /// </summary>
        /// <param name=""></param>
        private void updateAachievement(Grabacr07.KanColleWrapper.Models.Admiral result)
        {
            // ロギング
            AachievementLog logInstance = AachievementLog.Instance;
            if ((logInstance.History.Count == 0)
             || (logInstance.History.Last().AdmiralExp != result.Experience)
            ) {
				// ページ再読込時あたりのタイミングで0を取得することがあるので弾く
				if (result.Experience != 0)
				{
					AachievementLogStruct cils = new AachievementLogStruct(
						DateTime.Now,                                    // 日時
						result.Experience                                // 提督経験値
					);
					logInstance.HistoryAdd(cils);

				}
			}
        }
    }
}
