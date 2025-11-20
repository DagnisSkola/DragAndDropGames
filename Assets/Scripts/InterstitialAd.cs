using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    private string _adUnitId;

    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    private Button _interstitialAdButton;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }

    private void Update()
    {
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = isReady;
    }

    public void OnInterstitialAdButtonClicked()
    {
        ShowInterstitial();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before Unity Ads initialized!");
            return;
        }

        Debug.Log("Loading interstitial ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (isReady)
        {
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready, loading...");
            LoadAd();
        }
    }

    public void ShowInterstitial()
    {
        if (isReady)
            ShowAd();
        else
            LoadAd();
    }

    // --- Unity Ads Callbacks ---
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial ad loaded!");
        isReady = true;

        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = true;

        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Failed to load interstitial ad: {error} - {message}");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial ad started showing!");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on interstitial ad!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Interstitial ad show complete. Reloading...");
        LoadAd(); // only preload, do not auto-show
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Error showing interstitial ad: {error} - {message}");
        LoadAd();
    }

    // --- Button setup ---
    public void SetButton(Button button)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;

        // Set interactable based on current ad state
        _interstitialAdButton.interactable = isReady;
    }
}
