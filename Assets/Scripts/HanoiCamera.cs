using UnityEngine;

public class ForceLandscape : MonoBehaviour
{
    private ScreenOrientation previousOrientation;

    void Awake()
    {
        // Save the current orientation before changing it
        previousOrientation = Screen.orientation;

        // Force landscape orientation
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void OnDestroy()
    {
        // Revert to the previous orientation when leaving this scene
        Screen.orientation = previousOrientation;

        // Or if you want to allow auto-rotation when leaving:
        // Screen.orientation = ScreenOrientation.AutoRotation;
        // Screen.autorotateToPortrait = true;
        // Screen.autorotateToPortraitUpsideDown = true;
        // Screen.autorotateToLandscapeLeft = true;
        // Screen.autorotateToLandscapeRight = true;
    }
}