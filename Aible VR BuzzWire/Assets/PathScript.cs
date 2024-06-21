using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation; // You'll need to include this namespace

public class PathScript : MonoBehaviour
{

    // This needs to be assigned to in the inspector
    public PathCreator pathCreator;

    public static List<Vector3> pathpoints = new List<Vector3>();

    public Transform[] waypoints;

    public GameObject prefab;
    public GameObject holder;
    public float spacing = 1;

    const float minSpacing = .025f;

    // Start is called before the first frame update
    void Start()
    {
        // You can now access the vertex path with pathCreator.path
        // For example, this sets the position to the middle of the path:
        //transform.position = pathCreator.path.GetPoint(1);

        pathpoints.Clear();
        Vector3 posUser = new Vector3(0.0f, 0.0f, 0.0f);

        float xOfs = Random.Range(-1.5f, 1.5f);
        float zOfs = Random.Range(-2.0f, 1.0f);
        float yOfs = Random.Range(1.0f, 4.0f);

        Vector3 startPos0 = new Vector3(posUser.x, posUser.y, posUser.z);

         xOfs = Random.Range(-0.0f, 0.1f);
         zOfs = Random.Range(-0.0f, 0.2f);
         yOfs = Random.Range(-0.1f, 0.1f);

        Vector3 startPos1 = new Vector3((waypoints[0].position.x + waypoints[1].position.x) / 2 -xOfs, (waypoints[0].position.y + waypoints[1].position.y) / 2 - yOfs, (waypoints[0].position.z + waypoints[1].position.z) / 2);


        xOfs = Random.Range(-1.5f, 1.5f);
        zOfs = Random.Range(-0.0f, 0.2f);
        yOfs = Random.Range(-0.0f, 0.1f);

        Vector3 startPos2 = new Vector3((waypoints[0].position.x + waypoints[1].position.x) / 2, (waypoints[0].position.y + waypoints[1].position.y) / 2 + 0.05f, (waypoints[0].position.z + waypoints[1].position.z) / 2);

        xOfs = Random.Range(-0.0f, 0.1f);
        zOfs = Random.Range(-0.0f, 0.2f);
        yOfs = Random.Range(-0.1f, 0.1f);

        Vector3 startPos3 = new Vector3((waypoints[0].position.x + waypoints[1].position.x) / 2 + xOfs, (waypoints[0].position.y + waypoints[1].position.y) / 2 + yOfs, (waypoints[0].position.z + waypoints[1].position.z) / 2);


        pathpoints.Add(waypoints[0].position);
        pathpoints.Add(startPos1);
        //pathpoints.Add(startPos2);
        //pathpoints.Add(startPos3);
        pathpoints.Add(waypoints[1].position);

        


        pathCreator.bezierPath = new BezierPath(pathpoints, false, PathSpace.xyz);
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Aligned;

        if (pathCreator != null)
        {
            Generate();
            GetObjectsPos();
        }


    }

   

    // Update is called once per frame
    void Update()
    {

        

    }

    

    void Generate()
    {
        if (pathCreator != null && prefab != null && holder != null)
        {
            DestroyObjects();

            VertexPath path = pathCreator.path;

            spacing = Mathf.Max(minSpacing, spacing);
            float dst = 0;

            while (dst < path.length)
            {
                Vector3 point = path.GetPointAtDistance(dst);
                Quaternion rot = path.GetRotationAtDistance(dst);
                Instantiate(prefab, point, rot, holder.transform);
                dst += spacing;
            }
        }
    }

    void DestroyObjects()
    {
        int numChildren = holder.transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
        }
    }

    void GetObjectsPos()
    {
        int numChildren = holder.transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            Vector3 nxtPos = holder.transform.GetChild(i).position;
            //Debug.Log(nxtPos);

            Quaternion nxtQ = holder.transform.GetChild(i).rotation;
            //Debug.Log(nxtQ);
        }
    }

    //void OnDrawGizmos()
    //{
    //    float xMin = -0.05f;
    //    float yMin = -0.23f;
    //    float zMin = 0.2f;

    //    float xMax = 0.45f;
    //    float yMax = 0.23f;
    //    float zMax = 0.45f;



    //    Gizmos.color = Color.yellow;


    //    // Now we can simply calculate our 8 vertices of the bounding box
    //    Vector3 A = new Vector3(xMin, yMin, zMin);
    //    Vector3 B = new Vector3(xMin, yMin, zMax);
    //    Vector3 C = new Vector3(xMin, yMax, zMin);
    //    Vector3 D = new Vector3(xMin, yMax, zMax);

    //    Vector3 E = new Vector3(xMax, yMin, zMin);
    //    Vector3 F = new Vector3(xMax, yMin, zMax);
    //    Vector3 G = new Vector3(xMax, yMax, zMin);
    //    Vector3 H = new Vector3(xMax, yMax, zMax);


    //    // And finally visualize it
    //    Gizmos.DrawLine(A, B);
    //    Gizmos.DrawLine(B, D);
    //    Gizmos.DrawLine(D, C);
    //    Gizmos.DrawLine(C, A);

    //    Gizmos.DrawLine(E, F);
    //    Gizmos.DrawLine(F, H);
    //    Gizmos.DrawLine(H, G);
    //    Gizmos.DrawLine(G, E);

    //    Gizmos.DrawLine(A, E);
    //    Gizmos.DrawLine(B, F);
    //    Gizmos.DrawLine(D, H);
    //    Gizmos.DrawLine(C, G);

    //}


}
