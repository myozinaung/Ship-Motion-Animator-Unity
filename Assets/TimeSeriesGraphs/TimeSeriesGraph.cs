using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TimeSeriesGraph : MonoBehaviour
{
  public Transform Ship;
  [Range(1,6)]
  public int dataSelect = 1;
  public string dataLabel = "Data";
  public Transform cameraPos;
    
  public TMPro.TextMeshPro LabelText;
  public TMPro.TextMeshPro ValueText;
  public TMPro.TextMeshPro Axis0Text;
  public TMPro.TextMeshPro AxisUBText;
  public TMPro.TextMeshPro AxisLBText;

  
  public LineRenderer lineRenderer;
  public LineRenderer Axis0;
  public LineRenderer AxisUB;
  public LineRenderer AxisLB;
  
  public LineRenderer AxisU1;
  public LineRenderer AxisL1;
  public float yUB = 1;
  public float yLB = -1;

  public int lineVertexCount = 100;
  public float timeRange = 2f;

  private float space = 0.4f;

    
    
  Queue<float> dataQueue = new Queue<float>();
  
  void Start() {

    float yMid = (yUB + yLB)/2;

    LabelText.text = dataLabel;
    
    // For Axis
    Axis0.SetVertexCount(2);
    Axis0.SetPosition(0, new Vector3(cameraPos.position.x - timeRange/2 + space, cameraPos.position.y,0));
    Axis0.SetPosition(1, new Vector3(cameraPos.position.x + timeRange/2, cameraPos.position.y,0));
    Axis0Text.text = yMid.ToString();;
    
    AxisUBText.text = yUB.ToString();
    AxisUB.SetVertexCount(2);
    AxisUB.SetPosition(0, new Vector3(cameraPos.position.x - timeRange/2 + space, cameraPos.position.y + 1, 0));
    AxisUB.SetPosition(1, new Vector3(cameraPos.position.x + timeRange/2  - 2.5f, cameraPos.position.y + 1, 0));

    AxisLBText.text = yLB.ToString();
    AxisLB.SetVertexCount(2);
    AxisLB.SetPosition(0, new Vector3(cameraPos.position.x - timeRange/2 + space, cameraPos.position.y - 1, 0));
    AxisLB.SetPosition(1, new Vector3(cameraPos.position.x + timeRange/2, cameraPos.position.y - 1, 0));

    AxisU1.SetVertexCount(2);
    AxisU1.SetPosition(0, new Vector3(cameraPos.position.x - timeRange/2 + space, cameraPos.position.y + 0.5f, 0));
    AxisU1.SetPosition(1, new Vector3(cameraPos.position.x + timeRange/2, cameraPos.position.y + 0.5f, 0));

    AxisL1.SetVertexCount(2);
    AxisL1.SetPosition(0, new Vector3(cameraPos.position.x - timeRange/2 + space, cameraPos.position.y - 0.5f, 0));
    AxisL1.SetPosition(1, new Vector3(cameraPos.position.x + timeRange/2, cameraPos.position.y - 0.5f, 0));

    for (int i = 0; i < lineVertexCount; i++) { // initialize queue with zero
      dataQueue.Enqueue(0f);
    }

    
  }
  
  // void OnRenderObject() // Original function name
  void FixedUpdate() 
  {


    // For Timeseries Graph
    lineRenderer.SetVertexCount(lineVertexCount);
    Vector3[] positions = new Vector3[lineVertexCount];
        
    // MAIN Equation to Change Data
    float data = 0;
    switch (dataSelect) {
      case 1: // x_pos (Surge)
       data = Ship.position.x;
       break;
      case 2: // y_pos (Sway)
       data = Ship.position.z;
       break;
      case 3: // z_pos (Heave)
       data = Ship.position.y;
       break;
      case 4: // Roll
        data = Mathf.Deg2Rad*Ship.eulerAngles.x;
        data = Mathf.Rad2Deg*((data+Mathf.Sign(data)*Mathf.PI)%(2*Mathf.PI) - Mathf.Sign(data)*Mathf.PI); // Rewrite within -pi to pi
        break;
      case 5: // Pitch
        data = Mathf.Deg2Rad*Ship.eulerAngles.z;
        data = Mathf.Rad2Deg*((data+Mathf.Sign(data)*Mathf.PI)%(2*Mathf.PI) - Mathf.Sign(data)*Mathf.PI); // Rewrite within -pi to pi
        break;
      case 6: // Yaw
        data = -Mathf.Deg2Rad*Ship.eulerAngles.y;
        data = Mathf.Rad2Deg*((data+Mathf.Sign(data)*Mathf.PI)%(2*Mathf.PI) - Mathf.Sign(data)*Mathf.PI); // Rewrite within -pi to pi
        break;
    }

    float yMid = (yUB + yLB)/2;
    float yRange = yUB - yLB;
    dataQueue.Enqueue((data-yMid)/(yRange/2));
    if (dataQueue.Count >= lineVertexCount) {
      dataQueue.Dequeue();
    }

    float[] dataArray = new float[dataQueue.Count];
    dataQueue.CopyTo(dataArray, 0);

    float dt = timeRange / (lineVertexCount - 1);
    for (int i = 0; i < lineVertexCount; i++) {
      float time = space + dt * i*0.9f;
      float x = time - timeRange/2  + cameraPos.position.x;
        
      positions[i] = new Vector3(x,dataArray[i],0);
    }
    lineRenderer.SetPositions(positions);
    ValueText.text = data.ToString("F2");
    

  }

}
