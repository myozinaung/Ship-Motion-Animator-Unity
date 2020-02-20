using UnityEngine; 
using System.Collections;

public class rotateProp : MonoBehaviour {
	// AnimationCurve to smooth motion

    public Transform propeller;
    public Transform N_PE_EP;
    private AnimationCurve ac_angleX = new AnimationCurve(); 
    private AnimationCurve ac_angleY = new AnimationCurve(); 
    private AnimationCurve ac_angleZ = new AnimationCurve(); 
    private float maxTime;
 
     void Start() {
         LoadData("rpm.csv");
         StartCoroutine(DoTheRocking(true));
     }
         
     private void LoadData(string filePath) {
         
         string input = System.IO.File.ReadAllText (filePath);
         string[] lines = input.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
 
         for (int i = 0; i < lines.Length; i++) {
             string[] nums = lines[i].Split(new[] { ',' });
             if (nums.Length < 2) {
                 Debug.Log ("Misforned input on line "+i+1);
             }
             float timestamp;
			 float angleX;
             float angleY;
             float angleZ;
			 
             if (float.TryParse (nums[0], out timestamp)) {
				 if (float.TryParse (nums[1], out angleX)) {
                     ac_angleX.AddKey (timestamp, angleX);
                     if (timestamp > maxTime)
                         maxTime = timestamp;
				 }

                 if (float.TryParse (nums[2], out angleY)) {
                     ac_angleY.AddKey (timestamp, angleY);
                 }

                 if (float.TryParse (nums[4], out angleZ)) {
                     ac_angleZ.AddKey (timestamp, angleZ);
                 }
             }
         }
     }
 
     private IEnumerator DoTheRocking(bool repeat) {
         do {
             float time = 0.0f;
             while (time <= maxTime) {
//                 transform.eulerAngles = new Vector3(ac_angleX.Evaluate (time), 0.0f, 0.0f);
				 
                //  propeller.Rotate(ac_angleX.Evaluate (time)*6f*Time.deltaTime*1f, 0.0f, 0.0f,  Space.Self);
                 propeller.Rotate(ac_angleX.Evaluate (time)*6f*0.02f*1f, 0.0f, 0.0f,  Space.Self);
//				 transform.rotation = Quaternion.Euler(0,0,ac_angleX.Evaluate (time));
//				 transform.position = new Vector3 (0.0f,0.0f,0.0f);
                 N_PE_EP.position = new Vector3(ac_angleX.Evaluate (time),ac_angleY.Evaluate (time),ac_angleZ.Evaluate (time));
                 yield return null;
                 time += Time.deltaTime;
             }
         } while (repeat);
     }
 }