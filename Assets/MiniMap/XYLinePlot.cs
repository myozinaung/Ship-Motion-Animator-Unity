using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYLinePlot : MonoBehaviour
{
    public Transform Ship;
    public LineRenderer ShipPath;
    // Start is called before the first frame update
    Queue<Vector3> posQueue = new Queue<Vector3>();
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        posQueue.Enqueue(Ship.position);
        Vector3[] posArray = new Vector3[posQueue.Count];
        posQueue.CopyTo(posArray,0);
        ShipPath.SetVertexCount(posQueue.Count);
        ShipPath.SetPositions(posArray);
    }
}
