using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitManager : MonoBehaviour {

    public GameObject _manager;
    public InputField _populationSizeField;
    public InputField _populationKeepField;
    public InputField _layersField;
    public Toggle _biasToggle;
    public Button _startButton;
    public GameObject _canvas;

    public int _populationSize = 50;
    public int _populationKeep = 20;
    public int[] _layers = new int[] { 6, 10, 10, 2 };
    public bool _bias = true;

    // Use this for initialization
    void Start () {
        _canvas.transform.Find("Conf").gameObject.SetActive(false);
        _startButton.onClick.AddListener(StartSimulation);
    }

    void Update()
    {
        // Quit on ESC
        if (Input.GetKey("escape"))
            Application.Quit();
    }

    void StartSimulation()
    {
        // Population Size
        int popSize = 0;
        Int32.TryParse(_populationSizeField.text, out popSize);
        if (popSize <= 0)
            popSize = _populationSize;

        // Keep Population
        int popKeep = 0;
        Int32.TryParse(_populationKeepField.text, out popKeep);
        if (popKeep <= 0)
            popKeep = _populationKeep;
        if (popKeep >= popSize)
        {
            double halfPop = popSize / 2;
            popKeep = (int) Math.Floor(halfPop);
        }

        // Layers
        List<int> layersList = new List<int> {6};
        String[] layersField = _layersField.text.Replace(" ", "").Split(',');
        for (int i = 0; i < layersField.Length; ++i)
        {
            int layerValue = 0;
            Int32.TryParse(layersField[i], out layerValue);
            layersList.Add(layerValue);
        }
        layersList.Add(2);

        int[] layers;
        if (layersList.Count < 3)
        {
            layers = _layers;
        }
        else
        {
            bool valid = true;
            for (int i = 0; i < layersList.Count; ++i)
            {
                if (layersList[i] <= 0)
                    valid = false;
            }

            if (valid)
                layers = layersList.ToArray();
            else
                layers = _layers;
        }

        // Bias
        bool bias = _biasToggle.isOn;

        //Debug.Log(popSize);
        //Debug.Log(popKeep);
        //for (int i = 0; i < layers.Length; ++i)
        //    Debug.Log("L: "+layers[i]);
        //Debug.Log(bias);

        // Init Manager Values
        _manager.GetComponent<Manager>()._populationSize = popSize;
        _manager.GetComponent<Manager>()._selectedPopulationNb = popKeep;
        _manager.GetComponent<Manager>()._layers = layers;
        _manager.GetComponent<Manager>()._bias = bias;

        // Enable/Disable UI
        _canvas.transform.Find("Init").gameObject.SetActive(false);
        _canvas.transform.Find("Conf").gameObject.SetActive(true);

        // Set Conf UI
        _canvas.transform.Find("Conf").Find("PopSize").GetComponent<Text>().text = "Population Size: " + popSize;
        _canvas.transform.Find("Conf").Find("KeepSize").GetComponent<Text>().text = "Keep: " + popKeep + " best";
        String layersString = "Layers: [";
        for (int i = 0; i < layers.Length; ++i)
        {
            if (i > 0)
                layersString += ", ";
            layersString += layers[i];
        }
        layersString += "]";
        _canvas.transform.Find("Conf").Find("Layers").GetComponent<Text>().text = layersString;
        _canvas.transform.Find("Conf").Find("Bias").GetComponent<Text>().text = "Bias: " + bias;

        // Start Manager
        _manager.GetComponent<Manager>()._start = true;
    }
}
