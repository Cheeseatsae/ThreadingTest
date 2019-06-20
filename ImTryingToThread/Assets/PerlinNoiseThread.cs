using System.Threading;
using UnityEngine;

public class PerlinNoiseThread : MonoBehaviour
{
    private Texture2D tex;
    
    
    public int xNum;
    public int yNum;
    public float scale;
    
    struct v3
    {
        public float r;
        public float g;
        public float b;
    }
    v3[,] texture;
    
    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(xNum, yNum, TextureFormat.RGB24, false);
        
        Thread Yes = new Thread(PerlinNoise);
        Yes.Start();
        
        PerlinNoise();
        SetPerlinTex();
        
    }

    private void PerlinNoise()
    {
        Debug.Log("Started");
        texture = new v3[xNum,yNum];
        
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                float p = Mathf.PerlinNoise(x * scale, y * scale);
                texture[x, y].r = p;
                texture[x, y].g = p;
                texture[x, y].b = p;
            }
        }
        
    }

    private void SetPerlinTex()
    {

        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                tex.SetPixel(x,y,new Color(texture[x,y].r, texture[x,y].g, texture[x,y].b));
            }
        }
        tex.Apply();
        GetComponent<Renderer>().material.mainTexture = tex;
    }
}
