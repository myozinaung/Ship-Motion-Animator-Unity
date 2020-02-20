using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveOperatingPoint : MonoBehaviour
{
    public Transform N_PE;
    public Transform OPoint;
    public LineRenderer LoadDiagram;
    public Transform cam;
    public TMPro.TextMeshPro PEValue;
    public TMPro.TextMeshPro RPMValue;

    float[] PE_Points = {5286.8f, 7668.4f, 7930, 7930, 5286.8f};
    float[] N_Points = {110, 132.479f, 137, 143.85f, 143.85f};
    Vector3[] positions = new Vector3[5];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++) {
            positions[i] = new Vector3((N_Points[i]-110)/(33.85f/4)+cam.position.x-2, (PE_Points[i]-5286.8f)/(2643.2f/4)+cam.position.y-2, 0);
        }
        LoadDiagram.SetVertexCount(5);
        LoadDiagram.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        float N = (N_PE.position.x-110)/(33.85f/4)-2;
        float PE =(N_PE.position.y-5286.8f)/(2643.2f/4)-2;
        OPoint.localPosition = new Vector3(N,PE,10);
        PEValue.text = N_PE.position.y.ToString("F0") + " kW";
        RPMValue.text = N_PE.position.x.ToString("F0") + " rpm";
    }
}
