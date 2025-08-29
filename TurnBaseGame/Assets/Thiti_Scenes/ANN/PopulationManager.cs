//using System.Collections.Generic;
//using UnityEngine;

//public class PopulationManager : MonoBehaviour
//{
//    public GameObject agentPrefab;
//    public int populationSize = 10;
//    public float mutationRate = 0.1f;
//    public Transform spawnArea;
//    public float generationTime = 10f;

//    private float timer = 0f;
//    private int generation = 1;
//    private List<Agent> agents = new List<Agent>();

//    void Start()
//    {
//        SpawnInitialPopulation();
//    }

//    void Update()
//    {
//        timer += Time.deltaTime;
//        if (timer >= generationTime)
//        {
//            NextGeneration();
//            timer = 0f;
//        }
//    }

//    private void SpawnInitialPopulation()
//    {
//        for (int i = 0; i < populationSize; i++)
//        {
//            GameObject obj = Instantiate(agentPrefab, RandomPosition(), Quaternion.identity, spawnArea);
//            Agent agent = obj.GetComponent<Agent>();
//            agent.InitRandomBrain();
//            agents.Add(agent);
//        }
//        Debug.Log($"Generation {generation} spawned.");
//    }

//    private void NextGeneration()
//    {
//        generation++;
//        Debug.Log($"Generation {generation} starting...");

//        agents.Sort((a, b) => b.fitness.CompareTo(a.fitness));
//        int survivors = populationSize / 2;
//        List<Agent> newAgents = new List<Agent>();

//        for (int i = 0; i < survivors; i++)
//        {
//            newAgents.Add(agents[i]);
//            int mateIndex = Random.Range(0, survivors);
//            Agent childAgent = Instantiate(agentPrefab, RandomPosition(), Quaternion.identity, spawnArea)
//                                .GetComponent<Agent>();
//            childAgent.InitFromParents(agents[i].brain, agents[mateIndex].brain, mutationRate);
//            newAgents.Add(childAgent);
//        }

//        for (int i = survivors; i < agents.Count; i++)
//            if (agents[i] != null) Destroy(agents[i].game
