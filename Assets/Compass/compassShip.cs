using UnityEngine;
using System.Collections;

public class compassShip : MonoBehaviour {
    public Transform player;
    public Texture compBg;
    public Texture blipTex;

    void OnGUI(){
        GUI.DrawTexture(new Rect(0, 0, 200, 200), compBg);
        GUI.DrawTexture(CreateBlip(), blipTex);
    }

    Rect CreateBlip(){
        float angDeg = player.eulerAngles.y - 90;
        float angRed = angDeg * Mathf.Deg2Rad;

        float blipX = 25 * Mathf.Cos(angRed);
        float blipY = 25 * Mathf.Sin(angRed);

        blipX += 55;
        blipY += 55;

        return new Rect(blipX, blipY, 10, 10);
    }
}