using System;
namespace MetaAds
{
    [Serializable]
    public class ServerResponseData
    {
        public int session_id;
        public int creative_id;
        public string url;
        public bool is_image;
        public bool is_default;
        public string to_time;
        public string external_link;
        public string msg;
        public bool is_stream;
        public string token;
        public int ad_spot_id;
        public bool continue_registration;
    }
}
