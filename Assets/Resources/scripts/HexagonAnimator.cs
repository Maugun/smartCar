using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    THIS SCRIPT IS NOT USED IN THE CURRENT PROJECT

    This script was used to test the NN in early dev
*/

public class HexagonAnimator : MonoBehaviour
{
    bool increasingSize = true;
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = Color.yellow;
    }

    void Update()
    {
        float delta = Time.deltaTime;
        Vector3 angles = transform.eulerAngles;
        angles.z += delta * 50f;
        transform.eulerAngles = angles;

        Vector3 localScale = transform.localScale;
        if (increasingSize == true)
        {
            localScale += new Vector3(delta, delta, 0f);
            if (localScale.x >= 2f)
            {
                increasingSize = false;
            }
        }
        else if (increasingSize == false)
        {
            localScale -= new Vector3(delta, delta, 0f);
            if (localScale.x <= 1f)
            {
                increasingSize = true;
            }
        }

        transform.localScale = localScale;
    }
}
