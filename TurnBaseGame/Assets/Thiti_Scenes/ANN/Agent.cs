//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
//public class Agent : MonoBehaviour
//{
//    public ANN brain;
//    public float fitness = 0f;

//    public void InitRandomBrain()
//    {
//        brain = new ANN(3, 4, 3); // inputs=3, hidden=4, outputs=3
//    }

//    public void InitFromParents(ANN parent1, ANN parent2, float mutationRate)
//    {
//        brain = parent1.Clone(); // implement a Clone() in ANN
//        brain.Crossover(parent2); // implement simple weight crossover
//        brain.Mutate(mutationRate); // small random mutations
//    }

//    // Example placeholder to update fitness, called by your AI logic
//    public void UpdateFitness(float amount)
//    {
//        fitness += amount;
//    }
//}
