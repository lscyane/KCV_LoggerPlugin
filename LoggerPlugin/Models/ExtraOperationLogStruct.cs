using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class ExtraOperationLogStruct : IEquatable<ExtraOperationLogStruct>
    {
        /// <summary>
        /// 時刻
        /// </summary>
        [ProtoMember(1)]
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// EO攻略状況
        /// </summary>
        [ProtoMember(2)]
        public Dictionary<int, int> EOClearFlag { set; get; }

        /// <summary>
        /// CSVフラグ
        /// </summary>
        [ProtoMember(3)]
        public bool CsvFlag { get; private set; }

        /// <summary>
        /// EO攻略日
        /// </summary>
        [ProtoMember(4)]
        public Dictionary<int, DateTime> EOClearDate { set; get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 通常不使用 (Deserialize時に必要な実装)
        /// </remarks>
        public ExtraOperationLogStruct()
        {
            this.EOClearFlag = new Dictionary<int, int>();
            this.EOClearDate = new Dictionary<int, DateTime>();
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExtraOperationLogStruct( DateTime dateTime,  bool isCsv = false)
        {
            this.DateTime = dateTime;
            this.EOClearFlag = new Dictionary<int, int>();
            this.EOClearDate = new Dictionary<int, DateTime>();
            this.CsvFlag = isCsv;
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(ExtraOperationLogStruct other)
        {
            return ((this.DateTime == other.DateTime)
                && (this.CsvFlag == other.CsvFlag));
        }


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\"";
        }


        /// <summary>
        /// 攻略状況を更新します
        /// </summary>
        /// <param name="map_id"></param>
        /// <param name="value"></param>
        public void Update(int map_id, int cleared)
        {
            switch (map_id)
            {
                case 15:
                case 16:
                case 25:
                case 35:
                case 45:
                case 55:
                case 65:
                    if (this.EOClearFlag.ContainsKey(map_id))
                    {
                        // Update
                        if (this.EOClearFlag[map_id] != cleared)
                        {
                            this.EOClearFlag[map_id] = cleared;
                            this.EOClearDate[map_id] = DateTime.Now;
                        }

                        // 補正：1-5が未クリアなら1-6も未クリアにする (月跨ぎで再ログインせずに海域画面を開かれた場合の対策)
                        if ((map_id == 15)
                         && (cleared == 0)
                         && (this.EOClearFlag.ContainsKey(16))
                        ) {
                            this.EOClearFlag[16] = cleared;
                        }
                    }
                    else
                    {
                        // 初回Update、キーの追加
                        this.EOClearFlag.Add(map_id, cleared);
                        this.EOClearDate.Add(map_id, DateTime.MinValue);
                        if (cleared != 0)
                        {
                            this.EOClearDate[map_id] = DateTime.Now;
                        }
                    }
                    break;
            }
        }


        /// <summary>
        /// 戦果の合計値を返します
        /// </summary>
        /// <returns></returns>
        public int GetAachievementSum()
        {
            int retval = 0;
            foreach(var map in this.EOClearFlag)
            {
                if (map.Value != 0)
                {
                    switch (map.Key)
                    {
                        case 15: retval += 75; break;
                        case 16: retval += 75; break;
                        case 25: retval += 100; break;
                        case 35: retval += 150; break;
                        case 45: retval += 180; break;
                        case 55: retval += 200; break;
                        case 65: retval += 250; break;
                    }
                }
            }
            return retval;
        }


        /// <summary>
        /// 当日攻略EOの戦果合計を返します
        /// </summary>
        /// <returns></returns>
        public int GetAachievementSumToday()
        {
            int retval = 0;
            DateTime now = DateTime.Now;
            foreach (var map in this.EOClearFlag)
            {
                DateTime baseTime = new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
                if (DateTime.Now.Hour < 22)
                {
                    baseTime = baseTime.AddDays(-1);
                }
                if ((map.Value != 0)
                 && (this.EOClearDate.ContainsKey(map.Key))
                 && (this.EOClearDate[map.Key] > baseTime )
                ) { 
                    switch (map.Key)
                    {
                        case 15: retval += 75; break;
                        case 16: retval += 75; break;
                        case 25: retval += 100; break;
                        case 35: retval += 150; break;
                        case 45: retval += 180; break;
                        case 55: retval += 200; break;
                        case 65: retval += 250; break;
                    }
                }
            }
            return retval;
        }
    }
}
