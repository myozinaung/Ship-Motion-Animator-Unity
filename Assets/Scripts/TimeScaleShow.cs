using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleShow : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TimeScaleText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeScaleText.text = "x " + Time.timeScale.ToString();
    }
}
