using UnityEngine;

public class RingData : MonoBehaviour
{
    public int size; // Smaller number = smaller ring

    public void Initialize(int ringSize)
    {
        size = ringSize;
    }
}