using UnityEngine;

public class WaveSpectrumPlot : MonoBehaviour
{
    public string dataPath = "spectrum.csv";
    public Transform cam;
    public LineRenderer spectrum;
    public float S_scale = 0.6f;
    public float Omega_scale = 1.0f;
   
    // Start is called before the first frame update
    void Start()
    {
        LoadData(dataPath);

    }


     private void LoadData(string filePath) {
         
         string input = System.IO.File.ReadAllText (filePath);
         string[] lines = input.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
 
         Vector3[] positions = new Vector3[lines.Length-2];
         for (int i = 1; i < lines.Length-1; i++) {
             string[] nums = lines[i].Split(new[] { ',' });
             if (nums.Length < 2) {
                 Debug.Log ("Misforned input on line "+i+1);
             }
             float Omega_plot;
			 float S_plot;
             
             if (float.TryParse (nums[0], out Omega_plot)) {
				 if (float.TryParse (nums[1], out S_plot)) {
                     positions[i-1] = new Vector3(Omega_plot*Omega_scale+cam.position.x-4, S_plot*S_scale+cam.position.y-2.5f, 0);

				 }
		 
             }
         }
         spectrum.SetVertexCount(lines.Length-2);
         spectrum.SetPositions(positions);
     }
    // Update is called once per frame
    void Update()
    {
        
    }
}
