using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    private string _adUnitId;

    private bool _isReady = false;
    private Button _rewardedButton;
    public FlyingObjectManager flyingObjectManager;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (flyingObjectManager == null)
            flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();
    }

    private void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load rewarded ad before Unity Ads initialized!");
            return;
        }

        Debug.Log("Loading rewarded ad.");
        Advertisement.Load(_adUnitId, this);
    }

    // --- IUnityAdsLoadListener ---
    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("Rewarded ad loaded!");
        _isReady = true;

        if (_rewardedButton != null)
            _rewardedButton.interactable = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Failed to load rewarded ad: {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    private IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadAd();
    }

    // --- IUnityAdsShowListener ---
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Failed to show rewarded ad: {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Rewarded ad started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked rewarded ad");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded ad complete!");

            if (flyingObjectManager != null)
                flyingObjectManager.DestroyAllFlyingObjects();

            if (_rewardedButton != null)
                _rewardedButton.interactable = false;

            // Slow down time 75% for 10 seconds
            StartCoroutine(SlowDownTimeTemporarily(0.25f, 20f));

            StartCoroutine(WaitAndLoad(20f));
        }
    }

    private IEnumerator SlowDownTimeTemporarily(float targetTimeScale, float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = targetTimeScale;
        Debug.Log($"Time slowed down to {targetTimeScale * 100}% for {duration} seconds");

        // Wait using real time so it ignores Time.timeScale
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        Debug.Log("Time restored to normal!");
    }


    // --- Button setup ---
    public void SetButton(Button button)
    {
        if (button == null) return;

        _rewardedButton = button;
        _rewardedButton.onClick.RemoveAllListeners();
        _rewardedButton.onClick.AddListener(ShowAd);
        _rewardedButton.interactable = false;

        // Enable button immediately if ad already loaded
        if (_isReady)
            _rewardedButton.interactable = true;
    }

    public void ShowAd()
    {
        if (_rewardedButton != null)
            _rewardedButton.interactable = false;

        if (_isReady)
        {
            _isReady = false;
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready yet.");
            LoadAd();
        }
    }
}
