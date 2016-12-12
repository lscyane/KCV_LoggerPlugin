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
	public class MissionLogger : LoggerBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="plg"></param>
		public MissionLogger(LoggerPlugin plg) : base(plg, MissionLog.Instance)
		{
			// KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
			{
				var materials = KanColleClient.Current.Homeport.Materials;
				var proxy = KanColleClient.Current.Proxy;

				// 遠征結果
				proxy.api_req_mission_result.TryParse<kcsapi_mission_result_ex>().Subscribe(async mr =>
				{
					this.isSuccessResult = true;
					this.expeditionResult(mr.Data);
					await this.SaveAsync();
				});

				// 遠征結果(失敗時予備処理)
				proxy.api_req_mission_result.TryParse<kcsapi_mission_result_sub>().Subscribe(async mrs =>
				{
					string a = KanColleClient.Current.Master.UseItems[1].Name;


					if (!this.isSuccessResult)
					{
						this.expeditionResult((kcsapi_mission_result_ex)mrs.Data.ToOriginal());
						await this.SaveAsync();
					}
					this.isSuccessResult = false;
				});

			}, false);
		}


		private bool isSuccessResult = false;


		/// <summary>
		/// 遠征結果
		/// </summary>
		/// <param name="result"></param>
		private void expeditionResult(kcsapi_mission_result_ex mission)
		{
			// ログに出力する文字列
			String fleetString = "";
			// 艦隊情報
			for (int i = 1; i < mission.api_ship_id.Length; ++i)
			{
				int ship_id = mission.api_ship_id[i];
				Grabacr07.KanColleWrapper.Models.Ship ship = KanColleClient.Current.Homeport.Organization.Ships[ship_id];
				if (fleetString != "") { fleetString += ","; }
				fleetString += ship.Info.Name;
				fleetString += "(" + ship.Info.ShipType.Name + "Lv" + ship.Level + ")";
			}

			// 取得アイテム
			String getItem = "";
			if (mission.api_useitem_flag[0] > 0)
			{
				getItem += this.getItemName(mission.api_useitem_flag[0], mission.api_get_item1.api_useitem_name);
				getItem += "x" + mission.api_get_item1.api_useitem_count.ToString();
			}
			if (mission.api_useitem_flag[1] > 0)
			{
				if (getItem != "") { getItem += ","; }
				getItem += this.getItemName(mission.api_useitem_flag[1], mission.api_get_item2.api_useitem_name);
				getItem += "x" + mission.api_get_item2.api_useitem_count.ToString();
			}

			// ロギング
			MissionLog logInstance = MissionLog.Instance;
			MissionLogStruct csls = new MissionLogStruct(
				DateTime.Now,                       // 日時
				mission.api_maparea_name,           // 海域
				mission.api_quest_name,             // 遠征名
				mission.api_get_material[0],        // 燃料
				mission.api_get_material[1],        // 弾薬
				mission.api_get_material[2],        // 鋼材
				mission.api_get_material[3],        // ボーキサイト
				mission.api_clear_result,           // 結果
				fleetString,                        // 艦隊
				getItem                             // 獲得アイテム
			);
			logInstance.HistoryAdd(csls);
		}


		/// <summary>
		/// アイテム名を取得
		/// </summary>
		/// <param name="id">アイテムID</param>
		/// <param name="name">APIが指定してくるアイテム名</param>
		/// <returns>アイテム名</returns>
		private string getItemName(int id, object name)
		{
			// APIに名前指定があるときはそれを返す
			if (name != null && name.ToString() != "")
			{
				return name.ToString();
			}

			// idから名前を参照、無い時はID番号を返す
			string item = string.Format("ID:{0}", id);
			if ((id < KanColleClient.Current.Master.UseItems.Count) && (KanColleClient.Current.Master.UseItems[id].Name != ""))
			{
				item = KanColleClient.Current.Master.UseItems[id].Name;
			}
			return item;
		}

	}


	/// <summary>
	/// kcsapi_mission_result拡張クラス
	/// </summary>
	/// <remarks>
	/// 標準で用意されていないので独自で用意する。public必須
	/// </remarks>
	public class kcsapi_mission_result_ex : kcsapi_mission_result
	{
		public kcsapi_mission_result_ex() : base() { }

		public kcsapi_mission_result_item api_get_item2 { get; set; }
	}

	/// <summary>
	/// kcsapi_mission_result失敗時対策用クラス
	/// </summary>
	/// <remarks>
	/// 遠征失敗時パースに失敗している模様なのでパースできる形を用意する。public必須
	/// </remarks>
	public class kcsapi_mission_result_sub
	{
		public kcsapi_mission_result_sub() { }

		public int api_clear_result { get; set; }                
		public string api_detail { get; set; }                         
		public int api_get_exp { get; set; }                           
		public int[][] api_get_exp_lvup { get; set; }                
		//public kcsapi_mission_result_item api_get_item1 { get; set; }	// 失敗時はapi_get_item1のAPI自体が来ないせいかパースできない要因になっている
		public int api_get_material { get; set; }                       // 失敗時は配列ではないせいかパースできない要因になってい
		public int[] api_get_ship_exp { get; set; }                  
		public string api_maparea_name { get; set; }                 
		public int api_member_exp { get; set; }                        
		public int api_member_lv { get; set; }                   
		public int api_quest_level { get; set; }                       
		public string api_quest_name { get; set; }                
		public int[] api_ship_id { get; set; }                    
		public int[] api_useitem_flag { get; set; } 

		public kcsapi_mission_result_ex ToOriginal()
		{
			kcsapi_mission_result_ex retval = new kcsapi_mission_result_ex();
			retval.api_clear_result = this.api_clear_result;
			retval.api_detail = this.api_detail;
			retval.api_get_exp = this.api_get_exp;
			retval.api_get_exp_lvup = this.api_get_exp_lvup;
			retval.api_get_item1 = null;
			retval.api_get_item2 = null;
			retval.api_get_material = new int[] { 0, 0, 0, 0};
			retval.api_get_ship_exp = this.api_get_ship_exp;
			retval.api_maparea_name = this.api_maparea_name;
			retval.api_member_exp = this.api_member_exp;
			retval.api_member_lv = this.api_member_lv;
			retval.api_quest_level = this.api_quest_level;
			retval.api_quest_name = this.api_quest_name;
			retval.api_ship_id = this.api_ship_id;
			retval.api_useitem_flag = this.api_useitem_flag;
			return retval;
		}
	}
}

