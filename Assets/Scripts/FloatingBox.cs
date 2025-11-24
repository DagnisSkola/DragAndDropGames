using UnityEngine;

public class FloatingBox : MonoBehaviour
{
    private GameObject heldRing;
    public float ringOffset = -0.5f; // Offset below the box

    public bool IsEmpty()
    {
        return heldRing == null;
    }

    public void PlaceRing(GameObject ring)
    {
        heldRing = ring;
        UpdateRingPosition();
    }

    public GameObject RemoveRing()
    {
        GameObject ring = heldRing;
        heldRing = null;
        return ring;
    }

    public int GetHeldRingSize()
    {
        if (heldRing == null)
            return -1;

        return heldRing.GetComponent<RingData>().size;
    }

    private void UpdateRingPosition()
    {
        if (heldRing != null)
        {
            heldRing.transform.position = new Vector3(
                transform.position.x,
                transform.position.y + ringOffset,
                0
            );
        }
    }
}