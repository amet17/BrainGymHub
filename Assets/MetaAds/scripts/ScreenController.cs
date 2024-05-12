using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using System.Net;

namespace MetaAds
{
    public class ScreenController : MonoBehaviour
    {
        private string PATH_STATISTICS_Url= "/hb/heart-beat";
        private string PATH_SEND_TO_STATISTIC_SCREEN_CLICKED= "/tornado/adspot/click-on-external-link";
        private string PATH_CREATIVE_URL;
        private VideoPlayer currentVideoPlayer;
        private Renderer currentObjectRenderer;
        private ServerResponseData currentResponseData;
        private StatisticsData currentStatisticsData;
        private float timeUntilEnd;//temporarily, consult how often to send a request if the server does not respond
        public string wallet; // User wallet in MetaAds system
        private string authorizationToken; // authorization Token to send statistics data
        private bool isVisibleInCamera; // is Visible In Camera in this moment
        private ConfigData configData;
        private string objectId;
        int requestCreativeCoroutineId=0;
        private UniqueId uniqueIdObject;
        private AudioSource audioSource;
        private bool isPause;

        private void Awake()
        {
            objectId = GetComponent<UniqueId>().getUniqueId();
            configData = ResourcesManager.Instance.GetConfigData();
            PATH_CREATIVE_URL = configData.streamUrl;
            currentVideoPlayer = GetComponent<VideoPlayer>();
            currentObjectRenderer = GetComponent<Renderer>();
            currentResponseData = new ServerResponseData();
            currentStatisticsData = new StatisticsData(objectId);
            audioSource = GetComponent<AudioSource>();
            FindObjectOfType<StatisticsCollectionArea>();
            UserStatisticsManager.Instance.UpdateStatisticsCollectionArea();
        }
        void Start()
        {
            currentVideoPlayer.loopPointReached += OnVideoEnd;
            StartCoroutine(RequestCreative());
            StartCoroutine(SendStatistics());
            UserStatisticsManager.Instance.StartCollectingStatistics();

        }
        private void OnVideoEnd(VideoPlayer player)
        {
            currentResponseData.url = null;
            currentVideoPlayer.Stop();
            StartCoroutine(RequestCreative());
        }
        void OnMouseDown()
        {
            if (string.IsNullOrEmpty(currentResponseData.external_link))
            {
                return;
            }
            currentStatisticsData.clicked = true;
            StartCoroutine(SendToStatisticsScreenClicked());
#if UNITY_WEBGL
            var jsCode = "var newTab = window.open(); newTab.opener = null; newTab.location='" + currentResponseData.external_link + "';";
            Application.ExternalEval(jsCode);
#else
            Application.OpenURL(currentResponseData.external_link);
#endif

        }
        private void OnApplicationFocus(bool hasFocus)
        {
#if UNITY_WEBGL
            if (Screen.fullScreen) {
                isPause = false;
                resumeVideoPlayer();
                return;
            }
#endif
            if (!hasFocus)
            {
                isPause = true;
                pauseVideoPlayer();
            }
            else
            {
                isPause = false;
                resumeVideoPlayer();
            }
        }
        private void pauseVideoPlayer() {
            if (currentVideoPlayer != null && currentVideoPlayer.enabled) {
                currentVideoPlayer.Pause();
            }
        }
        private void resumeVideoPlayer()
        {
            if (currentVideoPlayer != null && currentVideoPlayer.enabled)
            {
                currentVideoPlayer.Play();
                StartCoroutine(RequestCreative());
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            gameObject.GetComponent<Renderer>().material = ResourcesManager.Instance.materialForImage;
            if (uniqueIdObject == null)
            {
                uniqueIdObject = GetComponent<UniqueId>();
                if (uniqueIdObject == null)
                {
                    uniqueIdObject = gameObject.AddComponent<UniqueId>();
                }
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            if (currentVideoPlayer == null)
            {
                currentVideoPlayer = gameObject.GetComponent<VideoPlayer>();
                if (currentVideoPlayer == null)
                {
                    currentVideoPlayer = gameObject.AddComponent<VideoPlayer>();
                }
            }
        }

#endif

        public string getDisplayUUID() {
            return objectId;
        }
        private void OnBecameVisible() // caught in the camera's field of view
        {
            isVisibleInCamera = true;
            currentStatisticsData.focused = true;
        }

        private void OnBecameInvisible() // disappeared from camera view
        {
            isVisibleInCamera = false;
        }

        IEnumerator DownloadImage(string MediaUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();
            if (!request.isNetworkError && !request.isHttpError)
            {
                currentVideoPlayer.Stop();
                currentVideoPlayer.enabled = false;
                currentObjectRenderer.material = ResourcesManager.Instance.materialForImage;
                currentObjectRenderer.material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        private void PlayVideoContent(string urlVideoContent)
        {
           currentVideoPlayer.Stop();
           currentVideoPlayer.url = urlVideoContent;
           currentObjectRenderer.material = ResourcesManager.Instance.materialForVideoContent;
           currentVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
           currentVideoPlayer.controlledAudioTrackCount = 1;
           currentVideoPlayer.SetTargetAudioSource(0, audioSource);
           currentVideoPlayer.enabled = true;
           currentVideoPlayer.Play();
            if (isPause) {
                currentVideoPlayer.Pause();
            }
        }

        private IEnumerator RequestCreative()
        {    
            requestCreativeCoroutineId ++;
            int currentRequestCreativeCoroutineId = requestCreativeCoroutineId;
            wallet = UserStatisticsManager.Instance.GetClientId();
            while (currentRequestCreativeCoroutineId==requestCreativeCoroutineId)
            {
                var token = getToken(configData.userId, objectId, wallet, configData.userSignature);
                UnityWebRequest request = UnityWebRequest.Get(PATH_CREATIVE_URL);
                request.SetRequestHeader("Authorization", token);
                yield return request.SendWebRequest();
               
                if (!request.isNetworkError && !request.isHttpError)
                {
                    ServerResponseData responseData = JsonUtility.FromJson<ServerResponseData>(request.downloadHandler.text);

                    try
                    {
                        if (responseData != null)
                        {
                            if (!string.IsNullOrEmpty(responseData.to_time))
                            {
                                var endTime = DateTime.Parse(responseData.to_time);
                                timeUntilEnd = (float)(endTime - DateTime.Now.ToUniversalTime()).TotalSeconds;
                            }
                            else
                            {
                                timeUntilEnd = configData.intervalBetweenRequests/1000;
                                responseData.to_time = "NullOrEmpty";
                            }
                           if (!responseData.url.Equals(currentResponseData.url) || !responseData.to_time.Equals(currentResponseData.to_time))
                            {
                                currentResponseData = responseData;
                                ChangeCreativeDependingServerResponse(responseData);
                            }
                        }
                        else
                        {
                            timeUntilEnd = configData.intervalBetweenRequests/1000;
                        }
                    }
                    catch (WebException e)
                    {
                        currentResponseData = null;
                        timeUntilEnd = configData.intervalBetweenRequests/1000;
                        Debug.Log("RequestCreative Exception - " + e.Message);
                    }
                    if (currentResponseData != null)
                    {
                        System.Threading.Interlocked.CompareExchange(ref currentResponseData, responseData, null);
                        System.Threading.Interlocked.CompareExchange(ref authorizationToken, currentResponseData.token, null);
                    }
                }
                else
                {
                    try
                    {
                        ServerResponseData responseData = JsonUtility.FromJson<ServerResponseData>(request.downloadHandler.text);
                        if (responseData.is_image && !string.IsNullOrEmpty(responseData.url))
                        {
                            ChangeCreativeDependingServerResponse(responseData);
                            timeUntilEnd = configData.intervalForSendMetrics / 1000;
                        }
                    }
                    catch
                    {
                        StartCoroutine(DownloadImage(ResourcesManager.Instance.GetConfigData().defaultImageUrl));
                    }
                }   
                
                yield return new WaitForSeconds(timeUntilEnd);
            }
        }

        private void ChangeCreativeDependingServerResponse(ServerResponseData responseData)
        {
            if (responseData.is_image)
            {
                StartCoroutine(DownloadImage(currentResponseData.url));
            }
            else
            {
                PlayVideoContent(currentResponseData.url);

            }
        }

        public string getToken(int id, string currentDisplayUuid, string wallet, string secretKey) // get a token having the following parameters 
        {
            Dictionary<string, object> payload = new Dictionary<string, object> {
                { "user_id", id },
                { "uuid", currentDisplayUuid },
                { "wallet", wallet },
                { "metaverse", configData.metaverse}
            };
            string token = JWT.JsonWebToken.Encode(payload, Encoding.UTF8.GetBytes(secretKey), JWT.JwtHashAlgorithm.HS256);
            return token;
        }

        IEnumerator SendStatistics()
        {
            while (true)
            {
                if (!currentStatisticsData.focused) // If the default value is recorded in the statistics, then we update the data to the value isVisibleInCamera (at the moment)
                {
                    currentStatisticsData.focused = isVisibleInCamera;
                }
                currentStatisticsData.creative_id = currentResponseData.creative_id;
                currentStatisticsData.session_id = currentResponseData.session_id;
                if (authorizationToken != null)
                {
                    UnityWebRequest request = UnityWebRequest.Head(configData.baseUrl + PATH_STATISTICS_Url);
                    string metricsJson = JsonUtility.ToJson(currentStatisticsData);
                    string metricsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(metricsJson));
                    request.SetRequestHeader("Authorization", authorizationToken);
                    request.SetRequestHeader("Metrics", metricsBase64);
                  /*  Debug.Log("Metrics=" +metricsJson);*/
                    currentStatisticsData.setDefoultValue();
                    yield return request.SendWebRequest();
/*                    if (request.isNetworkError || request.isHttpError)
                    {
                        Debug.Log("E R R O R= " + request.error);
                    }
                    else
                    {
                        Debug.Log("Metrics sent successfully! Status= " + request.responseCode);
                    }*/
                }
                else
                {
                    StartCoroutine(RequestCreative());
                }
                yield return new WaitForSeconds(configData.intervalForSendMetrics / 1000);
            }
        }
        IEnumerator SendToStatisticsScreenClicked()
        {
                var token = getToken(configData.userId, objectId, wallet, configData.userSignature);
            UnityWebRequest request = UnityWebRequest.Get(configData.baseUrl + PATH_SEND_TO_STATISTIC_SCREEN_CLICKED);
                request.SetRequestHeader("Authorization", token);
                yield return request.SendWebRequest();
            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log("E R R O R= " + request.error);
            }
            else
            {
                Debug.Log("SendToStatisticsScreenClicked send successfully! Status= " + request.responseCode);
            }

        }
    }
}