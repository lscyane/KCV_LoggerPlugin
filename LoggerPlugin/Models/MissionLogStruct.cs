using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class MissionLogStruct : IEquatable<MissionLogStruct>
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
        /// 遠征名
        /// </summary>
        [ProtoMember(3)]
        public String MissionName { get; private set; }

        /// <summary>
        /// 艦隊
        /// </summary>
        [ProtoMember(4)]
        public String Fleet { get; private set; }

        /// <summary>
        /// 結果
        /// </summary>
        [ProtoMember(5)]
        public int Result { get; private set; }

        /// <summary>
        /// 燃料
        /// </summary>
        [ProtoMember(6)]
        public int Fuel { get; private set; }

        /// <summary>
        /// 弾薬
        /// </summary>
        [ProtoMember(7)]
        public int Ammunition { get; private set; }

        /// <summary>
        /// 鋼材
        /// </summary>
        [ProtoMember(8)]
        public int Steel { get; private set; }

        /// <summary>
        /// ボーキサイト
        /// </summary>
        [ProtoMember(9)]
        public int Bauxite { get; private set; }

        /// <summary>
        /// 獲得アイテム
        /// </summary>
        [ProtoMember(10)]
        public String Item { get; private set; }

        /// <summary>
        /// CSV Import flag
        /// </summary>
        [ProtoMember(11)]
        public bool CsvFlag { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 通常不使用 (Deserialize時に必要な実装)
        /// </remarks>
        public MissionLogStruct() { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MissionLogStruct(DateTime dateTime, String area, String missionName, int fuel, int ammun, int steel, int baux, int result, String fleet, String item, bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.Area = area;
            this.MissionName = missionName;
            this.Fleet = fleet;
            this.Result = result;
            this.Fuel = fuel;
            this.Ammunition = ammun;
            this.Steel = steel;
            this.Bauxite = baux;
            this.Item = item;
            this.CsvFlag = isCsv;
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(MissionLogStruct other)
        {
            return (this.DateTime == other.DateTime)
                && (this.Area == other.Area)
                && (this.MissionName == other.MissionName)
                && (this.Fleet == other.Fleet)
                && (this.Result == other.Result)
                && (this.Fuel == other.Fuel)
                && (this.Ammunition == other.Ammunition)
                && (this.Steel == other.Steel)
                && (this.Bauxite == other.Bauxite)
                && (this.Item == other.Item)
                && (this.CsvFlag == other.CsvFlag);
        }


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\""
				+ $",\"{Area}\""
				+ $",\"{MissionName}\""
				+ $",\"{Fuel}\""
				+ $",\"{Ammunition}\""
				+ $",\"{Steel}\""
				+ $",\"{Bauxite}\""
				+ $",\"{Result}\"" 
				+ $",\"{Fleet}\""
				+ $",\"{Item}\"";
        }
    }
}
