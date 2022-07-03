using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidAgent boidAgentPrefab;
    public GameObject followTargetPrefab;

    [Range(0, 1000)] public int createAgentCount = 250;
    [Range(0, 1)] public float createAgentDensity = 0.3f;
    
    public List<BoidAgent> boidAgents = new List<BoidAgent>();

    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < createAgentCount; i++)
        {
            BoidAgent agent = Instantiate<BoidAgent>(boidAgentPrefab,
                    Random.insideUnitSphere * createAgentCount * createAgentDensity,
                    Quaternion.Euler(new Vector3(Random.Range(-90, 90), 0, 0)),
                    transform);
        
            boidAgents.Add(agent);
            agent.Initialize(this, createAgentCount, followTargetPrefab != null ? followTargetPrefab.transform : null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
