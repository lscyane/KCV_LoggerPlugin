using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class AachievementLogStruct : IEquatable<AachievementLogStruct>
    {
		/// <summary>
		/// 時刻
		/// </summary>
		[ProtoMember(1)]
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// 提督経験値
        /// </summary>
        [ProtoMember(2)]
        public int AdmiralExp { get; private set; }

        /// <summary>
        /// CSV Import flag
        /// </summary>
        [ProtoMember(3)]
        public bool CsvFlag { get; private set; }


		/// <summary>
		/// 経験値増分(計算表示用でProtoMemberには含めない)
		/// </summary>
		public int Incremental { get; set; }


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>
		/// 通常不使用 (Deserialize時に必要な実装)
		/// </remarks>
		public AachievementLogStruct() { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AachievementLogStruct( DateTime dateTime, int admiralExp, bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.AdmiralExp = admiralExp;
            this.CsvFlag = isCsv;
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(AachievementLogStruct other)
        {
            return ((this.DateTime == other.DateTime)
                && (this.AdmiralExp == other.AdmiralExp)
                && (this.CsvFlag == other.CsvFlag));
        }


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\",\"{AdmiralExp}\"";
        }
    }
}
