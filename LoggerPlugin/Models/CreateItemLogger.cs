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
    public class CreateItemLogger : LoggerBase
    {

        /// <summary>
        /// Current.Homeport.Materials ではなぜか開発等後の資材情報を更新してくれないのでローカルで管理する
        /// </summary>
        private int fuel;
        private int ammunition;
        private int steel;
        private int bauxite;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plg"></param>
        public CreateItemLogger(LoggerPlugin plg) : base(plg, CreateItemLog.Instance)
        {
            // KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
            {
                var materials = KanColleClient.Current.Homeport.Materials;
                var proxy = KanColleClient.Current.Proxy;

                // 資材情報更新
                this.updateLocalMaterials(materials);
                Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => (sender, e) => h(e),
                    h => materials.PropertyChanged += h,
                    h => materials.PropertyChanged -= h)
                    // プロパティ名が一致しているか調べて
                    .Where(e => this.isObservedPropertyName(e.PropertyName))
                    // まとめて通知が来るので10ms待機して
                    .Throttle(TimeSpan.FromMilliseconds(10))
                    // 処理
                    .Subscribe( _ =>
                    {
                        this.updateLocalMaterials(materials);
                    }
                );

                // 資材情報更新（建造)
                /* proxy.api_get_member_kdock.TryParse<kcsapi_kdock>().Subscribe(kd =>
                {
                    // 建造後に Homeport.Materials が更新されるので補正不要
                    this.fuel -= result.api_item1;
                    this.ammunition -= result.api_item2;
                    this.steel -= result.api_item3;
                    this.bauxite -= result.api_item4;
                }); */

                // 資材情報更新(廃棄)
                proxy.api_req_kousyou_destroyitem2.TryParse<kcsapi_destroyitem2>().Subscribe(di =>
                {
                    this.fuel += di.Data.api_get_material[0];
                    this.ammunition += di.Data.api_get_material[1];
                    this.steel += di.Data.api_get_material[2];
                    this.bauxite += di.Data.api_get_material[3];
                });

                // 資材情報更新(解体)
                /* proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(ds =>
                {
                    // 解体後に Homeport.Materials が更新されるので補正不要
                    this.updateLocalMaterials(ds.Data.api_material);
                }); */

                // 開発結果
                proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(async x =>
                {
                    this.updateCreateItem(x.Data);
                    await this.SaveAsync();
                });
            }, false);
        }


        /// <summary>
        /// ローカル資源情報の更新
        /// </summary>
        /// <param name="materials"></param>
        private void updateLocalMaterials(Materials materials)
        {
            int[] mat = { materials.Fuel, materials.Ammunition, materials.Steel, materials.Bauxite };
            this.updateLocalMaterials(mat);
        }
        private void updateLocalMaterials(int[] materials)
        {
            if (materials.Length >= 4)
            {
                this.fuel = materials[0];
                this.ammunition = materials[1];
                this.steel = materials[2];
                this.bauxite = materials[3];
            }
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
        /// 開発結果
        /// </summary>
        /// <param name=""></param>
        private void updateCreateItem(kcsapi_createitem result)
        {
            String itemName;
            String itemType;
            if (result.api_slot_item != null)
            {
                // 装備名
                itemName = KanColleClient.Current.Master.SlotItems[result.api_slot_item.api_slotitem_id].Name;
                // 装備種別
                itemType = KanColleClient.Current.Master.SlotItems[result.api_slot_item.api_slotitem_id].Type.ToString();
            }
            else
            {
                itemName = "(失敗)";
                itemType = "";
            }

            // 秘書艦
            String secretary = String.Format("{0}(Lv{1})",
                KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
                KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Level
            );

            // ロギング
            CreateItemLog logInstance = CreateItemLog.Instance;
            CreateItemLogStruct cils = new CreateItemLogStruct(
                DateTime.Now,                                   // 日時
                itemName,                                       // 装備名
                itemType,                                       // 種別
                (this.fuel - result.api_material[0]),           // 燃料
                (this.ammunition - result.api_material[1]),     // 弾薬
                (this.steel - result.api_material[2]),          // 鋼材
                (this.bauxite - result.api_material[3]),        // ボーキサイト
                secretary,                                      // 秘書艦
                KanColleClient.Current.Homeport.Admiral.Level  　// 司令部Lv
            );
            logInstance.HistoryAdd(cils);

            // ローカルの資材情報を更新
            this.updateLocalMaterials(result.api_material);
        }
    }
}
