using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    private string _adUnitId;

    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load banner ad before Unity ads was initialized!");
            return;
        }

        Debug.Log("Loading banner ad...");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded! Showing now...");
        ShowBannerAd();
    }

    private void OnBannerError(string message)
    {
        Debug.LogWarning("Banner error: " + message);
        // Retry after delay
        Invoke(nameof(LoadBanner), 5f);
    }

    private void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(_adUnitId, options);
    }

    private void OnBannerClicked()
    {
        Debug.Log("User clicked on banner ad!");
    }

    private void OnBannerHidden()
    {
        Debug.Log("Banner was hidden!");
    }

    private void OnBannerShown()
    {
        Debug.Log("Banner ad is now visible!");
    }
}