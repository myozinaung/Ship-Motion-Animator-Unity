using UnityEngine;

public class OceanAdvanced2 : MonoBehaviour
{

  public Material ocean;
  public Light sun;

  void Awake()
  {
    // Load the wave data generated
    string input = System.IO.File.ReadAllText ("wave.csv");
    string[] lines = input.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    
    Vector4[] v_waves = new Vector4[lines.Length];
    Vector4[] v_waves_dir = new Vector4[lines.Length]; 
    
    for (int i = 0; i < lines.Length; i++) 
    {
      string[] wave_para = lines[i].Split(new[] { ',' });
      float Omega;    float.TryParse (wave_para[0], out Omega);
      float Zeta_a;   float.TryParse (wave_para[1], out Zeta_a);
      float Wavenum;  float.TryParse (wave_para[2], out Wavenum);
      float Phase;    float.TryParse (wave_para[3], out Phase);
      float Psi;      float.TryParse (wave_para[4], out Psi);
      v_waves[i] = new Vector4(Omega, Zeta_a, Wavenum, Phase);
      v_waves_dir[i] = new Vector4(Mathf.Cos(Psi), Mathf.Sin(Psi), 0, 0);
    }
 
    // Send wave parameters to the "ocean" material shader
    ocean.SetVectorArray("waves_p", v_waves);
    ocean.SetVectorArray("waves_d", v_waves_dir);
    ocean.SetVector("world_light_dir", -sun.transform.forward);
	  // ocean.SetInt("No_WAVE", lines.Length);  
    
  }

  void FixedUpdate()
  {
    ocean.SetVector("world_light_dir", -sun.transform.forward);
    ocean.SetVector("sun_color", new Vector4(sun.color.r, sun.color.g, sun.color.b, 0.0F));
  }

}
