namespace MetaAds
{
        [System.Serializable]
        public class DataToGenerateStatisticToken
        {
            public int user_id;
            public string wallet, scene_coords, metaverse;
            public AdSpots ad_spots;
            public bool web3;
            public float width, height, length, shift_width, shift_height, shift_length;
            public DataToGenerateStatisticToken(int user_id, string wallet, string scene_coords, float width, float height, float length, float shift_width, float shift_height, float shift_length, bool web3, string metaverse, AdSpots ad_spots)
            {
                this.user_id = user_id;
                this.wallet = wallet;
                this.scene_coords = scene_coords;
                this.width = width;
                this.height = height;
                this.length = length;
                this.shift_width = shift_width;
                this.shift_height = shift_height;
                this.shift_length = shift_length;
                this.web3 = web3;
                this.metaverse = metaverse;
                this.ad_spots = ad_spots;
            }

        }

}








