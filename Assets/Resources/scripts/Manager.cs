using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    static public string GENERATION = "Generation: ";
    static public string BEST = "Best: ";
    static public string AVG = "AVG: ";
    static public string WORST = "Worst: ";
    static public string PERCENT = "%";

    public GameObject _prefab;
    public GameObject _spawn;
    public GameObject _canvas;
    public Sprite _neuronSprite;
    public Button _displaySwitch;
    public Button _pause;
    public bool _bias = true;
    public bool _start = true;

    private bool _isTraning = false;
    public int _populationSize = 50;
    public int _selectedPopulationNb = 25;
    public int _secBeforeDeath = 5;
    private int _generationNumber = 0;
    public int[] _layers = new int[] { 6, 10, 10, 2 };
    private List<NeuralNetwork> _nets;
    private List<CarUserControl> _prefabList = null;
    public int _deadNb = 0;
    private LineRenderer _avgLineRenderer = null;
    private List<float> _avgFitnessList = new List<float>();
    private LineRenderer _bestLineRenderer = null;
    private List<float> _bestFitnessList = new List<float>();
    private LineRenderer _worstLineRenderer = null;
    private List<float> _worstFitnessList = new List<float>();
    private float _maxFitness = 0;
    private Vector2 _resolutionScale;
    private int _displayMode = 0;
    public Material _weightMaterial;
    private NeuralNetwork _bestNN = null;

    private void Start()
    {
        // UI
        _canvas.transform.Find("GenerationNb").gameObject.SetActive(false);
        _canvas.transform.Find("Graph & Infos").gameObject.SetActive(false);
        _canvas.transform.Find("Best NN").gameObject.SetActive(false);
        Camera.main.transform.Find("Draw").gameObject.SetActive(false);

        // Button
        _displaySwitch.onClick.AddListener(SwitchDisplayMode);
        _pause.onClick.AddListener(Pause);
        _displaySwitch.gameObject.SetActive(false);
        //_pause.gameObject.SetActive(false);
        if (_start)
            _pause.transform.Find("Text").GetComponent<Text>().text = "Pause Generation";
        else
            _pause.transform.Find("Text").GetComponent<Text>().text = "Start Generation";
    }

    void Update()
    {
        if (_start)
        {
            if (_deadNb >= _populationSize)
            {
                _isTraning = false;
                _deadNb = 0;
            }

            if (_isTraning == false)
            {
                if (_generationNumber == 0)
                {
                    InitBoomerangNeuralNetworks();
                    _avgFitnessList.Add(0);
                    _bestFitnessList.Add(0);
                    _worstFitnessList.Add(0);
                    DrawGraph(_avgLineRenderer, _avgFitnessList);
                    DrawGraph(_bestLineRenderer, _bestFitnessList);
                    DrawGraph(_worstLineRenderer, _worstFitnessList);
                    _maxFitness = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
                }
                else
                {
                    // Sort UNN by Fitness
                    _nets.Sort();

                    // Data Display
                    UpdateDisplay();

                    // Selected UNN Population + Mutation
                    int selectedPop = 1;
                    for (int i = 0; i < _populationSize - _selectedPopulationNb; ++i)
                    {
                        if (selectedPop > _selectedPopulationNb)
                            selectedPop = 1;
                        _nets[i] = new NeuralNetwork(_nets[_populationSize - selectedPop]);
                        _nets[i].Mutate();
                        selectedPop++;
                    }

                    // Selected UNN Population
                    for (int i = _populationSize - _selectedPopulationNb; i < _populationSize; ++i)
                        _nets[i] = new NeuralNetwork(_nets[i]);

                    // Reset Fitness
                    for (int i = 0; i < _populationSize; i++)
                        _nets[i].SetFitness(0f);
                }

                // Set new generation
                _generationNumber++;
                _isTraning = true;
                CreatePrefabsBodies();
            }
        }
    }


    private void CreatePrefabsBodies()
    {
        if (_prefabList != null)
        {
            for (int i = 0; i < _prefabList.Count; i++)
            {
                GameObject.Destroy(_prefabList[i].gameObject);
            }

        }

        _prefabList = new List<CarUserControl>();

        for (int i = 0; i < _populationSize; i++)
        {
            CarUserControl car = ((GameObject)Instantiate(_prefab, _spawn.transform.position, _spawn.transform.rotation)).GetComponent<CarUserControl>();

            if (_generationNumber > 1 && i == _populationSize - 1)
                car.Init(_nets[i], _secBeforeDeath, _canvas);
            else
                car.Init(_nets[i], _secBeforeDeath);
            _prefabList.Add(car);
        }
    }

    void InitBoomerangNeuralNetworks()
    {
        _nets = new List<NeuralNetwork>();
        _avgLineRenderer = Camera.main.transform.Find("Draw").Find("Graph").Find("avgLineRenderer").GetComponent<LineRenderer>();
        _bestLineRenderer = Camera.main.transform.Find("Draw").Find("Graph").Find("bestLineRenderer").GetComponent<LineRenderer>();
        _worstLineRenderer = Camera.main.transform.Find("Draw").Find("Graph").Find("worstLineRenderer").GetComponent<LineRenderer>();

        // Init NN
        for (int i = 0; i < _populationSize; i++)
        {
            Color color = new Color(Random.value, Random.value, Random.value, 1.0f);
            NeuralNetwork net = new NeuralNetwork(_layers, color, _bias);
            net.Mutate();
            _nets.Add(net);
        }

        // Init UI
        _canvas.transform.Find("GenerationNb").gameObject.SetActive(true);
        _canvas.transform.Find("Graph & Infos").gameObject.SetActive(true);
        _canvas.transform.Find("Best NN").gameObject.SetActive(true);
        Camera.main.transform.Find("Draw").gameObject.SetActive(true);

        // Init Button
        _displaySwitch.gameObject.SetActive(true);
        _pause.gameObject.SetActive(true);
        if (_start)
            _pause.transform.Find("Text").GetComponent<Text>().text = "Pause Generation";
        else
            _pause.transform.Find("Text").GetComponent<Text>().text = "Start Generation";

        // Init NN Display
        InitDisplay();
    }

    void DrawGraph(LineRenderer lr, List<float> pts)
    {
        // Display only last 100 pts
        int lastHundredIndex = pts.Count - 100;
        if (lastHundredIndex < 0)
            lastHundredIndex = 0;

        Vector3[] vectorArray = null;
        if (lastHundredIndex > 0)
            vectorArray = new Vector3[100];
        else
            vectorArray = new Vector3[pts.Count];

        for (int i = 0; i < vectorArray.Length; ++i)
            vectorArray[i] = new Vector3(i * 0.5f, pts[i + lastHundredIndex] * 0.5f, 0);

        lr.positionCount = vectorArray.Length;
        lr.SetPositions(vectorArray);
        _canvas.transform.Find("Graph & Infos").Find("xAxis").Find("x0").GetComponent<Text>().text = "" +  lastHundredIndex;
        _canvas.transform.Find("Graph & Infos").Find("xAxis").Find("x100").GetComponent<Text>().text = "" + (lastHundredIndex + 100);
    }

    void InitDisplay()
    {
        int w = Screen.width;
        int h = Screen.height;

        // Resolution Scale
        Vector2 refReso = _canvas.GetComponent<CanvasScaler>().referenceResolution;
        _resolutionScale = new Vector2(w / refReso.x, h / refReso.y);

        for (int i = 0; i < _layers.Length; ++i)
        {
            for (int y = 0; y < _layers[i]; ++y)
            {
                // Neurons
                DrawNeuron(i, y, 0);

                // if not first Layer
                if (i > 0)
                {
                    // Weights lines Container
                    var neuronLineContainer = new GameObject
                    {
                        name = "n[" + i + "," + y + "]"
                    };
                    neuronLineContainer.transform.SetParent(Camera.main.transform.Find("Draw").Find("Best NN"));
                    neuronLineContainer.transform.position = Vector3.zero;
                    neuronLineContainer.transform.localPosition = Vector3.zero;
                    neuronLineContainer.transform.rotation = new Quaternion(0, 0, 0, 0);
                    neuronLineContainer.transform.localRotation = new Quaternion(0, 0, 0, 0);

                    // Weights Lines
                    DrawWeight(i, y, neuronLineContainer, _bias);
                }

                // If not last Layer
                if (i + 1 < _layers.Length)
                {
                    // Bias
                    if (y + 1 == _layers[i] && _bias)
                    {
                        DrawNeuron(i, y + 1, 1);
                    }
                }
            }
        }
        _canvas.transform.Find("Best NN").gameObject.SetActive(false);
        Camera.main.transform.Find("Draw").Find("Best NN").gameObject.SetActive(false);
    }

    void DrawNeuron(int i, int y, float value)
    {
        // Neuron Sprit
        var img = new GameObject
        {
            layer = 5,
            name = "n[" + i + "," + y + "]"
        };
        img.AddComponent<Image>();
        img.GetComponent<Image>().sprite = _neuronSprite;
        img.transform.localScale = new Vector3(0.5f * _resolutionScale.x, 0.5f * _resolutionScale.y, 1f);
        img.transform.SetParent(_canvas.transform.Find("Best NN").transform);
        img.transform.position = new Vector3((50f + (i * 100)) * _resolutionScale.x, (580f - (y * 50)) * _resolutionScale.y, 0f);

        // Neuron Text value
        var txt = new GameObject
        {
            layer = 5
        };
        txt.AddComponent<Text>();
        txt.GetComponent<Text>().fontSize = 12;
        txt.GetComponent<Text>().color = Color.black;
        txt.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        txt.transform.localScale = new Vector3(1 * _resolutionScale.x, 1 * _resolutionScale.y, 1f);
        txt.transform.SetParent(img.transform);
        txt.GetComponent<Text>().text = "" + value;
        txt.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.transform.position = new Vector3((50f + (i * 100)) * _resolutionScale.x, (580f - (y * 50)) * _resolutionScale.y, 0f);
    }

    void DrawWeight(int i, int y, GameObject neuronLineContainer, bool bias)
    {
        int addBias = 0;
        if (bias)
            addBias = 1;
        for (int j = 0; j < _layers[i - 1] + addBias; ++j)
        {
            List<Vector3> positions = new List<Vector3>
                        {
                            new Vector3(15f + ((i-1) * 16f), 71.5f + (y * -6.75f), 0f),
                            new Vector3(5f + ((i-1) * 16f), 71.5f + (j * -6.75f), 0f)
                        };

            var weight = new GameObject
            {
                name = "w[" + j + "]"
            };
            weight.AddComponent<LineRenderer>();
            weight.GetComponent<LineRenderer>().startWidth = 0.2f;
            weight.GetComponent<LineRenderer>().endWidth = 0.2f;
            weight.GetComponent<LineRenderer>().useWorldSpace = false;
            weight.GetComponent<LineRenderer>().SetPositions(positions.ToArray());
            weight.GetComponent<LineRenderer>().material = _weightMaterial;
            weight.transform.SetParent(neuronLineContainer.transform);
            weight.transform.position = Vector3.zero;
            weight.transform.localPosition = Vector3.zero;
            weight.transform.rotation = new Quaternion(0, 0, 0, 0);
            weight.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    void UpdateDisplay()
    {
        // Gen Info
        _canvas.transform.Find("GenerationNb").GetComponent<Text>().text = GENERATION + _generationNumber;

        // Fitness info
        float avgFitness = 0f;
        float bestFitness = _nets[_populationSize - 1].GetFitness();
        float worstFitness = _nets[0].GetFitness();
        for (int i = 0; i < _populationSize; ++i)
            avgFitness += _nets[i].GetFitness();
        avgFitness = avgFitness / _populationSize;
        avgFitness = avgFitness / (_maxFitness / 100);
        bestFitness = bestFitness / (_maxFitness / 100);
        worstFitness = worstFitness / (_maxFitness / 100);
        _canvas.transform.Find("Graph & Infos").Find("Best").GetComponent<Text>().text = BEST + bestFitness.ToString("0.00") + PERCENT;
        _canvas.transform.Find("Graph & Infos").Find("AVG").GetComponent<Text>().text = AVG + avgFitness.ToString("0.00") + PERCENT;
        _canvas.transform.Find("Graph & Infos").Find("Worst").GetComponent<Text>().text = WORST + worstFitness.ToString("0.00") + PERCENT;

        // Graph
        _avgFitnessList.Add(avgFitness);
        _bestFitnessList.Add(bestFitness);
        _worstFitnessList.Add(worstFitness);
        DrawGraph(_avgLineRenderer, _avgFitnessList);
        DrawGraph(_bestLineRenderer, _bestFitnessList);
        DrawGraph(_worstLineRenderer, _worstFitnessList);

        // Besr NN Display
        _bestNN = _nets[_populationSize - 1];
        
        // Best Weights
        for (int i = 0; i < _bestNN._weights.Length; ++i)
        {
            for (int y = 0; y < _bestNN._weights[i].Length; ++y)
            {
                    
                for (int k = 0; k < _bestNN._weights[i][y].Length; ++k)
                {
                    float w = _bestNN._weights[i][y][k];
                    Color color = Color.black;
                    if (w >= 0)
                        color = new Color(1f, 0f, 0f, w);
                    else
                        color = new Color(0f, 0f, 1f, -w);
                    Camera.main.transform.Find("Draw").Find("Best NN").Find("n[" + (i + 1) + "," + y + "]").Find("w[" + k + "]").GetComponent<LineRenderer>().material.color = color;
                }
            }
        }
    }

    void SwitchDisplayMode()
    {
        if (_displayMode == 0)
        {
            _displayMode = 1;

            _canvas.transform.Find("Graph & Infos").gameObject.SetActive(false);
            Camera.main.transform.Find("Draw").Find("Graph").gameObject.SetActive(false);
            
            _canvas.transform.Find("Best NN").gameObject.SetActive(true);
            Camera.main.transform.Find("Draw").Find("Best NN").gameObject.SetActive(true);
        }
        else if (_displayMode == 1)
        {
            _displayMode = 0;

            _canvas.transform.Find("Best NN").gameObject.SetActive(false);
            Camera.main.transform.Find("Draw").Find("Best NN").gameObject.SetActive(false);
            
            _canvas.transform.Find("Graph & Infos").gameObject.SetActive(true);
            Camera.main.transform.Find("Draw").Find("Graph").gameObject.SetActive(true);
        }
    }

    void Pause()
    {
        if (_start)
        {
            _start = false;
            _pause.transform.Find("Text").GetComponent<Text>().text = "Start Generation";
        }
        else
        {
            _start = true;
            _pause.transform.Find("Text").GetComponent<Text>().text = "Pause Generation";
        }
    }
}
