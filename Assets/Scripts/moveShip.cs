using UnityEngine; 
using System.Collections;

public class moveShip : MonoBehaviour {
	// AnimationCurve to smooth motion
    public string csvPath = "eta.csv";
    public Transform ship;
    
	private AnimationCurve ac_posX = new AnimationCurve(); // X
	private AnimationCurve ac_posY = new AnimationCurve(); // Y
	private AnimationCurve ac_posZ = new AnimationCurve(); // Z
	
    private AnimationCurve ac_angleX = new AnimationCurve(); // Roll
    private AnimationCurve ac_angleY = new AnimationCurve(); // Yaw
    private AnimationCurve ac_angleZ = new AnimationCurve(); // Ptich
	
    private float maxTime;
 
     void Start() {
         LoadData(csvPath);
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
			 
			 float posX;
			 float posY;
			 float posZ;
			 
             float angleX;
			 float angleY;
			 float angleZ;
			 
             if (float.TryParse (nums[0], out timestamp)) {
				 if (float.TryParse (nums[1], out posX)) {
                     ac_posX.AddKey (timestamp, posX);
                     if (timestamp > maxTime)
                         maxTime = timestamp;
				 }
				 if (float.TryParse (nums[3], out posY)) {
                     ac_posY.AddKey (timestamp, posY);
				 }				 
				 if (float.TryParse (nums[2], out posZ)) {
                     ac_posZ.AddKey (timestamp, posZ);
				 }				 
				 
                 if (float.TryParse (nums[4], out angleX)) {
                     ac_angleX.AddKey (timestamp, angleX);
                 }
                 if (float.TryParse (nums[6], out angleY)) {
                     ac_angleY.AddKey (timestamp, -angleY);
                 }
                 if (float.TryParse (nums[5], out angleZ)) {
                     ac_angleZ.AddKey (timestamp, angleZ);
                 }				 
             }
         }
     }
 
     private IEnumerator DoTheRocking(bool repeat) {
         do {
             float time = 0.0f;
             while (time <= maxTime) {
                 ship.eulerAngles = new Vector3(ac_angleX.Evaluate (time), ac_angleY.Evaluate (time), ac_angleZ.Evaluate (time));
				 ship.position = new Vector3 (ac_posX.Evaluate (time),ac_posY.Evaluate (time),ac_posZ.Evaluate (time));
                 yield return null;
                 time += Time.deltaTime;
             }
         } while (repeat);
     }
 }