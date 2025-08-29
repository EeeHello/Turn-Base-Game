using System;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public List<float> weights;
    public float bias;

    public Neuron(int inputCount, System.Random rng)
    {
        weights = new List<float>();
        for (int i = 0; i < inputCount; i++)
            weights.Add((float)(rng.NextDouble() * 2 - 1)); // random -1..1
        bias = (float)(rng.NextDouble() * 2 - 1);
    }

    private float Sigmoid(float x) => 1f / (1f + Mathf.Exp(-x));

    public float Activate(List<float> inputs)
    {
        float sum = 0f;
        for (int i = 0; i < inputs.Count; i++)
            sum += inputs[i] * weights[i];
        sum += bias; // offset
        return Sigmoid(sum);
    }
}