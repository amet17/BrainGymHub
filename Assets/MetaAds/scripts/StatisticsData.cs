using System;
namespace MetaAds
{
    [Serializable]
    public class StatisticsData
    {
        public bool focused;
        public string user;
        public bool web3;
        public string uuid;
        public int reacted;
        public int session_id;
        public int creative_id;
        public bool clicked;
       // public string public_key;

        public StatisticsData(string pin)
        {
            user = UserStatisticsManager.Instance.GetClientId();
            this.uuid = pin;
            setDefoultValue();
        }

        public void setDefoultValue()
        {
            focused = false;
            web3 = false;
            reacted = 0;
            session_id = 0;
            creative_id = 0;
            clicked = false;
            //public_key = null;
        }
    }
}
