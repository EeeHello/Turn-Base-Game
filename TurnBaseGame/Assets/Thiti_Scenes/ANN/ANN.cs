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
        return output.Forward(hiddenOut);
    }
}