using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unsupervised Neural Network (NN)
public class NeuralNetwork : IComparable<NeuralNetwork>
{
    public int[] _layers;               // Layers
    public float[][] _neurons;          // Neuron Matix
    public float[][][] _weights;        // Weight Matrix
    private float _fitness;             // Fitness of the NN
    public Color _color;                // Color
    public bool _bias;                  // Bias


    #region Constructor ======================================================#

    /// <summary>
    /// Initilizes and NN with random Weights
    /// </summary>
    /// <param name="layers">NN Layers</param>
    public NeuralNetwork(int[] layers, Color color, bool bias = true)
    {
        // DeepCopy of Layers of this NN
        _layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; ++i)
        {
            _layers[i] = layers[i];
        }

        // Generate Matrix
        InitNeurons();
        InitWeights();

        // Color
        _color = color;
        _bias = bias;
    }

    /// <summary>
    /// DeepCopy Constructor 
    /// </summary>
    /// <param name="copyNetwork">NN to DeepCopy</param>
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        _layers = new int[copyNetwork._layers.Length];
        for (int i = 0; i < copyNetwork._layers.Length; i++)
        {
            _layers[i] = copyNetwork._layers[i];
        }

        // Generate Matrix
        InitNeurons();
        InitWeights();

        // Set Weights
        CopyWeights(copyNetwork._weights);

        // Color
        _color = copyNetwork._color;
        _bias = copyNetwork._bias;
    }

    #endregion

    #region Initialization ===================================================#

    /// <summary>
    /// Generate Neutron Matrix
    /// </summary>
    private void InitNeurons()
    {
        // Neuron Init
        List<float[]> neuronsList = new List<float[]>();

        // Run through all Layers
        for (int i = 0; i < _layers.Length; ++i)
        {
            // Add Layer to Neuron List
            neuronsList.Add(new float[_layers[i]]);
        }

        // Convert List to Array
        _neurons = neuronsList.ToArray();

    }

    /// <summary>
    /// Generate Weight Matrix
    /// </summary>
    private void InitWeights()
    {
        // Weight Init (will be converted to 3D Array)
        List<float[][]> weightsList = new List<float[][]>();

        // Itterate over all Neurons that have a Weight connection
        for (int i = 1; i < _layers.Length; i++)
        {
            // Layer Weight List for this current Layer (will be converted to 2D Array)
            List<float[]> layerWeightsList = new List<float[]>();

            int neuronsInPreviousLayer = _layers[i - 1];

            // Itterate over all Neurons in this current Layer
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                // Neurons Weights
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                // Itterate over all Neurons in the previous Layer and set the Weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // Give random Weights to Neuron Weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                // Add Neuron Weights of this current Layer to Layer Weights
                layerWeightsList.Add(neuronWeights);
            }

            // Add this Layers Weights converted into 2D Array into Weights List
            weightsList.Add(layerWeightsList.ToArray());
        }

        // Convert to 3D Array
        _weights = weightsList.ToArray();
    }

    /// <summary>
    /// Copy Weights
    /// </summary>
    /// <param name="copyWeights">Weights to Copy</param>
    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    _weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    #endregion

    #region Core =============================================================#

    /// <summary>
    /// Feed Forward (FF) this NN with a given input Array
    /// </summary>
    /// <param name="inputs">NN Inputs</param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        // Add inputs to the Neuron Matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            _neurons[0][i] = inputs[i];
        }

        // Itterate over all Neurons and compute FF values 
        for (int i = 1; i < _layers.Length; i++)
        {
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                float value = 0f;
                if (_bias)
                    value = 1f; // Add Bias here if needed

                for (int k = 0; k < _neurons[i - 1].Length; k++)
                {
                    // Sum off all Weights connections of this Neuron Weight values in their previous Layer
                    value += _weights[i - 1][j][k] * _neurons[i - 1][k];
                }

                // Hyperbolic Tangent activation
                _neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        // Return output Layer
        return _neurons[_neurons.Length - 1];
    }

    /// <summary>
    /// Mutate NN Weights
    /// </summary>
    public void Mutate()
    {
        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    float weight = _weights[i][j][k];

                    // Mutate Weight value 
                    float randomNumber = UnityEngine.Random.Range(0f, 100f);

                    if (randomNumber <= 2f) // 1
                    {
                        // Flip sign of Weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f) // 2
                    {
                        // Random Weight between -0.5 and 0.5
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)  // 3
                    {
                        // Randomly increase Weight by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNumber <= 8f) // 4
                    {
                        // Randomly decrease Weight by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    // Set mutated Weight
                    _weights[i][j][k] = weight;
                }
            }
        }
    }

    #region Fitness

    /// <summary>
    /// Add Fitness
    /// </summary>
    /// <param name="fit">Fitness to Add</param>
    public void AddFitness(float fit)
    {
        _fitness += fit;
    }

    /// <summary>
    /// Set Fitness
    /// </summary>
    /// <param name="fit">Fitness</param>
    public void SetFitness(float fit)
    {
        _fitness = fit;
    }

    /// <summary>
    /// Get Fitness
    /// </summary>
    public float GetFitness()
    {
        return _fitness;
    }

    /// <summary>
    /// Compare 2 NN and Sort based on Fitness
    /// </summary>
    /// <param name="other">NN to be compared to</param>
    /// <returns>1 == sup, -1 == inf, 0 == eq</returns>
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (_fitness > other._fitness)
            return 1;
        else if (_fitness < other._fitness)
            return -1;
        else
            return 0;
    }

    #endregion

    #endregion
}
