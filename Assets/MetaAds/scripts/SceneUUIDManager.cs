using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace MetaAds
{
    [Serializable]
    public class SceneUUIDEntry
    {
        public string sceneName;
        public string sceneUUID;
    }

    [Serializable]
    public class SceneUUIDData
    {
        public List<SceneUUIDEntry> entries = new List<SceneUUIDEntry>();
    }

    public class SceneUUIDManager
    {
        private const string SceneDataFileName = "MetaAdsResources/SceneUUIDData";
        private const string SceneDataFilePath = "Assets/Resources/MetaAdsResources/SceneUUIDData.json";
        public static SceneUUIDManager instance;
        public static SceneUUIDManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneUUIDManager();
                }
                return instance;
            }
        }
        public string GetSceneUUID(string sceneName)
        {
            SceneUUIDData sceneData = LoadSceneUUIDData();
            SceneUUIDEntry entry = sceneData.entries.Find(e => e.sceneName == sceneName);
            if (entry != null)
            {
                return entry.sceneUUID;
            }
            else
            {
                string newUUID = Guid.NewGuid().ToString();

                entry = new SceneUUIDEntry
                {
                    sceneName = sceneName,
                    sceneUUID = newUUID
                };
                sceneData.entries.Add(entry);

                SaveSceneUUIDData(sceneData);

                return newUUID;
            }
        }

        private SceneUUIDData LoadSceneUUIDData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(SceneDataFileName);
            if (textAsset != null)
            {
                string jsonData = textAsset.text;
                return JsonUtility.FromJson<SceneUUIDData>(jsonData);
            }
            else
            {
                return new SceneUUIDData();
            }
        }

        private void SaveSceneUUIDData(SceneUUIDData sceneData)
        {
            string jsonData = JsonUtility.ToJson(sceneData);
            string directoryPath = Path.GetDirectoryName(SceneDataFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(SceneDataFilePath, jsonData);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
