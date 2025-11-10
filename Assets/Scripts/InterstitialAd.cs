using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnityId = "Interstitial_Android";
    string _adUnitId;

    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    [SerializeField] Button _interstitialAdButton;

    void Awake()
    {
        _adUnitId = _androidAdUnityId;
    }

    private void Update()
    {
        // Only update button state if button exists and hasn't been destroyed
        if (_interstitialAdButton != null && AdManager.Instance != null && AdManager.Instance.interstitialAd != null)
        {
            _interstitialAdButton.interactable = isReady;
        }
    }

    public void OnInterstitialAdButtonClicked()
    {
        Debug.Log("Interstitial ad button clicked!");
        ShowInterstitial();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before initialization");
            return;
        }

        Debug.Log("Loading interstitial ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (isReady)
        {
            //Hide banner ad on interstitial ad show...
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet!");
            LoadAd();
        }
    }

    public void ShowInterstitial()
    {
        if (AdManager.Instance.interstitialAd != null && isReady)
        {
            Debug.Log("Showing interstitial ad manually!");
            ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready yet, loading again!");
            LoadAd();
        }
    }

    public void OnUnityAdsAdLoaded(String placementId)
    {
        Debug.Log("Interstitial ad loaded!");
        if (_interstitialAdButton != null)
        {
            _interstitialAdButton.interactable = true;
        }
        isReady = true;
        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("Failed to load interstitial ad!");
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.LogWarning("User clicked on interstitial ad!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Interstitial ad watched completely");
            StartCoroutine(SlowDownTimeTemporarily(30f));
            LoadAd();
        }
        else
        {
            Debug.Log("Interstitial ad skipped or status is unknown!");
            LoadAd();
        }
    }

    private IEnumerator SlowDownTimeTemporarily(float seconds)
    {
        Time.timeScale = 0.4f;
        Debug.Log("Time slowed down to 0.4x for " + seconds + " sec");
        yield return new WaitForSeconds(seconds);

        Time.timeScale = 1.0f;
        Debug.Log("Time restored to normal!");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("Error showing interstitial ad!");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Showing interstitial ad at this moment!");
        Time.timeScale = 0f;
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = isReady;
    }
}