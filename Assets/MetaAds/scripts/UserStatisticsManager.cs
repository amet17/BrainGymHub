using System.Collections;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using UnityEngine.SceneManagement;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;

namespace MetaAds
{
    public partial class UserStatisticsManager : MonoBehaviour
    {
        private static UserStatisticsManager instance;
        private const string PATH_GET_AUTHORIZATION_TOKEN_USER_STATISTICS= "/tornado/pixel/get-token";
        private const string PATH_SEND_PIXEL = "/hb/heart-beat-pixel";
        private const string CLIENT_UUID_PREFS = "client_uuid";
        private string authorizationToken;
        private GameObject statisticsMainCharacter;
        Bounds sceneBounds;
        AdSpots adSpots;
        Coordinates currentCoordinates;
        public string clientId;
        private bool isCoroutineSendStatisticsRunning;
        private bool isCoroutineRequestAuthorizationTokenRunning;
        private StatisticsCollectionArea statisticsCollectionArea;

        public static UserStatisticsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UserStatisticsManager>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(UserStatisticsManager).Name);
                        instance = singletonObject.AddComponent<UserStatisticsManager>();
                        instance.statisticsCollectionArea = FindObjectOfType<StatisticsCollectionArea>();                   
                        instance.clientId = instance.GetClientId();
                        DontDestroyOnLoad(singletonObject);

                    }
                }
                return instance;
            }
        }
        public void UpdateStatisticsCollectionArea()
        {
            statisticsCollectionArea = FindObjectOfType<StatisticsCollectionArea>();
        }

        public void StartCollectingStatistics()
        {
            StartCoroutine(RequestAuthorizationToken());
        }
        public void SetStatisticsMainCharacter(GameObject mainCharacter) {
            statisticsMainCharacter = mainCharacter;
        }
        public GameObject GetStatisticMainCharacter()
        {
            if (statisticsMainCharacter == null)
            {
                statisticsMainCharacter = Camera.main.gameObject;
            }
            return statisticsMainCharacter;
        }
        IEnumerator RequestAuthorizationToken()
        {
            if (isCoroutineRequestAuthorizationTokenRunning)
            {
                yield break;
            }
            isCoroutineRequestAuthorizationTokenRunning = true;
            string newToken = getToken();
            UnityWebRequest request = UnityWebRequest.Get(ResourcesManager.Instance.GetConfigData().baseUrl + PATH_GET_AUTHORIZATION_TOKEN_USER_STATISTICS);
            request.SetRequestHeader("Authorization", newToken);
            yield return request.SendWebRequest();

            if (!request.isNetworkError && !request.isHttpError)
            {
                try
                {
                    ResponseGetToken responseToken = JsonUtility.FromJson<ResponseGetToken>(request.downloadHandler.text);
                    Interlocked.Exchange(ref authorizationToken, responseToken.token);   
                    StartCoroutine(SendStatistics());
                }
                catch (WebException e)
                {
                    Debug.Log("Request TOKEN Exception - " + e.Message);
                }
            }
            isCoroutineRequestAuthorizationTokenRunning = false;

        }

        private IEnumerator SendStatistics()
        {
            if (isCoroutineSendStatisticsRunning) {
                yield break;
            }
            isCoroutineSendStatisticsRunning = true;
            while (true)
            {
    
                    if (currentCoordinates == null) {
                        currentCoordinates = new Coordinates(GetStatisticMainCharacter().transform.position);
                    }
                    if (authorizationToken != null && GetSceneBounds().Contains(GetStatisticMainCharacter().transform.position))
                    {
                        UnityWebRequest request = UnityWebRequest.Head(ResourcesManager.Instance.GetConfigData().baseUrl + PATH_SEND_PIXEL);
                        currentCoordinates.setPosition(statisticsMainCharacter.transform.position);
                        string metricsJson = currentCoordinates.getJson();//JsonUtility.ToJson(currentCoordinates);
                        string metricsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(metricsJson));
                       // Debug.Log("METRICS_PIXEL= " + metricsJson);
                        request.SetRequestHeader("Authorization", authorizationToken);
                        request.SetRequestHeader("Metrics", metricsBase64);

                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            StartCoroutine(RequestAuthorizationToken());
                            /* Debug.Log("E R R O R= " + request.error);*/
                        }
                        else
                        {
                            /* Debug.Log("Metrics sent SUCCESFULLY! Status= " + request.responseCode);*/
                            yield return new WaitForSeconds(ResourcesManager.Instance.GetConfigData().intervalForPixelMetrics / 1000);
                        }
                    }
                    else
                    {
                        //Debug.Log("Token=Null or Bounds dont contains character");
                        yield return new WaitForSeconds(ResourcesManager.Instance.GetConfigData().intervalForPixelMetrics / 1000);
                    }
            }
        }

        Bounds GetSceneBounds()
        {
            if (!SceneManager.GetActiveScene().isLoaded)
            {
                return new Bounds();
            }
            if (statisticsCollectionArea != null)
            {
                sceneBounds = statisticsCollectionArea.getBounds();
            }
            else
            {
                Renderer[] allRenderer = FindObjectsOfType<Renderer>();
                sceneBounds = allRenderer[0].bounds;
                foreach (Renderer renderer in allRenderer)
                {
                    sceneBounds.Encapsulate(renderer.bounds);
                }
                
            }
           // Debug.Log("Bounds size=" + sceneBounds.size);
            return sceneBounds;
        }
        string getToken(int id, string wallet, string scene_coords, float width, float height, float length, float shift_width, float shift_height, float shift_length, bool web3, string metaverse, AdSpots adSpots, string secretKey) // get a token having the following parameters 
        {
            DataToGenerateStatisticToken dataToGenerateToken = new DataToGenerateStatisticToken(id, wallet, scene_coords, width, height, length, shift_width, shift_height, shift_length, web3, metaverse, adSpots);
            string payload = JsonUtility.ToJson(dataToGenerateToken);
            string token = JWT.JsonWebToken.Encode(payload, Encoding.UTF8.GetBytes(secretKey), JWT.JwtHashAlgorithm.HS256);
            //Debug.Log("Token2=" + token);
            return token;
        }
        string getToken()
        {
            GetSceneBounds();
/*            Debug.Log("BoundsSize = " + sceneBounds.size.ToString());
            Debug.Log("BoundsMin = " + sceneBounds.min.ToString());
            Debug.Log("BoundsMax = " + sceneBounds.max.ToString());*/
            adSpots = new AdSpots(FindObjectsOfType<ScreenController>());
            return getToken(ResourcesManager.Instance.GetConfigData().userId, clientId, SceneUUIDManager.Instance.GetSceneUUID(SceneManager.GetActiveScene().name), Mathf.Round(sceneBounds.max.x), Mathf.Round(sceneBounds.max.y), Mathf.Round(sceneBounds.max.z), Mathf.Round(sceneBounds.min.x), Mathf.Round(sceneBounds.min.y), Mathf.Round(sceneBounds.min.z), false, ResourcesManager.Instance.GetConfigData().metaverse, adSpots, ResourcesManager.Instance.GetConfigData().userSignature);
        }
        public void setUserId(string client_uuid) {
            PlayerPrefs.SetString(CLIENT_UUID_PREFS, client_uuid);
        }
        public string GetClientId() {

            if (string.IsNullOrEmpty(PlayerPrefs.GetString(CLIENT_UUID_PREFS)))
            {
                clientId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(CLIENT_UUID_PREFS, clientId);
            }
            else {
                clientId = PlayerPrefs.GetString(CLIENT_UUID_PREFS);
            }
            return clientId;
        }

        [Serializable]
        public class ResponseGetToken
        {
            public string token;
        }

    }
}








