using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCompass : MonoBehaviour
{
    [Range(-180,180)]
    public float psiRef = 0;
    public Transform Ship;
    public Transform ShipRef;
    public Transform ShipComp;
    public TMPro.TextMeshPro CompassHeading;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShipRef.eulerAngles = new Vector3(0,0,psiRef);
        float psi = -Ship.eulerAngles.y;
        ShipComp.eulerAngles = new Vector3(0,0,psi);
        psi = psi*Mathf.Deg2Rad;
        psi = Mathf.Rad2Deg*((psi+Mathf.Sign(psi)*Mathf.PI)%(2*Mathf.PI) - Mathf.Sign(psi)*Mathf.PI); // Rewrite within -pi to pi
        CompassHeading.text = psi.ToString("F1") + "°";
    }
    
}
