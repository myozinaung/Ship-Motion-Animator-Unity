using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipInfoShow : MonoBehaviour 
{
    public Transform nameLable;
    public Camera myCamera;

    void Start() 
    {
       // myCamera = Camera.main; 
    }
 
 // Update is called once per frame
    void Update () 
    {
        Vector3 namePose = myCamera.WorldToScreenPoint(this.transform.position);
        nameLable.transform.position = namePose;
    }
}