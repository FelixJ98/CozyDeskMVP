using UnityEngine;

public class UnlockableBuilding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string buildingName;
    public int cost;

    public GameObject buildingPrefab;

    public void TryUnlock(LazyCoinManager coinManager)
    {
        if (coinManager.TrySpendCoins(cost))
        {
            UnlockBuilding();
        }
        else
        {
            Debug.Log($"Cannot unlock {buildingName}. Need {cost} coins.");
        }
    }

    private void UnlockBuilding()
    {
        Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        Debug.Log($"{buildingName} has been unlocked.");
    }
}
