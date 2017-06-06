using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
    [ProtoContract]
    public class RankingLogStruct : IEquatable<RankingLogStruct>
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
        public Dictionary<string, RankData> Admiral { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// 通常不使用 (Deserialize時に必要な実装)
        /// </remarks>
        public RankingLogStruct()
        {
            this.Admiral = new Dictionary<string, RankData>();
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RankingLogStruct(DateTime time)
        {
            this.DateTime = time;
            this.Admiral = new Dictionary<string, RankData>();
        }


        /// <summary>
        /// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このインスタンスと比較するオブジェクト</param>
        /// <returns></returns>
        public bool Equals(RankingLogStruct other)
        {
            return ((this.DateTime == other.DateTime));
        }


        /// <summary>
        /// CSV出力用の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"\"{DateTime}\",\"{Admiral}\"";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aachievement"></param>
        public void Update(string name, uint achievement, int rank)
        {
            RankData data;
            if (!this.Admiral.TryGetValue(name, out data)) {
                data = new RankData();
                data.rank = rank;
                data.achievement = achievement;
                this.Admiral.Add(name, data);
            }
        }
    }


    [ProtoContract]
    public struct RankData
    {
        /// <summary>
        /// 生ランク値
        /// </summary>
        [ProtoMember(1)]
        public int rank { get; set; }

        /// <summary>
        /// 生戦果値
        /// </summary>
        [ProtoMember(2)]
        public uint achievement { get; set; }



        public class ConstADef
        {
            public DateTime date { get; set; }
            public int val { get; set; }
        }
        const string tableFileName = "AchieConstTable.xml";
        static List<ConstADef> table = new List<ConstADef>();
        static RankData()
        {
            bool isSuccess = false;

            try
            {
                if (System.IO.File.Exists(tableFileName))
                {
                    // ファイルが合ったら読み込む
                    var serializer = new System.Xml.Serialization.XmlSerializer(table.GetType());
                    var sr = new System.IO.StreamReader(tableFileName, new System.Text.UTF8Encoding(false));
                    table = (List<ConstADef>)serializer.Deserialize(sr);
                    sr.Close();
                    isSuccess = true;
                }
            }
            catch
            {
                isSuccess = false;
            }

            if (!isSuccess)
            {
                // 無かったら適当に作る
                table.Add(new ConstADef { date = new DateTime(2017, 2, 28, 12, 0, 0), val = 91 });
                table.Add(new ConstADef { date = new DateTime(2017, 3, 17, 12, 0, 0), val = 25 });
                table.Add(new ConstADef { date = new DateTime(2017, 4, 5, 12, 0, 0), val = 63 });
                table.Add(new ConstADef { date = new DateTime(2099, 12, 31, 12, 0, 0), val = 26 });

                var serializer = new System.Xml.Serialization.XmlSerializer(table.GetType());
                var sw = new System.IO.StreamWriter(tableFileName, false, new System.Text.UTF8Encoding(false));
                serializer.Serialize(sw, table);
                sw.Close();
            }
        }


        public int 勲章数(DateTime date)
        {
            int rank = this.rank;
            uint apiVal = this.achievement;
            /// <remarks>
            ///     甲勲章 = api_itslcqtmrxtf / 係数 -157
            /// </remarks>
            return (int)(apiVal / 勲章係数(rank)) - 157;
        }


        public int 戦果(DateTime date)
        {
            int rank = this.rank;
            uint apiVal = this.achievement;
            /// <remarks>
            ///     戦果 = api_wuhnhojjxmke / (係数*a) - 91
            /// </remarks>

            return (int)(apiVal / (戦果係数(rank) * getConstA(date))) - 91;
        }


        private int 勲章係数(int rank)
        {
            int val = 0;
            switch (rank % 13)
            {
                case 0: val = 10784; break;
                case 1: val = 3054; break;
                case 2: val = 3009; break;
                case 3: val = 6914; break;
                case 4: val = 6422; break;
                case 5: val = 6585; break;
                case 6: val = 5632; break;
                case 7: val = 6421; break;
                case 8: val = 7548; break;
                case 9: val = 6472; break;
                case 10: val = 6765; break;
                case 11: val = 7522; break;
                case 12: val = 8439; break;
            }
            return val;
        }


        private int 戦果係数(int rank)
        {
            int[] table = {
                8931,
                1201,
                1156,
                5061,
                4569,
                4732,
                3779,
                4568,
                5695,
                4619,
                4912,
                5669,
                6586,
            };
            return table[rank % 13];
        }


        private int getConstA(DateTime date)
        {
            int a = 1; // a はメンテごとに変わることが多い

            foreach (var pair in table)
            {
                if (date < pair.date)
                {
                    a = pair.val;
                    break;
                }
            }
            /*
            if (date < new DateTime(2017, 2, 28, 15, 0, 0)) { a = 91; }
            else if (date < new DateTime(2017, 3, 17, 15, 0, 0)) { a = 25; }
            else if (date < new DateTime(2017, 4, 5, 15, 0, 0)) { a = 63; }
            else { a = 26; }
            */
            return a;
        }

        public static int EditConstA
        {
            get { return table.Last().val; }
            set { table.Last().val = value; }
        }
    }
}
