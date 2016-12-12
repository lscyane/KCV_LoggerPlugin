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
    public class MaterialLogger : LoggerBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public MaterialLogger(LoggerPlugin plg) : base(plg, MaterialLog.Instance)
        {
            // KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
            {
                var materials = KanColleClient.Current.Homeport.Materials;

                // 資材情報更新
                Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => (sender, e) => h(e),
                    h => materials.PropertyChanged += h,
                    h => materials.PropertyChanged -= h)
                    // プロパティ名が一致しているか調べて
                    .Where(e => this.isObservedPropertyName(e.PropertyName))
                    // まとめて通知が来るので10ms待機して
                    .Throttle(TimeSpan.FromMilliseconds(10))
                    // 処理
                    .Subscribe( async _ =>
                    {
                        if (this.updateMaterials(materials))
						{
							await this.SaveAsync();
						}
                    }
                );
            }, false);
        }


        /// <summary>
        /// 監視対象のプロパティ名と一致しているかを調べます。
        /// </summary>
        /// <param name="propertyName">変更が通知されたプロパティ名</param>
        /// <returns></returns>
        private bool isObservedPropertyName(string propertyName)
        {
            var materials = KanColleClient.Current.Homeport.Materials;
            return propertyName == nameof(materials.Fuel) || propertyName == nameof(materials.Ammunition)
                || propertyName == nameof(materials.Steel) || propertyName == nameof(materials.Bauxite)
                || propertyName == nameof(materials.InstantRepairMaterials);
        }


        /// <summary>
        /// 資源記録
        /// </summary>
        /// <param name=""></param>
        private bool updateMaterials(Materials materials)
        {
			bool modifyFlag = false;
            MaterialLog logInstance = MaterialLog.Instance;

			// ロギング
			MaterialLogStruct logst = new MaterialLogStruct(
				DateTime.Now,                       // 日時
				materials.Fuel,                     // 燃料
				materials.Ammunition,               // 弾薬
				materials.Steel,                    // 鋼材
				materials.Bauxite,                  // ボーキサイト
				materials.InstantRepairMaterials,   // 高速修復材
				materials.InstantBuildMaterials,    // 高速建造材
				materials.DevelopmentMaterials,     // 開発資材
				materials.ImprovementMaterials      // 改修資材
			);

			DateTime lastUpdate = (logInstance.History.Count == 0 ? DateTime.MinValue : logInstance.History.Last().DateTime);
			DateTime baseTime = (DateTime.Now.Hour < 5 ? DateTime.Today.AddHours(5 - 24) : DateTime.Today.AddHours(5));
			int minu = Properties.Settings.Default.MaterialLogInterval;
			if ((lastUpdate < baseTime)                             // 基準時間後の最初は無条件で記録
			 || (	(lastUpdate.AddMinutes(minu) < DateTime.Now)	// 最後の記録からInterval時間以上経過している時
				&&  (!logInstance.History.Last().Equals(logst)))	// 資源に変化がある時
			) {
				logInstance.HistoryAdd(logst);
				modifyFlag = true;
			}

			return modifyFlag;
        }
    }
}
