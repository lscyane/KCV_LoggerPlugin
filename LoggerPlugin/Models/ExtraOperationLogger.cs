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
using Grabacr07.KanColleWrapper.Models;

namespace KCVLoggerPlugin.Models
{
    /// <summary>
    /// ログ管理クラス
    /// </summary>
    /// <remarks>
    /// 共通処理はLoggerBaseにまとめています
    /// </remarks>
    public class ExtraOperationLogger : LoggerBase
    {
        protected StatefulModel.MultipleDisposable compositeDisposable { get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public ExtraOperationLogger(LoggerPlugin plg) : base(plg, ExtraOperationLog.Instance)
        {
            this.compositeDisposable = new StatefulModel.MultipleDisposable();

            // KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
            {
                var proxy = KanColleClient.Current.Proxy;

                // マップ選択画面を開いた時のマップ情報取得
                proxy.ApiSessionSource
                    .Where(s => s.Request.PathAndQuery == "/kcsapi/api_get_member/mapinfo")
                    .TryParse<Raw.mapinfo>()
                    .Subscribe(async m =>
                    {
                        this.updateExtraOperation(m.Data.api_map_info);
                        await this.SaveAsync();
                    });

            }, false);
        }


        /// <summary>
        /// マップ情報更新
        /// </summary>
        /// <param name=""></param>
        private void updateExtraOperation(Raw.member_mapinfo[] result)
        {
            ExtraOperationLog logInstance = ExtraOperationLog.Instance;
            ExtraOperationLogStruct currentLog;
            if ((logInstance.History.Count == 0)
             || (logInstance.History.Last().DateTime.Year != DateTime.Now.Year)
             || (logInstance.History.Last().DateTime.Month != DateTime.Now.Month)
            ) {
                // 今月分新規作成
                currentLog = new ExtraOperationLogStruct(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                logInstance.History.Add(currentLog);
            }
            else
            {
                // 今月分を取得
                currentLog = logInstance.History.Last();
            }

            // ロギング
            logInstance.HistoryWriting = true;
            foreach (var info in result)
            {
                currentLog.Update(info.api_id, info.api_cleared);
            }
            logInstance.HistoryWriting = false;
        }
    }
}
