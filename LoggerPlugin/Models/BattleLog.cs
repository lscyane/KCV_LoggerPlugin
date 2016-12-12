using Livet;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCVLoggerPlugin.Models
{
    /// <remarks>
    /// SettingViewModelからも参照する共有インスタンスなのでシングルトンにする
    /// (MVVMにおいて共有インスタンスはシングルトン化するのが一般的だと思います)
    /// </remarks>
    public class BattleLog : LogBase<BattleLogStruct>
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */


        #region シングルトン実装
        /// <summary>
        /// SingletonSample クラスのシングルトン インスタンスを取得します。
        /// このプロパティは、基底クラスの実装を隠します。
        /// instance フィールドは基底クラス型なので、本クラス内での参照ではキャストされたこのプロパティを利用してください。
        /// </summary>
        /// <value>
        /// SingletonSample クラスのシングルトン インスタンス。
        /// 生成されていない場合は、インスタンスを生成して返します。
        /// </value>
        public new static BattleLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockForSingleton)
                    {
                        if (instance == null)
                        {
                            // ダブルチェックロッキングとすることで、パフォーマンスの確保と
                            // インスタンス生成が複数発生しないことを両立する。
                            instance = new BattleLog();
                        }
                    }
                }
                // instance フィールドに格納されている型は基底クラスのため、
                // 派生クラスにキャストして返す。
                return (BattleLog)instance;
            }
        }

        /// <summary>
        /// SingletonSample クラスのシングルトン インスタンスを初期化します。 
        /// </summary>
        protected BattleLog()
        {
            // NOP

            // protected とすることで、外部からのインスタンス生成を防いでいるので、
            // 何もないからといってコンストラクタを削除してはならない。
        }
        #endregion 


        /// <summary>
        /// CSV形式のファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override void ImportCsv(String path)
        {
            ObservableCollection<BattleLogStruct> data = new ObservableCollection<BattleLogStruct>();

			var parser = new CsvHelper.CsvReader(new StreamReader(path, System.Text.Encoding.UTF8));
			parser.Configuration.Encoding = System.Text.Encoding.UTF8;
			parser.Configuration.AllowComments = true;
			parser.Configuration.Comment = '#';
			parser.Configuration.HasHeaderRecord = false;

			while (parser.Read())
			{
				List<String> mediumClass = new List<string>();
				for (var i = 0; i < parser.CurrentRecord.Length; ++i)
				{
					mediumClass.Add(parser.CurrentRecord.ElementAt(i));
				}

				// データ格納
				data.Add(new BattleLogStruct(
                    DateTime.Parse(mediumClass[0]),
                    mediumClass[1],
                    mediumClass[2],
                    mediumClass[3],
                    mediumClass[4],
                    mediumClass[5],
					(mediumClass.Count > 6 ? int.Parse(mediumClass[6]) : -1),
					(mediumClass.Count > 7 ? int.Parse(mediumClass[7]) : -1),
					(mediumClass.Count > 8 ? int.Parse(mediumClass[8]) : -1),
					(mediumClass.Count > 9 ? int.Parse(mediumClass[9]) : -1),
					true
                    ));
            }
			parser.Dispose();
			this.SetHistory(data);
		}


        /// <summary>
        /// CSV形式でファイル出力する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public override void ExportCsv(String path)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            foreach (BattleLogStruct bl in this.History)
            {
                sw.WriteLine(bl.ToString());
            }
            sw.Close();
        }
    }
}