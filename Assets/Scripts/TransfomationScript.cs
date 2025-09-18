using UnityEngine;

public class TransfomationScript : MonoBehaviour
{
    public ObjectScript objScript;

    void Update()
    {
        if(ObjectScript.lastDragged != null){
            if(Input.GetKey(KeyCode.Z)) {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(0, 0, Time.deltaTime * 15f);
            }
            if (Input.GetKey(KeyCode.X))
            {
                ObjectScript.lastDragged.GetComponent<RectTransform>().transform.Rotate(0, 0, Time.deltaTime * -15f);
            }

            if(Input.GetKey(KeyCode.UpArrow))
            {
                if (ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y < 0.85f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x,
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y + 0.001f, 1f);

                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y > 0.45f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x,
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y - 0.001f, 1f);

                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x < 0.85f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y,
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x + 0.001f, 1f);

                }
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x > 0.45f)
                {
                    ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale = new Vector3(
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.y,
                        ObjectScript.lastDragged.GetComponent<RectTransform>().transform.localScale.x - 0.001f, 1f);

                }
            }

        }
    }
}
