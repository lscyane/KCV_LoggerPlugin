using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KCVLoggerPlugin.Models
{
	[ProtoContract]
	public class MaterialLogStruct : IEquatable<MaterialLogStruct>
	{
		/// <summary>
		/// 時刻
		/// </summary>
		[ProtoMember(1)]
		public DateTime DateTime { get; private set; }

		/// <summary>
		/// 燃料
		/// </summary>
		[ProtoMember(2)]
		public int Fuel { get; private set; }

		/// <summary>
		/// 弾薬
		/// </summary>
		[ProtoMember(3)]
		public int Ammunition { get; private set; }

		/// <summary>
		/// 鋼材
		/// </summary>
		[ProtoMember(4)]
		public int Steel { get; private set; }

		/// <summary>
		/// ボーキサイト
		/// </summary>
		[ProtoMember(5)]
		public int Bauxite { get; private set; }

		/// <summary>
		/// 高速修復材
		/// </summary>
		[ProtoMember(6)]
		public int RepairTool { get; private set; }

		/// <summary>
		/// 高速建造剤
		/// </summary>
		[ProtoMember(7)]
		public int InstantBuildTool { get; private set; }

		/// <summary>
		/// 開発資材
		/// </summary>
		[ProtoMember(8)]
		public int DevelopmentTool { get; private set; }

		/// <summary>
		/// 改修資材
		/// </summary>
		[ProtoMember(9)]
		public Int32 ImprovementTool { get; private set; }

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
		public MaterialLogStruct() { }


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MaterialLogStruct(DateTime dateTime, int fuel, int ammun, int steel, int baux, int repair, int instant, int develop, int improve, bool isCsv = false)
		{
			this.DateTime = dateTime;
			this.Fuel = fuel;
			this.Ammunition = ammun;
			this.Steel = steel;
			this.Bauxite = baux;
			this.RepairTool = repair;
			this.InstantBuildTool = instant;
			this.DevelopmentTool = develop;
			this.ImprovementTool = improve;
			this.CsvFlag = isCsv;
		}


		/// <summary>
		/// このインスタンスと、指定したオブジェクトの値が同一かどうかを判断します。
		/// </summary>
		/// <param name="other">このインスタンスと比較するオブジェクト</param>
		/// <returns></returns>
		public bool Equals(MaterialLogStruct other)
		{
			return 
				//	(this.DateTime == other.DateTime)
				   (this.Fuel == other.Fuel)
				&& (this.Ammunition == other.Ammunition)
				&& (this.Steel == other.Steel)
				&& (this.Bauxite == other.Bauxite)
				&& (this.RepairTool == other.RepairTool)
				&& (this.InstantBuildTool == other.InstantBuildTool)
				&& (this.DevelopmentTool == other.DevelopmentTool)
				&& (this.ImprovementTool == other.ImprovementTool);
			//  && (this.CsvFlag == other.CsvFlag);
		}


		/// <summary>
		/// CSV出力用の文字列を返します。
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			String tost = "";
			tost += $"\"{DateTime}\"";
			tost += $",\"{Fuel}\"";
			tost += $",\"{Ammunition}\"";
			tost += $",\"{Steel}\"";
			tost += $",\"{Bauxite}\"";
			tost += $",\"{RepairTool}\"";
			tost += $",\"{InstantBuildTool}\"";
			tost += $",\"{DevelopmentTool}\"";
			tost += $",\"{ImprovementTool}\"";
			return tost;
		}
	}
}
