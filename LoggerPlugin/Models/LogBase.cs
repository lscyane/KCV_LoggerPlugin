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
    public interface ILogBase
    {
        Task LoadAsync(string filePath, Action onSuccess);
        Task SaveAsync(string filePath, Action onSuccess);
        void ImportCsv(String path);
        void ExportCsv(String path);
    }

    /// <remarks>
    /// SettingViewModelからも参照する共有インスタンスなのでシングルトンにする
    /// (MVVMにおいて共有インスタンスはシングルトン化するのが一般的だと思います)
    /// </remarks>
    public class LogBase<T> : NotificationObject, ILogBase
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        /// <summary>
        /// ログデータのインスタンス
        /// </summary>
        public ObservableCollection<T> History { get; private set; }

        #region HistoryWriting変更通知プロパティ
        private bool _HistoryWriting = false;
        public bool HistoryWriting
        {
            get
            {
                return this._HistoryWriting;
            }
            set
            {
                if (this._HistoryWriting == value)
                    return;
                this._HistoryWriting = value;
                RaisePropertyChanged(() => this.HistoryWriting);    // DBリフレッシュのトリガ
            }
        }
        #endregion


        #region シングルトン実装
        /// <summary>
        /// SingletonSampleBase クラスのシングルトン インスタンスを初期化します。 
        /// </summary>
        /// <remarks>
        /// シングルトンパターンを採用するため、コンストラクタは公開しません。
        /// 継承を考え、protected としています。
        /// </remarks>
        protected LogBase()
        {
            this.BlankNew();

            // protected とすることで、外部からのインスタンス生成を防いでいるので、
            // 何もないからといってコンストラクタを削除してはならない。
        }


        /// <summary>
        /// SingletonSampleBase クラスのシングルトン インスタンスを保持します。
        /// </summary>
        protected static LogBase<T> instance = null;


        /// <summary>
        /// SingletonSampleBase クラスのシングルトン管理用ロックオブジェクトを保持します。
        /// </summary>
        protected static Object lockForSingleton = new object();


        /// <summary>
        /// SingletonSampleBase クラスのシングルトン インスタンスを取得します。
        /// </summary>
        /// <value>
        /// SingletonSampleBase クラスのシングルトン インスタンス。
        /// 生成されていない場合は、インスタンスを生成して返します。
        /// </value>
        public static LogBase<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockForSingleton)
                    {
                        // ダブルチェックロッキングとすることで、パフォーマンスの確保と
                        // インスタンス生成が複数発生しないことを両立する。
                        if (instance == null)
                        {
                            instance = new LogBase<T>();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion


        /// <summary>
        /// ログデータの初期化
        /// </summary>
        public void BlankNew()
        {
            this.HistoryWriting = true;
            this.History = new ObservableCollection<T>();
            this.HistoryWriting = false;
        }


        /// <summary>
        /// ログデータのセット
        /// </summary>
        /// <param name="history"></param>
        public void SetHistory(ObservableCollection<T> history)
        {
            this.HistoryWriting = true;
            this.History = history;
            this.HistoryWriting = false;
        }


        /// <summary>
        /// ロギング
        /// </summary>
        /// <param name="date">日時</param>
        /// <param name="area">海域</param>
        /// <param name="fleet">艦隊</param>
        /// <param name="eFleet">敵艦隊</param>
        /// <param name="rank">戦闘結果</param>
        /// <param name="drop">ドロップ</param>
        public void HistoryAdd(T log)
        {
            this.HistoryWriting = true;
            this.History.Add(log);
            this.HistoryWriting = false;
        }


        /// <summary>
        /// ProtoBufでデシリアライズする
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public Task LoadAsync(string filePath, Action onSuccess)
        {
            // タスクコンテキストの拠り所を生成
            var tcs = new TaskCompletionSource<bool>();

			if (File.Exists(filePath))
            {
                try
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        //var history = await Task.Run(() => Serializer.Deserialize<ObservableCollection<T>>(stream));
                        var history = Serializer.Deserialize<ObservableCollection<T>>(stream);
                        this.SetHistory(history);
                    }
					tcs.SetResult(true);
					onSuccess?.Invoke();
                }
                catch (ProtoException ex)
                {
                    if (this.History == null)
                    {
                        this.BlankNew();
                    }
                    System.Diagnostics.Debug.WriteLine(ex);
					tcs.SetException(new Exception("ログデータの読み込みに失敗しました。データが破損している可能性があります。"));
                }
                catch (IOException ex)
                {
                    if (this.History == null)
                    {
                        this.BlankNew();
                    }
                    System.Diagnostics.Debug.WriteLine(ex);
					tcs.SetException(new Exception("ログデータの読み込みに失敗しました。必要なアクセス権限がない可能性があります。"));
                }
            }
            else
            {
                if (this.History == null)
                    this.BlankNew();
            }

			return tcs.Task;
        }


        /// <summary>
        /// ProtoBufでシリアライズする
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public Task SaveAsync(string filePath, Action onSuccess)
        {
            // タスクコンテキストの拠り所を生成
            var tcs = new TaskCompletionSource<bool>();

			try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    Serializer.Serialize(stream, this.History);
                }
				tcs.SetResult(true);
				onSuccess?.Invoke();
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
				tcs.SetException(new Exception("ログデータの保存に失敗しました。必要なアクセス権限がない可能性があります。"));
            }

            return tcs.Task;
        }


        /// <summary>
        /// CSV形式のファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual void ImportCsv(String path)
        {
        }


        /// <summary>
        /// CSV形式でファイル出力する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public virtual void ExportCsv(String path)
        {
        }


    }
}
