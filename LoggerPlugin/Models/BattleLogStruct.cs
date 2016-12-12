using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class BattleLogStruct : IEquatable<BattleLogStruct>
    {
        /// <summary>
        /// 時刻
        /// </summary>
        [ProtoMember(1)]
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// 海域
        /// </summary>
        [ProtoMember(2)]
        public String Area { get; private set; }

        /// <summary>
        /// 艦隊
        /// </summary>
        [ProtoMember(3)]
        public String Friend { get; private set; }

        /// <summary>
        /// 敵艦隊
        /// </summary>
        [ProtoMember(4)]
        public String Foe { get; private set; }

        /// <summary>
        /// 戦闘結果
        /// </summary>
        [ProtoMember(5)]
        public String Result { get; private set; }

        /// <summary>
        /// ドロップ
        /// </summary>
        [ProtoMember(6)]
        public String Drop { get; private set; }

        /// <summary>
        /// CSV Import flag
        /// </summary>
        [ProtoMember(7)]
        public bool CsvFlag { get; private set; }

		/// <summary>
		/// 自陣形
		/// </summary>
		[ProtoMember(8)]
		public int FriendFormation { get; private set; }

		/// <summary>
		/// 敵陣形
		/// </summary>
		[ProtoMember(9)]
		public int EnemyFormation { get; private set; }

		/// <summary>
		/// 交戦形態
		/// </summary>
		[ProtoMember(10)]
		public int MatchType { get; private set; }

		/// <summary>
		/// 制空権
		/// </summary>
		[ProtoMember(11)]
		public int AirSuperityType { get; private set; }


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>
		/// 通常不使用 (Deserialize時に必要な実装)
		/// </remarks>
		public BattleLogStruct() { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleLogStruct(DateTime dateTime, String area, String friend, String foe, String result, String drop, int friendForm, int enemyForm, int match, int seiku, bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.Area = area;
            this.Friend = friend;
            this.Foe = foe;
            this.Result = result;
            this.Drop = drop;
            this.CsvFlag = isCsv;
			this.FriendFormation = friendForm;
			this.EnemyFormation = enemyForm;
			this.MatchType = match;
			this.AirSuperityType = seiku;
		}


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(BattleLogStruct other)
        {
            return (this.DateTime == other.DateTime)
                && (this.Area == other.Area)
                && (this.Friend == other.Friend)
                && (this.Foe == other.Foe)
                && (this.Result == other.Result)
                && (this.Drop == other.Drop)
                && (this.CsvFlag == other.CsvFlag)
				&& (this.FriendFormation == other.FriendFormation)
				&& (this.EnemyFormation == other.EnemyFormation)
				&& (this.MatchType == other.MatchType)
				&& (this.AirSuperityType == other.AirSuperityType);
		}


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\""
				+ $",\"{Area}\""
				+ $",\"{Friend}\""
				+ $",\"{Foe}\""
				+ $",\"{Result}\""
				+ $",\"{Drop}\""
				+ $",\"{FriendFormation}\""
				+ $",\"{EnemyFormation}\""
				+ $",\"{MatchType}\""
				+ $",\"{AirSuperityType}\"";
        }
    }
}
