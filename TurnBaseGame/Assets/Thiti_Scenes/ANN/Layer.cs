using System;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public List<Neuron> neurons;

    public Layer(int neuronCount, int inputCount, System.Random rng)
    {
        neurons = new List<Neuron>();
        for (int i = 0; i < neuronCount; i++)
            neurons.Add(new Neuron(inputCount, rng));
    }

    public List<float> Forward(List<float> inputs)
    {
        List<float> outputs = new List<float>();
        foreach (var n in neurons)
            outputs.Add(n.Activate(inputs));
        return outputs;
    }
}