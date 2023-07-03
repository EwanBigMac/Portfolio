using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UI.Extensions;



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]




public class raaProcMeshSliceStandalone : MonoBehaviour
{
    Mesh mesh;
    private Renderer renderer;



    public Slider sliceSlider;
    public RangeSlider thresholdSlider;
    public Slider transparencySlider;
    public Slider scaleSlider;
    public Slider shiftSlider;
    public Slider rotateSlider;
    public Toggle autoSSTToggle;

    

    private float rotation = 0.0f;



  
    private Texture3D tex;
    private Texture2D[] slices;



    Vector3[] vertices;
    Vector3[] uvs;
    int[] triangles;



    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        renderer = GetComponent<Renderer>();



        if (sliceSlider != null)
        {
            sliceSlider.onValueChanged.AddListener(delegate { OnSliceSlider(); });
            sliceSlider.minValue = 10.0f;
            sliceSlider.maxValue = 1100.0f;
            sliceSlider.wholeNumbers = true;
            sliceSlider.value = MainManager.Instance.slices;
            OnSliceSlider();
        }



        if (thresholdSlider != null)
        {
            thresholdSlider.OnValueChanged.AddListener(delegate { OnThresholdSlider(); });
            thresholdSlider.MinValue = 0.0f;
            thresholdSlider.MaxValue = 1.0f;
            thresholdSlider.LowValue = MainManager.Instance.lThresh;
            thresholdSlider.HighValue = MainManager.Instance.hThresh;
            OnThresholdSlider();
        }



        if (transparencySlider != null)
        {
            transparencySlider.onValueChanged.AddListener(delegate { OnTransparencySlider(); });
            transparencySlider.minValue = 0.0f;
            transparencySlider.maxValue = 1.0f;
            transparencySlider.value = MainManager.Instance.trans;
            OnTransparencySlider();
        }



        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(delegate { OnScaleSlider(); });
            scaleSlider.minValue = 0.0f;
            scaleSlider.maxValue = 5.0f;
            scaleSlider.value = MainManager.Instance.trans;
            OnScaleSlider();
        }



        if (shiftSlider != null)
        {
            shiftSlider.onValueChanged.AddListener(delegate { OnShiftSlider(); });
            shiftSlider.minValue = 0.0f;
            shiftSlider.maxValue = 1.0f;
            shiftSlider.value = MainManager.Instance.shift;
            OnShiftSlider();
        }



        if (rotateSlider != null)
        {
            rotateSlider.onValueChanged.AddListener(delegate { OnRotateSlider(); });
            rotateSlider.minValue = 0.0f;
            rotateSlider.maxValue = 360.0f;
            rotateSlider.value = MainManager.Instance.rotate;
            OnRotateSlider();
        }



        if (autoSSTToggle != null)
        {
            autoSSTToggle.onValueChanged.AddListener(delegate { OnAutoSST(); });
        }




    }

    public void saveValues()
    {
        MainManager.Instance.slices = sliceSlider.value;
        MainManager.Instance.lThresh = thresholdSlider.LowValue;
        MainManager.Instance.hThresh = thresholdSlider.HighValue;
        MainManager.Instance.trans = transparencySlider.value;
        MainManager.Instance.scale = scaleSlider.value;
        MainManager.Instance.shift = shiftSlider.value;
        MainManager.Instance.rotate = rotateSlider.value;
    }
        

    void OnApplicationQuit()
    {
    }




    public void OnSliceSlider()
    {
        renderer.material.SetInt("_Slice", (int)sliceSlider.value);
    }
    public void OnTransparencySlider()
    {
        renderer.material.SetFloat("_Trans", transparencySlider.value);
    }
    public void OnScaleSlider()
    {
        renderer.material.SetFloat("_Scale", scaleSlider.value);
    }
    public void OnShiftSlider()
    {
        renderer.material.SetFloat("_Shift", shiftSlider.value);
    }
    public void OnRotateSlider()
    {
        float v = rotateSlider.value - rotation;
        rotation = rotateSlider.value*5;



        transform.Rotate(Vector3.up, Mathf.Deg2Rad * v);



    }



    public void OnThresholdSlider()
    {
        float[] v = { thresholdSlider.LowValue, thresholdSlider.HighValue };
        renderer.material.SetFloatArray("_Thres", v);
    }



    public void OnAutoSST()
    {
        if (autoSSTToggle.isOn) renderer.material.SetInt("_AutoSST", 1);
        else renderer.material.SetInt("_AutoSST", 0);



    }



    // Use this for initialization
    void Start()
    {
        float[] fRed = { 1.0f, 0.0f, 0.0f, 1.0f };
        float[] fGreen = { 0.0f, 1.0f, 0.0f, 1.0f };
        float[] fBlue = { 0.0f, 0.0f, 1.0f, 1.0f };
        


        /*
                im = new raaInterfaceManager();
                im.addIntSlider("Slices", "_Slice", 200, 50, 1100);
                im.addSlider("Transparency", "_Trans", 1.0f, 0.0f, 1.0f);
                im.addSlider("Trans Power", "_TransP", 1.0f, -2.0f, 2.0f);
                im.addRangeSlider("Window", "_Thres", 0.0f, 1.0f, 0.0f, 1.0f, false);
                im.addSlider("Contrast", "_Contrast", 1.0f, 0.0f, 5.0f);
                im.addSlider("Contrast Power", "_ContrastP", 1.0f, -2.0f, 2.0f);
                im.addSlider("Shift", "_Shift", 0.0f, -1.0f, 1.0f);
                im.addSlider("Scale", "_Scale", 1.0f, 0.0f, 3.0f);
                string[] modes = { "Off", "Colour", "Subtract" };
                im.addRangeSliderCombo("Red Window", "_RedWindow", 0.2f, 0.4f, 0.0f, 1.0f, fRed, true, 0, modes);
                im.addRangeSliderCombo("Green Window", "_GreenWindow", 0.4f, 0.6f, 0.0f, 1.0f, fGreen, true, 0, modes);
                im.addRangeSliderCombo("Blue Window", "_BlueWindow", 0.6f, 0.8f, 0.0f, 1.0f, fBlue, true, 0, modes);
        */



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
        //        im.initalise(GetComponent<Renderer>());



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
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f));
    }
}