using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    string _layerHitName = "Player"; // The name of the layer set on each car

    List<string> _allGuids = new List<string>(); // The list of Guids of all the cars increased

    private void OnTriggerEnter(Collider other) // Once anything goes through the wall
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(_layerHitName)) // If this object is a car
        {
            CarUserControl car = other.transform.root.GetComponent<CarUserControl>(); // Get the compoent of the car
            string carGuid = car._guid; // Get the Unique ID of the car

            if (!_allGuids.Contains(carGuid)) // If we didn't increase the car before
            {
                _allGuids.Add(carGuid); // Make sure we don't increase it again
                car.OnCheckPoint(); // Increase the car's fitness
            }
        }
    }
}
