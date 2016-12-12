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

namespace KCVLoggerPlugin.Models
{
	/// <summary>
	/// ログ管理クラス
	/// </summary>
	public class BattleLogger : LoggerBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="plg"></param>
		public BattleLogger(LoggerPlugin plg) : base(plg, BattleLog.Instance)
		{
			// KanColleClientのIsStartedがtrueに変更されたらデータの購読を開始
			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), () =>
			{
				var proxy = KanColleClient.Current.Proxy;

				// 戦闘開始(NPC)
				proxy.api_req_sortie_battle.TryParse<kcsapi_battle_ex>().Subscribe(bt =>
				{
					this.battle_ex = bt.Data;
				});

				// 戦闘結果
				proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(async br =>
				{
					this.updateBattleResult(br.Data);
					await this.SaveAsync();
				});

			}, false);
		}


		/// <summary>
		/// 戦闘開始時の情報。
		/// 味方艦情報は戦闘結果には入らないので戦闘結果を受信するまで保持しておく。
		/// </summary>
		private kcsapi_battle_ex battle_ex = null;


		/// <summary>
		/// 戦闘結果の受信
		/// </summary>
		/// <param name="result"></param>
		private void updateBattleResult(kcsapi_battleresult result)
		{
			// 通常昼戦時の艦隊情報
			Grabacr07.KanColleWrapper.Models.Fleet sortieFleet = KanColleClient.Current.Homeport.Organization.Fleets[this.battle_ex.api_dock_id];

			// ログに出力する文字列
			String fleetString = sortieFleet.Name;				// 艦隊名
			for (int i = 0; i < sortieFleet.Ships.Length; ++i)	// 艦娘名
			{
				Grabacr07.KanColleWrapper.Models.Ship ship = sortieFleet.Ships[i];
				String mvp = "";
				if (i == result.api_mvp - 1)
				{
					mvp = "*";    // MVP
				}
				fleetString += "," + mvp + ship.Info.Name;
				fleetString += "(Lv" + ship.Level;
				fleetString += " HP" + ship.HP.Current.ToString() + "/" + ship.HP.Maximum.ToString() + ")";
			}

			// ログに出力する「敵艦隊」の文字列
			String eFleetString = result.api_enemy_info.api_deck_name;          // 敵艦隊名
			foreach (int shipID in result.api_ship_id)
			{
				if (shipID == -1)
				{
					continue;
				}
				Grabacr07.KanColleWrapper.Models.ShipInfo shipInfo = KanColleClient.Current.Master.Ships[shipID];
				eFleetString += "," + shipInfo.Name;
				if ((shipInfo.Kana != null) && (shipInfo.Kana != "-"))
				{
					eFleetString += "(" + shipInfo.Kana + ")";  // flagshipとかはここに入るっぽい
				}
			}

			// ドロップ
			String drop = "";
			if (result.api_get_ship != null)
			{
				drop = result.api_get_ship.api_ship_name + "(" + result.api_get_ship.api_ship_type + ")";
			}

			// ロギング
			BattleLog logInstance = BattleLog.Instance;
			BattleLogStruct bls = new BattleLogStruct(
				DateTime.Now,						// 日時
				result.api_quest_name,				// 海域
				fleetString,						// 艦隊
				eFleetString,						// 敵艦隊
				result.api_win_rank,				// 戦闘結果
				drop,								// ドロップ
				this.battle_ex.api_formation[0],    // 自陣形
				this.battle_ex.api_formation[1],    // 敵陣形
				this.battle_ex.api_formation[2],	// 交戦形態
				this.battle_ex.api_kouku.api_stage1.api_disp_seiku	// 制空権
				);
			logInstance.HistoryAdd(bls);
		}
	}


	/// <summary>
	/// kcsapi_battle拡張クラス
	/// </summary>
	/// <remarks>
	/// 標準で用意されていないので独自で用意する。public必須
	/// </remarks>
	public class kcsapi_battle_ex : kcsapi_battle
	{
		public kcsapi_battle_ex() : base() { }

		public int[] api_formation { get; set; }
		public kcsapi_kouku api_kouku { get; set; }

		public class kcsapi_kouku
		{
			public int[][] api_plane_from { get; set; }
			public kcsapi_api_stage api_stage1 { get; set; }
			public kcsapi_api_stage api_stage2 { get; set; }
			public kcsapi_api_stage api_stage3 { get; set; }

			public class kcsapi_api_stage
			{
				public int api_f_count { get; set; }
				public int api_f_lostcount { get; set; }
				public int api_e_count { get; set; }
				public int api_e_lostcount { get; set; }
				public int api_disp_seiku { get; set; }
				public int[] api_touch_plane { get; set; }
			}
		}

#if false
		{POST /kcsapi/api_req_sortie/battle HTTP/1.1
		svdata={
			"api_result":1,
			"api_result_msg":"\u6210\u529f",
			"api_data":{
				"api_dock_id":1,
				"api_ship_ke":[-1,501,-1,-1,-1,-1,-1],
				"api_ship_lv":[-1,1,-1,-1,-1,-1,-1],
				"api_nowhps":[-1,42,32,11,6,27,1,20,-1,-1,-1,-1,-1],
				"api_maxhps":[-1,42,36,16,16,30,13,20,-1,-1,-1,-1,-1],
				"api_midnight_flag":0,
				"api_eSlot":[[501,-1,-1,-1,-1],[-1,-1,-1,-1,-1],[-1,-1,-1,-1,-1],[-1,-1,-1,-1,-1],[-1,-1,-1,-1,-1],[-1,-1,-1,-1,-1]],
				"api_eKyouka":[[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0]],
				"api_fParam":[[59,79,52,59],[39,71,35,37],[11,28,11,9],[23,51,25,13],[25,44,25,25],[11,26,14,9]],
				"api_eParam":[[5,15,6,5],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0]],
				"api_search":[5,5],
				"api_formation":[1,1,1],
				"api_stage_flag":[1,0,0],
				"api_kouku":{
					"api_plane_from":[[-1],[-1]],
					"api_stage1":{
						"api_f_count":0,
						"api_f_lostcount":0,
						"api_e_count":0,
						"api_e_lostcount":0,
						"api_disp_seiku":1,
						"api_touch_plane":[-1,-1]
					},
					"api_stage2":null,
					"api_stage3":null
				},
				"api_support_flag":0,
				"api_support_info":null,
				"api_opening_taisen_flag":0,
				"api_opening_taisen":null,
				"api_opening_flag":0,
				"api_opening_atack":null,
				"api_hourai_flag":[1,0,0,0],
				"api_hougeki1":{
					"api_at_list":[-1,1],
					"api_at_type":[-1,0],
					"api_df_list":[-1,[7]],
					"api_si_list":[-1,[-1]],
					"api_cl_list":[-1,[1]],
					"api_damage":[-1,[59]]
				},
				"api_hougeki2":null,
				"api_hougeki3":null,
				"api_raigeki":null
			}
		}
#endif
	}
}
