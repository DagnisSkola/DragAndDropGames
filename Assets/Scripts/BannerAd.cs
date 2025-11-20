using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class BannerAd : MonoBehaviour
{
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    private string _adUnitId;

    private Button _bannerButton;
    public bool isBannerVisible = false;

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
        Debug.Log("Banner ad loaded!");
        isBannerVisible = false;

        // Enable button if assigned
        if (_bannerButton != null)
            _bannerButton.interactable = true;

        // Auto-show banner immediately
        ShowBannerAd();
    }


    private void OnBannerError(string message)
    {
        Debug.LogWarning("Banner error: " + message);
        // Retry after delay
        Invoke(nameof(LoadBanner), 5f);
    }

    public void ShowBannerAd()
    {
        if (isBannerVisible)
        {
            HideBannerAd();
        }
        else
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(_adUnitId, options);
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        isBannerVisible = false;
    }

    private void OnBannerClicked()
    {
        Debug.Log("User clicked on banner ad!");
    }

    private void OnBannerHidden()
    {
        Debug.Log("Banner is hidden!");
        isBannerVisible = false;
    }

    private void OnBannerShown()
    {
        Debug.Log("Banner ad is visible!");
        isBannerVisible = true;
    }

    // --- Button Setup ---
    public void SetButton(Button button)
    {
        if (button == null)
            return;

        _bannerButton = button;
        _bannerButton.onClick.RemoveAllListeners();
        _bannerButton.onClick.AddListener(ShowBannerAd);

        // Disable initially until banner loads
        _bannerButton.interactable = false;

        // Enable immediately if banner is already loaded
        if (Advertisement.isInitialized)
            _bannerButton.interactable = true;
    }
}
