using System.Collections.Generic;
using UnityEngine;

namespace MetaAds
{
   
        [System.Serializable]
        public class AdSpots
        {
            public List<string> identifiers;
            public List<Coordinates> coords;
            public AdSpots(ScreenController[] arrScreenControllers)
            {
                arrScreenControllers = GameObject.FindObjectsOfType<ScreenController>();
                identifiers = new List<string>();
                coords = new List<Coordinates>();
                foreach (ScreenController obj in arrScreenControllers)
                {
                    identifiers.Add(obj.getDisplayUUID());
                    coords.Add(new Coordinates(obj.transform.position));
                }
            }
            public string GetAdSpotsJson()
            {

                string json = JsonUtility.ToJson(this);
                Debug.Log(json);
                return json;

            }
        }



}








