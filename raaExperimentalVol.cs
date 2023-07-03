using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]


public class raaExperimentalVol : MonoBehaviour
{

    static private uint g_shareID = 1;
    private raaAsyncServer server = null;

    raaInterfaceManager im;

    Mesh mesh;

    private Texture3D tex;
    private Texture2D[] slices;
    public int clientID = 0;

    Vector3[] vertices;
    Vector3[] uvs;
    int[] triangles;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void OnApplicationQuit()
    {
        server = null;
    }
    void Start()
    {
        uint shareID = g_shareID++;
        string shareName = "VRShareTransform_" + shareID.ToString();

        float[] fRed = { 1.0f, 0.0f, 0.0f, 1.0f };
        float[] fGreen = { 0.0f, 1.0f, 0.0f, 1.0f };
        float[] fBlue = { 0.0f, 0.0f, 1.0f, 1.0f };

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

        server = new raaAsyncServer(39723, processEvent);

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

            slices[i - 1] = Resources.Load<Texture2D>("vol8/" + number);
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
        im.initalise(GetComponent<Renderer>());

        makeMeshData();
        createMesh();
    }

    void processEvent(raaEvent e)
    {
        switch (e.type())
        {
            case raaEvent.cm_usConnect:
                Debug.Log("Connect");
                im.sendInitMsg(server);
                break;
            case raaEvent.cm_usDisconnect:
                Debug.Log("Disconnect");
                break;
            case raaEvent.cm_usMsg:
                im.process(GetComponent<Renderer>(), e.msg());
                break;
        }
    }

    void Update()
    {
        if (server != null) server.processEvents();
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
