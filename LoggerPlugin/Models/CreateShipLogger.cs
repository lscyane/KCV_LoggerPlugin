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
	public class CreateShipLogger : LoggerBase
	{
		int kdock_id = 0;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="plg"></param>
		public CreateShipLogger(LoggerPlugin plg) : base(plg, CreateShipLog.Instance)
		{
			// KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
			{
				var materials = KanColleClient.Current.Homeport.Materials;
				var proxy = KanColleClient.Current.Proxy;

				// 建造リクエスト
				proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(cs =>
				{
					this.kdock_id = int.Parse(cs.Request.GetValues("api_kdock_id")[0]);
				});

				// 建造結果
				proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(async kd =>
				{
					this.updateKDock(kd.Data[this.kdock_id-1]);
					await this.SaveAsync();
				});
			}, false);
		}


		/// <summary>
		/// 建造結果
		/// </summary>
		/// <param name="result"></param>
		private void updateKDock(kcsapi_kdock result)
		{
			String shipName = "取得失敗";
			String shipType = "取得失敗";
			if (result.api_created_ship_id > 0)
			{
				shipName = KanColleClient.Current.Master.Ships[result.api_created_ship_id].Name;		     // 艦娘名
				shipType = KanColleClient.Current.Master.Ships[result.api_created_ship_id].ShipType.Name;    // 種別
			}

			// 秘書艦
			String secretary = String.Format("{0}(Lv{1})",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Level
			);

			// ロギング
			CreateShipLog logInstance = CreateShipLog.Instance;
			CreateShipLogStruct csls = new CreateShipLogStruct(
				DateTime.Now,                                    // 日時
				shipName,                                        // 装備名
				shipType,                                        // 種別
				result.api_item1,                                // 投入した燃料
				result.api_item2,                                // 投入した弾薬
				result.api_item3,                                // 投入した鋼材
				result.api_item4,                                // 投入したボーキサイト
				result.api_item5,                                // 投入した開発資材
				secretary,                                       // 秘書艦
				KanColleClient.Current.Homeport.Admiral.Level    // 司令部Lv
			);
			logInstance.HistoryAdd(csls);
		}


	}
}
