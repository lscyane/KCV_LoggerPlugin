namespace KCVLoggerPlugin.Models.Raw
{
    public class kcsapi_bounus
    {
        public member_bounus[] api_bounus { get; set; }
    }

    public class member_bounus
    {
        public int api_type { get; set; }
        public int api_count { get; set; }
        public member_item api_item { get; set; }
    }

    public class member_item
    {
        public int api_id { get; set; }
        public string api_name { get; set; }
    }
}
