﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class CarUserControl : MonoBehaviour
{
    private CarController _car;             // the car controller we want to use
    private bool _initilized = false;
    private NeuralNetwork _net;
    private float _h = 0f;
    private float _v = 0f;
    public string _guid;                    // The Unique ID of the current car
    public int _secBeforeDeath = 5;
    public int _rayCastLen = 12;

    [SerializeField]
    private LayerMask _sensorMask;          // Defines the layer of the walls ("Wall")
    private LineRenderer _lr;
    private bool _displayNN = false;
    private GameObject _canvas = null;

    public bool _isPlayer = false;

    public void Init(NeuralNetwork net, int secBeforeDeath, GameObject canvas = null)
    {
        if (canvas != null)
        {
            _canvas = canvas;
            _displayNN = true;
            transform.Find("Indicator").gameObject.SetActive(true);
        }
        _secBeforeDeath = secBeforeDeath;
        _guid = Guid.NewGuid().ToString();
        _car = GetComponent<CarController>();
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 14;
        _net = net;
        transform.Find("SkyCar").Find("SkyCarBody").GetComponent<MeshRenderer>().material.color = _net._color;
        _initilized = true;
        StartCoroutine(IsNotImproving());
    }

    private void FixedUpdate()
    {
        if (_isPlayer && !_initilized)
        {
            _car = GetComponent<CarController>();
            _initilized = true;
        }

        if (_initilized)
        {
            if (_isPlayer)
            {
                // Give inputs to the car!
                _h = CrossPlatformInputManager.GetAxis("Horizontal");
                _v = CrossPlatformInputManager.GetAxis("Vertical");
            }
            else
            {
                // NN Actions
                Brain();

                // Display NN
                if (_displayNN)
                    DisplayBrain();
            }
                

            //Debug.Log(_h + " , " + _v);
            _car.Move(_h, _v, _v, 0f);
        }
    }

    private void Brain()
    {
        // Cast forward, backward, right & left
        float[] inputs = new float[6];
        inputs[0] = (float)CastRay(transform.forward, Vector3.forward, 1);
        inputs[1] = (float) CastRay(-transform.forward, -Vector3.forward, 3);
        inputs[2] = (float) CastRay(transform.right, Vector3.right, 5);
        inputs[3] = (float) CastRay(-transform.right, -Vector3.right, 7);

        // Cast top right & top left
        float SqrtHalf = Mathf.Sqrt(0.5f);
        inputs[4] = (float) CastRay(transform.right * SqrtHalf + transform.forward * SqrtHalf,
                                    Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf,
                                    9);
        inputs[5] = (float) CastRay(-transform.right * SqrtHalf + transform.forward * SqrtHalf,
                                    -Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf,
                                    13);

        // Feed UNN
        float[] output = _net.FeedForward(inputs);

        // Set Output
        _v = output[0];
        _h = output[1];
        //Debug.Log(output.ToString());
    }

    double CastRay(Vector3 rayDirection, Vector3 lineDirection, int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayDirection, out hit, _rayCastLen, _sensorMask)) // Cast a ray
        {
            float dist = Vector3.Distance(hit.point, transform.position); // Get the distance of the hit in the line
            _lr.SetPosition(index, dist * lineDirection);
            return dist; // Return the distance
        }
        else
        {
            _lr.SetPosition(index, _rayCastLen * lineDirection);
            return _rayCastLen; // Return the maximum distance
        }
            
    }

    public void OnCheckPoint()
    {
        _net.AddFitness(1);
    }

    public void WallHit()
    {
        // Tell the Evolution Manager that the car is dead
        gameObject.SetActive(false); // Make sure the car is inactive
        GameObject.Find("Manager").GetComponent<Manager>()._deadNb +=1;
        //Debug.Log(GameObject.Find("Manager").GetComponent<Manager>()._deadNb);
    }

    IEnumerator IsNotImproving()
    {
        while (true)
        {
            float OldFitness = _net.GetFitness(); // Save the initial fitness
            yield return new WaitForSeconds(_secBeforeDeath); // Wait for some time
            if (OldFitness == _net.GetFitness()) // Check if the fitness didn't change yet
                WallHit(); // Kill this car
        }
    }

    private void DisplayBrain()
    {
        for (int i = 0; i < _net._neurons.Length; ++i)
        {
            for (int y = 0; y < _net._neurons[i].Length; ++y)
            {
                _canvas.transform.Find("Best NN").Find("n[" + i + "," + y + "]").GetChild(0).GetComponent<Text>().text = _net._neurons[i][y].ToString("0.000");
            }
        }
    }
}
