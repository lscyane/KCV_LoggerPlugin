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
    public class RankingLogger : LoggerBase
    {
        protected StatefulModel.MultipleDisposable compositeDisposable { get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public RankingLogger(LoggerPlugin plg) : base(plg, RankingLog.Instance)
        {
            this.compositeDisposable = new StatefulModel.MultipleDisposable();

            // KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
            {
                var proxy = KanColleClient.Current.Proxy;

                // ランキング画面を開いた時の情報取得
                proxy.ApiSessionSource
                    .Where(s => {
                        return s.Request.PathAndQuery == "/kcsapi/api_req_ranking/mxltvkpyuklh";
                    })
                    .TryParse<Raw.datalist>()
                    .Subscribe(async m =>
                    {
                        this.updateRanking(m.Data.api_list);
                        await this.SaveAsync();
                    });

            }, false);
        }


        /// <summary>
        /// 開発結果
        /// </summary>
        /// <param name=""></param>
        private void updateRanking(Raw.member_datalist[] list)
        {
            // 基準時間
            DateTime now = DateTime.Now.AddHours(-3);
            DateTime lastUpdateTime = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
            if (lastUpdateTime > now)
            {
                lastUpdateTime = lastUpdateTime.AddHours(-12);
            }
            lastUpdateTime = lastUpdateTime.AddHours(3);

            // 新規作成・追加
            RankingLog logInstance = RankingLog.Instance;
            if ((logInstance.History.Count == 0)
             || (logInstance.History.Last().DateTime < lastUpdateTime)
            )
            {
                RankingLogStruct rls = new RankingLogStruct(lastUpdateTime);
                logInstance.HistoryAdd(rls);
            }

            // ロギング
            logInstance.HistoryWriting = true;
            foreach (var item in list) {
                logInstance.History.Last().Update(item.api_mtjmdcwtvhdr, item.api_wuhnhojjxmke, item.api_mxltvkpyuklh);
            }
            logInstance.HistoryWriting = false;

            // debug
            foreach(var dat in logInstance.History.Last().Admiral)
            {
                //System.Diagnostics.Debug.WriteLine("[" + dat.Value.rank.ToString() + "] " + dat.Key + " : " + dat.Value.戦果() + "(" + dat.Value.勲章数() + ")");
                System.Diagnostics.Debug.WriteLine(dat.Value.rank.ToString() + "\t" + dat.Value.achievement);
            }
        }
    }
}
