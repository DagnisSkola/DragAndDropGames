using UnityEngine;
using UnityEngine.EventSystems;

public class TransformationScript : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float scaleSpeed = 0.5f;
    public static bool isTransforming = false;
    private bool rotateCW, rotateCCW, scaleUpY, scaleDownY, scaleUpX, scaleDownX;

    void Start()
    {
        Debug.Log("TransformationScript initialized");
    }

    void Update()
    {
        if (ObjectScript.lastDragged == null)
        {
            // Log when there's no object to transform
            if (isTransforming)
            {
                Debug.LogWarning("Transformation flags active but no lastDragged object!");
            }
            return;
        }

        RectTransform rt = ObjectScript.lastDragged.GetComponent<RectTransform>();

        if (rt == null)
        {
            Debug.LogError($"Object {ObjectScript.lastDragged.name} has no RectTransform component!");
            return;
        }

        // Log current scale at the start of transformations
        if (scaleUpX || scaleDownX || scaleUpY || scaleDownY)
        {
            Debug.Log($"Current scale: {rt.localScale} | ScaleUpX: {scaleUpX} | ScaleDownX: {scaleDownX} | ScaleUpY: {scaleUpY} | ScaleDownY: {scaleDownY}");
        }

        // Rotation
        if (rotateCW)
        {
            rt.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            Debug.Log($"Rotating CW: {rt.localEulerAngles.z}");
        }

        if (rotateCCW)
        {
            rt.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            Debug.Log($"Rotating CCW: {rt.localEulerAngles.z}");
        }

        // Scale Y
        if (scaleUpY)
        {
            if (rt.localScale.y < 0.9f)
            {
                rt.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);
                Debug.Log($"Scaling Up Y: {rt.localScale.y}");
            }
            else
            {
                Debug.LogWarning($"Cannot scale Y up - already at max: {rt.localScale.y}");
            }
        }

        if (scaleDownY)
        {
            if (rt.localScale.y > 0.35f)
            {
                rt.localScale -= new Vector3(0, scaleSpeed * Time.deltaTime, 0);
                Debug.Log($"Scaling Down Y: {rt.localScale.y}");
            }
            else
            {
                Debug.LogWarning($"Cannot scale Y down - already at min: {rt.localScale.y}");
            }
        }

        // Scale X
        if (scaleUpX)
        {
            if (rt.localScale.x < 0.9f)
            {
                rt.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
                Debug.Log($"Scaling Up X: {rt.localScale.x}");
            }
            else
            {
                Debug.LogWarning($"Cannot scale X up - already at max: {rt.localScale.x}");
            }
        }

        if (scaleDownX)
        {
            if (rt.localScale.x > 0.35f)
            {
                rt.localScale -= new Vector3(scaleSpeed * Time.deltaTime, 0, 0);
                Debug.Log($"Scaling Down X: {rt.localScale.x}");
            }
            else
            {
                Debug.LogWarning($"Cannot scale X down - already at min: {rt.localScale.x}");
            }
        }

        isTransforming = rotateCW || rotateCCW || scaleUpY || scaleDownY || scaleUpX || scaleDownX;
    }

    // Rotation methods
    public void StartRotateCW(BaseEventData data)
    {
        rotateCW = true;
        Debug.Log("Started Rotate CW");
    }

    public void StopRotateCW(BaseEventData data)
    {
        rotateCW = false;
        Debug.Log("Stopped Rotate CW");
    }

    public void StartRotateCCW(BaseEventData data)
    {
        rotateCCW = true;
        Debug.Log("Started Rotate CCW");
    }

    public void StopRotateCCW(BaseEventData data)
    {
        rotateCCW = false;
        Debug.Log("Stopped Rotate CCW");
    }

    // Scale Y methods
    public void StartScaleUpY(BaseEventData data)
    {
        scaleUpY = true;
        Debug.Log("Started Scale Up Y");
    }

    public void StopScaleUpY(BaseEventData data)
    {
        scaleUpY = false;
        Debug.Log("Stopped Scale Up Y");
    }

    public void StartScaleDownY(BaseEventData data)
    {
        scaleDownY = true;
        Debug.Log("Started Scale Down Y");
    }

    public void StopScaleDownY(BaseEventData data)
    {
        scaleDownY = false;
        Debug.Log("Stopped Scale Down Y");
    }

    // Scale X methods
    public void StartScaleUpX(BaseEventData data)
    {
        scaleUpX = true;
        Debug.Log("Started Scale Up X");
    }

    public void StopScaleUpX(BaseEventData data)
    {
        scaleUpX = false;
        Debug.Log("Stopped Scale Up X");
    }

    public void StartScaleDownX(BaseEventData data)
    {
        scaleDownX = true;
        Debug.Log("Started Scale Down X");
    }

    public void StopScaleDownX(BaseEventData data)
    {
        scaleDownX = false;
        Debug.Log("Stopped Scale Down X");
    }
}