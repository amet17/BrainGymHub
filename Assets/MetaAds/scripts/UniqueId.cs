using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MetaAds
{
    [ExecuteInEditMode]
    public class UniqueId : MonoBehaviour
    {
        [SerializeField]
        private string uniqueId;

        public string getUniqueId()
        {
            return uniqueId;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
            if (string.IsNullOrEmpty(uniqueId)||isDuplicateUniqueId()) {
                uniqueId = Guid.NewGuid().ToString();
            }
        }
        public bool IsPartOfPrefabAsset(GameObject gameObject)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            return prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab;
        }

        public bool isDuplicateUniqueId() {
            UniqueId[] uniqueIdObjects = FindObjectsOfType<UniqueId>();
            foreach (UniqueId uniqueIdObject in uniqueIdObjects)
            {
                if (uniqueIdObject.gameObject.GetInstanceID() != this.gameObject.GetInstanceID() && uniqueIdObject.uniqueId.Equals(uniqueId))
                {
                    return true;
                }
            }
                return false;
        }
#endif
    }
}
