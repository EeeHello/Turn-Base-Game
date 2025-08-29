using System;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    private Layer hidden;
    private Layer output;

    public ANN(int inputCount, int hiddenCount, int outputCount, int seed = 42)
    {
        System.Random rng = new System.Random(seed);
        hidden = new Layer(hiddenCount, inputCount, rng);
        output = new Layer(outputCount, hiddenCount, rng);
    }

    public List<float> Forward(List<float> inputs)
    {
        var hiddenOut = hidden.Forward(inputs);
        var rawOutputs = output.Forward(hiddenOut);

        return Softmax(rawOutputs);
    }
    private List<float> Softmax(List<float> values)
    {
        List<float> result = new List<float>(values.Count);

        // stabilize by subtracting max (avoids overflow in exp)
        float maxVal = float.MinValue;
        foreach (float v in values)
            if (v > maxVal) maxVal = v;

        float sumExp = 0f;
        List<float> expVals = new List<float>(values.Count);
        foreach (float v in values)
        {
            float e = Mathf.Exp(v - maxVal);
            expVals.Add(e);
            sumExp += e;
        }

        foreach (float e in expVals)
            result.Add(e / sumExp);

        return result;
    }
}