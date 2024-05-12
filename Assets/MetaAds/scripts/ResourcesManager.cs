using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace MetaAds {
   class ResourcesManager
    {
        private static ResourcesManager instance;
        private const string PATH_TO_ENVIRONMENT = "MetaAdsResources/Environment";
        private const string PATH_TO_CONFIG = "MetaAdsResources/MetaAdsConfig";
        private const string imageMaterialName = "MetaAdsImageMaterial";
        private const string videoMaterialName = "MetaAdsVideoContentMaterial";
        private string currentMaterialsPath = "MetaAdsResources/Materials/Standard/";
        private ConfigData configData;
        public Material materialForVideoContent {
            get;
            set;
        }
        public Material materialForImage {
            get;
            set;
        }
        public static ResourcesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourcesManager();
                    instance.DefineRenderPipelineType();
                }
                return instance;
            }
        }
        public ConfigData GetConfigData() {
            if (configData == null)
            {
                TextAsset jsonFile = Resources.Load<TextAsset>(PATH_TO_CONFIG);
                if (jsonFile != null)
                {
                    configData = JsonUtility.FromJson<ConfigData>(jsonFile.ToString());
                }
            }
            return configData;
        }

        public void DefineRenderPipelineType()
        {
            materialForImage = Resources.Load(currentMaterialsPath + imageMaterialName) as Material;
            materialForVideoContent = Resources.Load(currentMaterialsPath + videoMaterialName) as Material;
        }

    }
}
