using UnityEngine;

public class PoopManager : MonoBehaviour
{
    public GameObject poopPrefab;
    public float spawnHeight = -2.75f;
    public float spawnX = -1.9f;
    public float poopSpacing = 0.5f;
    public Transform parent;

    public void SpawnPoop()
    {
        float nextY = spawnHeight;

        if (parent != null && parent.childCount > 0)
        {
            Transform lastPoop = parent.GetChild(parent.childCount - 1);
            nextY = lastPoop.position.y + poopSpacing;
        }

        Vector3 spawnPosition = new Vector3(spawnX, nextY, 0f);

        GameObject newPoop = Instantiate(poopPrefab, spawnPosition, Quaternion.identity);

        if (parent != null)
        {
            newPoop.transform.SetParent(parent);
        }
    }

    public void ClearAllPoops()
    {
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("Todas las cacas han sido limpiadas.");
    }
}
