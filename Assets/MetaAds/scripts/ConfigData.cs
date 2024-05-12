using System;
namespace MetaAds
{
    [Serializable]
    public class ConfigData
    {
        public int userId;
        public string userSignature;
        public string baseUrl;
        public string streamUrl;
        public string statisticsUrl;
        public string defaultImageUrl;
        public string metaverse;
        public string pixelUrl;
        public float intervalBetweenRequests;
        public float intervalForMetrics;
        public float intervalForSendMetrics;
        public float intervalForPixelMetrics;
        public float intervalForUpdatePixelToken;
    }
}
