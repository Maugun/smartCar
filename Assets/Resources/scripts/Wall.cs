using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    string _layerHitName = "Player"; // The name of the layer set on each car

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(_layerHitName)) // Make sure it's a car
        {
            collision.transform.root.GetComponent<CarUserControl>().WallHit(); // If it is a car, tell it that it just hit a wall
        }
    }
}
