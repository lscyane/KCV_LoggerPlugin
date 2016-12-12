using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class CreateShipLogStruct : IEquatable<CreateShipLogStruct>
    {
        /// <summary>
        /// 時刻
        /// </summary>
        [ProtoMember(1)]
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// 艦娘
        /// </summary>
        [ProtoMember(2)]
        public String ShipName { get; private set; }

        /// <summary>
        /// 種別
        /// </summary>
        [ProtoMember(3)]
        public String ShipType { get; private set; }

        /// <summary>
        /// 投資燃料
        /// </summary>
        [ProtoMember(4)]
        public int Fuel { get; private set; }

        /// <summary>
        /// 投資弾薬
        /// </summary>
        [ProtoMember(5)]
        public int Ammunition { get; private set; }

        /// <summary>
        /// 投資鋼材
        /// </summary>
        [ProtoMember(6)]
        public int Steel { get; private set; }

        /// <summary>
        /// 投資ボーキサイト
        /// </summary>
        [ProtoMember(7)]
        public int Bauxite { get; private set; }

        /// <summary>
        /// 開発資材
        /// </summary>
        [ProtoMember(8)]
        public int DevTool { get; private set; }

        /// <summary>
        /// 秘書艦
        /// </summary>
        [ProtoMember(9)]
        public String Secretary { get; private set; }

        /// <summary>
        /// 司令部レベル
        /// </summary>
        [ProtoMember(10)]
        public int Level { get; private set; }

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
        public CreateShipLogStruct() { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateShipLogStruct(DateTime dateTime, String shipneme, String shiptype, Int32 fuel, Int32 ammun, Int32 steel, Int32 baux, Int32 devtool, String secret, Int32 level, bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.ShipName = shipneme;
            this.ShipType = shiptype;
            this.Fuel = fuel;
            this.Ammunition = ammun;
            this.Steel = steel;
            this.Bauxite = baux;
            this.DevTool = devtool;
            this.Secretary = secret;
            this.Level = level;
            this.CsvFlag = isCsv;
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(CreateShipLogStruct other)
        {
            return (this.DateTime == other.DateTime)
                && (this.ShipName == other.ShipName)
                && (this.ShipType == other.ShipType)
                && (this.Fuel == other.Fuel)
                && (this.Ammunition == other.Ammunition)
                && (this.Steel == other.Steel)
                && (this.Bauxite == other.Bauxite)
                && (this.DevTool == other.DevTool)
                && (this.Secretary == other.Secretary)
                && (this.Level == other.Level)
                && (this.CsvFlag == other.CsvFlag);
        }


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\",\"{ShipName}\",\"{ShipType}\",\"{Fuel}\",\"{Ammunition}\",\"{Steel}\",\"{Bauxite}\",\"{DevTool}\",\"{Secretary}\",\"{Level}\"";
        }
    }
}
