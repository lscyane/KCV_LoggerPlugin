namespace KCVLoggerPlugin.Models.Raw
{

    public class datalist
    {
        public int api_count { get; set; }
        public int api_disp_page { get; set; }
        public member_datalist[] api_list { get; set; }
        public int api_page_count { get; set; }
    }

    public class member_datalist
    {
        /// <summary> 順位 </summary>
        public int api_mxltvkpyuklh { get; set; }
        /// <summary> 提督名 </summary>
        public string api_mtjmdcwtvhdr { get; set; }
        public int api_pbgkfylkbjuy { get; set; }
        /// <summary> 階級 </summary>
        public int api_pcumlrymlujh { get; set; }
        /// <summary> コメント </summary>
        public string api_itbrdpdbkynm { get; set; }
        /// <summary> 甲種勲章 </summary>
        public uint api_itslcqtmrxtf { get; set; }
        /// <summary> 生戦果値 </summary>
        public uint api_wuhnhojjxmke { get; set; }
    }

}
