using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class CreateItemLogStruct : IEquatable<CreateItemLogStruct>
    {
        /// <summary>
        /// 時刻
        /// </summary>
        [ProtoMember(1)]
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// 装備名
        /// </summary>
        [ProtoMember(2)]
        public String SlotItem { get; private set; }

        /// <summary>
        /// 装備種別
        /// </summary>
        [ProtoMember(3)]
        public String SlotType { get; private set; }

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
        /// 秘書艦
        /// </summary>
        [ProtoMember(8)]
        public String Secretary { get; private set; }

        /// <summary>
        /// 司令部レベル
        /// </summary>
        [ProtoMember(9)]
        public int Level { get; private set; }

        /// <summary>
        /// CSV Import flag
        /// </summary>
        [ProtoMember(10)]
        public bool CsvFlag { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 通常不使用 (Deserialize時に必要な実装)
        /// </remarks>
        public CreateItemLogStruct() { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateItemLogStruct(DateTime dateTime, String slotitem, String slottype, int fuel, int ammun, int steel, int baux, String secret, int level, bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.SlotItem = slotitem;
            this.SlotType = slottype;
            this.Fuel = fuel;
            this.Ammunition = ammun;
            this.Steel = steel;
            this.Bauxite = baux;
            this.Secretary = secret;
            this.Level = level;
            this.CsvFlag = isCsv;
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(CreateItemLogStruct other)
        {
            return (this.DateTime == other.DateTime)
                && (this.SlotItem == other.SlotItem)
                && (this.SlotType == other.SlotType)
                && (this.Fuel == other.Fuel)
                && (this.Ammunition == other.Ammunition)
                && (this.Steel == other.Steel)
                && (this.Bauxite == other.Bauxite)
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
            return $"\"{DateTime}\",\"{SlotItem}\",\"{SlotType}\",\"{Fuel}\",\"{Ammunition}\",\"{Steel}\",\"{Bauxite}\",\"{Secretary}\",\"{Level}\"";
        }
    }
}
