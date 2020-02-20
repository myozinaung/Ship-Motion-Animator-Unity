using UnityEngine; 
using System.Collections;

public class rotateRudder : MonoBehaviour {
	// AnimationCurve to smooth motion
    public Transform rudder;
    public Transform delta;
    private AnimationCurve ac_angleX = new AnimationCurve();

    private float maxTime;
 
     void Start() {
         LoadData("delta.csv");
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

             if (float.TryParse (nums[0], out timestamp)) {
				 if (float.TryParse (nums[1], out angleX)) {
                     ac_angleX.AddKey (timestamp, angleX);
                     if (timestamp > maxTime)
                         maxTime = timestamp;
				 }
		 
             }
         }
     }
 
     private IEnumerator DoTheRocking(bool repeat) {
         do {
             float time = 0.0f;
			 float time_old = 0.0f;
             while (time <= maxTime) {
//               transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
//				 transform.rotation = Quaternion.Euler(0.0f,ac_angleX.Evaluate (time), 0.0f);
				 rudder.Rotate(Vector3.up, ac_angleX.Evaluate (time)-ac_angleX.Evaluate (time_old), Space.Self);
                 delta.eulerAngles = new Vector3(ac_angleX.Evaluate (time),0,0);
                 yield return null;
				 time_old = time;
                 time += Time.deltaTime;
             }
         } while (repeat);
     }
 }