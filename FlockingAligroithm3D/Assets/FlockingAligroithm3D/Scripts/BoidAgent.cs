using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAgent : MonoBehaviour
{
    public Boid boid { get => m_Boid; private set => m_Boid = value; }

    public BoidSettings boidSettings;
    public Vector3 currentVelocity;
    public Vector3 currentDir;
    public Vector3 center = Vector3.zero;

    public Transform followTarget;
    public Collider collider;
    float m_CircleRange;
    private Boid m_Boid;

    public void Initialize(Boid boid, float circleRange, Transform followTarget)
    {
        this.boid = boid;
        m_CircleRange = boidSettings.circleRadius;
        this.followTarget = followTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cohesionVec = CalculateCohesion() * boidSettings.cohesionWeight;
        Vector3 aligmentVec = CalculateAligment() * boidSettings.aligmentWeight;
        Vector3 avoidanceVec = CalculateAvoidance() * boidSettings.avoidanceWeight;
        Vector3 circleRadiusVec = CalculateCircleRadius();
        Vector3 obstlceVec = ObstacleObjectFilter() * boidSettings.obstcleDefectDist;

        Vector3 velocity = followTarget != null ? (cohesionVec + aligmentVec + avoidanceVec + obstlceVec) : (cohesionVec + aligmentVec + avoidanceVec + obstlceVec + circleRadiusVec);

        float time = Time.deltaTime * boidSettings.movingSpeed;
        Vector3 dir = velocity.normalized;        
        if(velocity == Vector3.zero)
        {
            velocity = transform.forward * boidSettings.movingSpeed;
            dir = transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(dir);
        if(followTarget != null)
        {   
            transform.position += velocity * time;
            transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime);
        }
        else
        {
            transform.position += velocity * time;
        }
    }

    Vector3 ObstacleObjectFilter()
    {
        LayerMask obstcle = boidSettings.obstcleMask;
        float dist = boidSettings.obstcleDefectDist;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, dist, obstcle))
        {
            return hit.normal;
        }
        else
        {
            return Vector3.zero;
        }
    }

    List<Transform> SameAgentFilter()
    {
        List<Transform> boidAgents = GetNearNeighbor();
        List<Transform> sameAgents = new List<Transform>();

        if(boidAgents.Count == 0)
        {
            return sameAgents;
        }

       for(int i = 0; i < boidAgents.Count; i++)
        {
            BoidAgent b = boidAgents[i].GetComponentInParent<BoidAgent>();
            if(b != null && b.boid == boid)
            {
                sameAgents.Add(b.transform);
            }
        }

        return sameAgents;
    }

    Vector3 CalculateCircleRadius()
    {
        Vector3 centerToOffset = center - transform.position;
        Vector3 value = centerToOffset.sqrMagnitude > m_CircleRange * m_CircleRange ? centerToOffset.normalized : Vector3.zero;
        return value;
    }

    Vector3 CalculateAvoidance()
    {
        List<Transform> boidAgents = boidSettings.useSameAgentFilter ? SameAgentFilter() : GetNearNeighbor();
        if(boidAgents.Count == 0)
        {
            return transform.forward;
        }

        Vector3 avoidMove = Vector3.zero;
        int avoidCnt = 0;
        for(int i = 0; i < boidAgents.Count; i++)
        {
            Vector3 offset = (transform.position - boidAgents[i].position);
            if(offset.sqrMagnitude < boidSettings.avoidanceRadius)
            {
                avoidCnt++;
                avoidMove += offset;
            }
        }

        if(avoidCnt > 0)
        {
            avoidMove /= avoidCnt;
        }

        avoidMove.Normalize();
        return avoidMove;
    }

    Vector3 CalculateAligment()
    {
        List<Transform> boidAgents = boidSettings.useSameAgentFilter ? SameAgentFilter() : GetNearNeighbor();
        if(boidAgents.Count == 0)
        {
            return transform.forward;
        }

        Vector3 aligmentMove = Vector3.zero;
        for(int i = 0; i < boidAgents.Count; i++)
        {
            aligmentMove += boidAgents[i].forward;
        }

        aligmentMove /= boidAgents.Count;
        aligmentMove.Normalize();
        return aligmentMove;
    }

    Vector3 CalculateCohesion()
    {
        List<Transform> boidAgents = GetNearNeighbor();
        if(boidAgents.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesionMove = Vector3.zero;
        for(int i = 0; i < boidAgents.Count; i++)
        {
            cohesionMove += boidAgents[i].position;
        }

        cohesionMove /= boidAgents.Count;
        cohesionMove -= transform.position;
        cohesionMove = Vector3.SmoothDamp(transform.forward, cohesionMove, ref currentVelocity, boidSettings.smoothDamp, boidSettings.movingSpeed);
        cohesionMove.Normalize();
        return cohesionMove;
    }

    List<Transform> GetNearNeighbor()
    {
        List<Transform> neighbors = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, boidSettings.neighborRadius);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(transform != colliders[i].transform)
            {
                neighbors.Add(colliders[i].transform);
            }
        }

        return neighbors;
    }

}
