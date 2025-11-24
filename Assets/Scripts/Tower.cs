using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Stack<GameObject> rings = new Stack<GameObject>();
    public float ringHeight = 0.4f; // Height between rings
    public float baseHeight = -1.75f; // Starting height above tower

    public void AddRing(GameObject ring)
    {
        rings.Push(ring);
        UpdateRingPosition(ring);
    }

    public GameObject RemoveRing()
    {
        if (rings.Count == 0)
            return null;

        return rings.Pop();
    }

    public GameObject PeekTopRing()
    {
        if (rings.Count == 0)
            return null;

        return rings.Peek();
    }

    public int GetTopRingSize()
    {
        if (rings.Count == 0)
            return int.MaxValue; // Tower is empty, any ring can be placed

        return rings.Peek().GetComponent<RingData>().size;
    }

    private void UpdateRingPosition(GameObject ring)
    {
        float yPosition = baseHeight + (rings.Count - 1) * ringHeight;
        ring.transform.position = new Vector3(
            transform.position.x,
            yPosition,
            0
        );
    }

    public void RefreshAllRingPositions()
    {
        GameObject[] ringArray = rings.ToArray();
        for (int i = 0; i < ringArray.Length; i++)
        {
            float yPosition = baseHeight + i * ringHeight;
            ringArray[ringArray.Length - 1 - i].transform.position = new Vector3(
                transform.position.x,
                yPosition,
                0
            );
        }
    }

    public int GetRingCount()
    {
        return rings.Count;
    }
}