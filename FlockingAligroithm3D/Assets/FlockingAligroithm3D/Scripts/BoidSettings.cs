using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidSettings")]
public class BoidSettings : ScriptableObject
{

    public float movingSpeed = 5;
    public float neighborRadius = 5;
    public float smoothDamp = 0.5f;
    public float avoidanceRadius = 4f;
    public float circleRadius = 30;
    public float obstcleDefectDist = 3f;

    public float cohesionWeight = 1;
    public float aligmentWeight = 1;
    public float avoidanceWeight = 1;
    public float obstlceWeight = 5;
    public bool useSameAgentFilter = true;
    public LayerMask obstcleMask;   
}
