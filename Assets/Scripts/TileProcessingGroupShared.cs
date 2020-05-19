// By Olli S.

using UnityEngine;

public class TileProcessingGroupShared : MonoBehaviour
{
    public ComputeShader compute;

    [Header("Textures")]
    [SerializeField] Texture2D sourceTexture;
    RenderTexture resultTexture = null;

    int kernel_tiles;

    int propSourceTexture = Shader.PropertyToID("SourceTexture");
    int propResultTexture = Shader.PropertyToID("ResultTexture");

    int textureSize;
    [Range(1, 5)] [SerializeField] int kernelSize = 1;

    [Header("Visualize threads")]
    public bool VisualizeDispatchThreadID;
    public bool VisualizeGroupID;
    public bool VisualizeGroupThreadID;
    public bool VisualizeGroupIndex;

    [Header("Debugs")]
    [SerializeField] int count;
    const int THREADSGROUP = 32;


    void Start()
    {
        sourceTexture.wrapMode = TextureWrapMode.Clamp;

        textureSize = sourceTexture.width;

        kernel_tiles = compute.FindKernel("K_Tiles");

        resultTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat);
        resultTexture.enableRandomWrite = true;
        resultTexture.wrapMode = TextureWrapMode.Clamp;
        resultTexture.filterMode = FilterMode.Point;
        resultTexture.autoGenerateMips = false;
        resultTexture.Create();

        // Reminder - how many groups of threads are instantiated, not how many threads are instantiated.
        count = Mathf.Max(1, textureSize / THREADSGROUP);
    }


    void Update()
    {
        // Set textures to the kernel
        compute.SetTexture(kernel_tiles, propSourceTexture, sourceTexture);
        compute.SetTexture(kernel_tiles, propResultTexture, resultTexture);


        // Set variables
        compute.SetInt("_KernelSize", kernelSize);
        compute.SetBool("_VisualizeDispatchThreadID", VisualizeDispatchThreadID);
        compute.SetBool("_VisualizeGroupID", VisualizeGroupID);
        compute.SetBool("_VisualizeGroupThreadID", VisualizeGroupThreadID);
        compute.SetBool("_VisualizeGroupIndex", VisualizeGroupIndex);


        // Render
        compute.Dispatch(kernel_tiles, count, count, 1);
    }


    GUIStyle guiStyle = new GUIStyle();

    void OnGUI()
    {
        // Draw the result to screen
        int  w  =  Screen.width / 2;
        int  h  =  Screen.height / 2;
        int  s  =  1024;

        if (s > Screen.height) {
            s = Screen.height;
        }

        GUI.DrawTexture (new Rect (w - s/2 , h - s/2 , s, s), resultTexture, ScaleMode.ScaleToFit, false, w/h);
    }


    void OnDisable()
    {
        if (resultTexture != null) {
            resultTexture.Release();
            DestroyImmediate(resultTexture);
        }
    }
}