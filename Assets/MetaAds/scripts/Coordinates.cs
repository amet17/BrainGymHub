using UnityEngine;
using System.Globalization;

namespace MetaAds
{
        [System.Serializable]
        public class Coordinates
        {
            public float x;
            public float y;
            public float z;
            CultureInfo culture = CultureInfo.InvariantCulture;
            public Coordinates(Vector3 position)
            {
                setPosition(position);
            }
            public void setPosition(Vector3 pos) {
                this.x = Mathf.Round(pos.x * 1000f) / 1000f;
                this.y = Mathf.Round(pos.y * 1000f) / 1000f; 
                this.z = Mathf.Round(pos.z * 1000f) / 1000f;
            }
        public string getJson()
        {
            string xFormatted = x.ToString("F3", culture).TrimEnd('0').TrimEnd('.');
            string yFormatted = y.ToString("F3", culture).TrimEnd('0').TrimEnd('.');
            string zFormatted = z.ToString("F3", culture).TrimEnd('0').TrimEnd('.');

            string json = string.Format("{{\"x\": {0}, \"y\": {1}, \"z\": {2}}}", xFormatted, yFormatted, zFormatted);      
            return json;
        }


    }
}








