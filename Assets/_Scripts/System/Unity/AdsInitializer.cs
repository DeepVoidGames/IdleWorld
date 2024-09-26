using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
 
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [SerializeField] private RewardedAdsButton _rewardedAdsButton;
    [SerializeField] private InterstitialAdExample _interstitialAdExample;
 
    void Awake()
    {
        InitializeAds();
    }
 
    public void InitializeAds()
    {
        #if UNITY_IOS
                _gameId = _iOSGameId;
        #elif UNITY_ANDROID
                _gameId = _androidGameId;
        #elif UNITY_EDITOR
                _gameId = _androidGameId; //Only for testing the functionality in the Editor
        #endif
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        StartCoroutine(ShowAds());
    }

    IEnumerator ShowAds()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(2.5f);
        }
        _rewardedAdsButton.LoadAd();
        _interstitialAdExample.LoadAd();
    }
 
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}