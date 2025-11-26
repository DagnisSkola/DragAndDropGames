using UnityEngine;

public class AllowAutoRotation : MonoBehaviour
{
    private ScreenOrientation previousOrientation;

    void Awake()
    {
        // Save the current orientation before changing it
        previousOrientation = Screen.orientation;

        // Enable auto-rotation
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    void OnDestroy()
    {
        // Optional: revert to the previous orientation when leaving this scene
        Screen.orientation = previousOrientation;
    }
}
