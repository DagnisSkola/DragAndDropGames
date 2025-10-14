using UnityEngine;

public class TransfomationScript : MonoBehaviour
{
    public ObjectScript objScript;

    void Update()
    {
        if (ObjectScript.lastDragged != null)
        {
            // Rotation controls
            if (Input.GetKey(KeyCode.Z))
            {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(0, 0, Time.deltaTime * 30f);
            }
            if (Input.GetKey(KeyCode.X))
            {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(0, 0, Time.deltaTime * -30f);
            }

            // Get current scale
            Vector3 currentScale = ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale;

            // Scale Y axis (Up/Down arrows)
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (currentScale.y < 0.85f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        currentScale.x,
                        currentScale.y + 0.001f,
                        1f);
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (currentScale.y > 0.45f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        currentScale.x,
                        currentScale.y - 0.001f,
                        1f);
                }
            }

            // Scale X axis (Left/Right arrows)
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (currentScale.x > 0.45f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        currentScale.x - 0.001f,
                        currentScale.y,
                        1f);
                }
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (currentScale.x < 0.85f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        currentScale.x + 0.001f,
                        currentScale.y,
                        1f);
                }
            }
        }
    }
}