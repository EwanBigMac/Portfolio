using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]


public class RTVolvis : MonoBehaviour
{
    Mesh mesh;
    private Renderer renderer;

    private float rotation = 0.0f;

    private Texture3D tex;
    private Texture2D[] slices;

    private static float dim = 0.5f;

    private static Vector3 min = new Vector3(-dim, -dim, -dim);
    private static Vector3 max = new Vector3(dim, dim, dim);

    private static Vector3[] verts =
    {
        new Vector3(-dim, -dim, -dim),
        new Vector3(dim, -dim, -dim),
        new Vector3(dim, dim, -dim),
        new Vector3(-dim, dim, -dim),
        new Vector3(-dim, -dim, dim),
        new Vector3(dim, -dim, dim),
        new Vector3(dim, dim, dim),
        new Vector3(-dim, dim, dim)
    };

    private static Vector3[] texcoords =
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),
    };

    private static int[] tris =
    {
        0, 1, 2,
        2, 3, 0,
        1, 5, 6,
        6, 2, 1,
        5, 4, 7,
        7, 6, 5,
        4, 0, 3,
        3, 7, 4,
        4, 5, 1,
        1, 0, 4,
        3, 2, 6,
        6, 7, 3,
    };


    Vector3[] vertices;
    Vector3[] uvs;
    int[] triangles;

    public Slider TransparencySlider;
    public Slider ShiftSlider;
    public Slider ScaleSlider;
    public RangeSlider ThresholdSlider;   


    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        renderer = GetComponent<Renderer>();

        if (ThresholdSlider != null)
        {
            ThresholdSlider.OnValueChanged.AddListener(delegate { onThresholdSlider(); });
            ThresholdSlider.MinValue = 0.0f;
            ThresholdSlider.MaxValue = 1.0f;
            ThresholdSlider.LowValue = MainManager.Instance.lThresh;
            ThresholdSlider.HighValue = MainManager.Instance.hThresh;
            onThresholdSlider();
        }

        if (TransparencySlider != null)
        {
            TransparencySlider.onValueChanged.AddListener(delegate { onTransparencySlider(); });
            TransparencySlider.minValue = 0.0f;
            TransparencySlider.maxValue = 1.0f;
            TransparencySlider.value = MainManager.Instance.trans;
            onTransparencySlider();
        }

        if (ScaleSlider != null)
        {
            ScaleSlider.onValueChanged.AddListener(delegate { onScaleSlider(); });
            ScaleSlider.minValue = 0.0f;
            ScaleSlider.maxValue = 3.0f;
            ScaleSlider.value = MainManager.Instance.scale;
            onScaleSlider();
        }

        if (ShiftSlider != null)
        {
            ShiftSlider.onValueChanged.AddListener(delegate { onShiftSlider(); });
            ShiftSlider.minValue = 0.0f;
            ShiftSlider.maxValue = 1.0f;
            ShiftSlider.value = MainManager.Instance.shift;
            onShiftSlider();
        }

        

    }

    public void saveValues()
    {
        
        MainManager.Instance.lThresh = ThresholdSlider.LowValue;
        MainManager.Instance.hThresh = ThresholdSlider.HighValue;
        MainManager.Instance.trans = TransparencySlider.value;
        MainManager.Instance.scale = ScaleSlider.value;
        MainManager.Instance.shift = ShiftSlider.value;
        
    }

    public void onTransparencySlider()
    {
        renderer.material.SetFloat("_Transparency", (float)TransparencySlider.value);
    }

    public void onThresholdSlider()
    {
        float[] v = { ThresholdSlider.LowValue, ThresholdSlider.HighValue };

//        Debug.Log($"Threshold: {v[0]}, {v[1]}");

        renderer.material.SetFloatArray("_Threshold", v);
    }

    public void onScaleSlider()
    {
        renderer.material.SetFloat("_Scale", ScaleSlider.value);
    }
    public void onShiftSlider()
    {
        renderer.material.SetFloat("_Shift", ShiftSlider.value);
    }


   
    void Start()
    {
        float[] fRed = { 1.0f, 0.0f, 0.0f, 1.0f };
        float[] fGreen = { 0.0f, 1.0f, 0.0f, 1.0f };
        float[] fBlue = { 0.0f, 0.0f, 1.0f, 1.0f };

        int width = 256;
        int height = 256;
        int depth = 99;

        slices = new Texture2D[depth];

        for (int i = 1; i <= depth; i++)
        {
            string number;

            if (i < 10) number = "mrbrain-8bit00" + i;
            else if (i < 100) number = "mrbrain-8bit0" + i;
            else number = "mrbrain-8bit" + i;

            slices[i - 1] = Resources.Load<Texture2D>("vol8/" + number); //stanford head
        }


        tex = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);

        Color[] cols = new Color[width * height * depth];
        int idx = 0;

        float[] max = { 0, 0, 0, 0 };
        float[] min = { 255, 255, 255, 255 };

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, idx++)
                {
                    cols[idx] = slices[z].GetPixel(x, y);
                }
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        GetComponent<Renderer>().material.SetTexture("_Volume", tex);

        makeMeshData();
        createMesh();
    }

    void Update()
    {
        saveValues();
    }

    void makeMeshData()
    {
        vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0) };
        triangles = new int[] { 0, 1, 2 };
    }

    void createMesh()
    {
        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, texcoords);
        mesh.SetTriangles(tris, 0);
        mesh.bounds = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f));
    }
}
