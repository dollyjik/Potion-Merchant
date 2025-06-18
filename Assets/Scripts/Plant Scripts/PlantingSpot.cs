using UnityEngine;

public class PlantingSpot : MonoBehaviour
{
    public bool isOccupied = false;
    private GameObject plantedPlant;

    public bool Plant(GameObject plantPrefabToPlant)
    {
        if (isOccupied)
        {
            Debug.Log("This spot is already occupied.");
            return false;
        }

        plantedPlant = Instantiate(plantPrefabToPlant, transform.position, transform.rotation);
        isOccupied = true;
        
        Debug.Log(plantPrefabToPlant.name + " was planted at " + gameObject.name);
        return true;
    }

    public void ClearSpot()
    {
        if (plantedPlant != null)
        {
            Destroy(plantedPlant);
        }
        isOccupied = false;
    }
}