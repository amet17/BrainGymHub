using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MetaAds
{
    public class StatisticsCollectionArea : MonoBehaviour
    {
        Bounds bounds;
        private void Awake()
        {
            bounds = GetComponent<BoxCollider>().bounds;
        }
        public Bounds getBounds()
        {
            return bounds;
        }
    }
}
