using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    [SerializeField] Button _rewardedButtons;
    public FlyingObjectManager flyingObjectManager;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (flyingObjectManager == null)
            flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load rewarded ad before Unity ads was initialized");
            return;
        }
        Debug.Log("Loading rewarded ad.");
        Advertisement.Load(_adUnitId, this);
    }

    // IUnityAdsLoadListener implementation
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded ad loaded!");
        if (placementId.Equals(_adUnitId))
        {
            if (_rewardedButtons != null)
                _rewardedButtons.interactable = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Failed to load rewarded ad: {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    public IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    // IUnityAdsShowListener implementation
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Failed to show rewarded ad: {message}");
        Time.timeScale = 1f; // Resume game
        StartCoroutine(WaitAndLoad(5f));
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Rewarded ad started");
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on rewarded ad");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if(placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsCompletionState.COMPLETED))
        {
            Debug.Log("Rwarded ad complete!");
            flyingObjectManager.DestroyAllFlyingObjects();
            _rewardedButtons.interactable = false;
            StartCoroutine(WaitAndLoad(10f));
        }
        Time.timeScale = 1f;
    }

    public void SetButton(Button button)
    {
        if(button == null)
        {
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowAd);
        _rewardedButtons = button;
        _rewardedButtons.interactable = false;
    }

    public void ShowAd()
    {
        _rewardedButtons.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }
}