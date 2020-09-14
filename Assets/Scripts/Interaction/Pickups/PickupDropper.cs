using UnityEngine;

public class PickupDropper : MonoBehaviour
{
    public Pickup[] pickups;

    public void SpawnPickup(int i)
    {
        GameManager.Instance.SpawnPickup(pickups[i], transform.position);
    }

    public void SpawnRandomPickupOnChange(float percentage)
    {
        if (Random.Range(0f, 1f) > percentage) return;

        SpawnRandomPickup();
    }

    public void SpawnRandomPickup()
    {
        SpawnPickup(Random.Range(0, pickups.Length));
    }
}
