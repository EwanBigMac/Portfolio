using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class raaProcMeshBillboard : MonoBehaviour
{
    Mesh mesh;

    //    private Texture3D tex;

    public Slider transSlider;
    public Slider transPowerSlider;
    public RangeSlider thresholdSlider;
    public Slider contrastSlider;
    public Slider contrastPowSlider;
    public Slider scaleSlider;
    public Slider shiftSlider;
    public Slider tileSlider;
    public Slider relativeSlider;
    public Slider separationSlider;

    private Renderer renderer;
    private Texture2D[] slices;
    private Color[] voxels;

    //static private uint g_shareID = 1;
    //private vrClusterManager m_ClusterMgr = null;
    //private vrCommand m_Command = null;
    //private raaAsyncServer server = null;
    //raaInterfaceManager im;

    public int clientID = 0;
    public int width = 256;
    public int height = 256;
    public int depth = 99;

    Vector3[] vertices;
    Vector3[] uvs;
    int[] triangles;

    int[] startIndex = { 3, 3, 3, 0, 2, 0, 1, 4, 2, 6, 7, 7 };
    int[] endIndex = { 2, 7, 0, 4, 1, 1, 5, 5, 6, 5, 6, 4 };
    int[] edgeIndex = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    int[,] faceOrder = { { 8, 0, 2, 1, 10, 11, 3, 7, 9, 6, 5, 4 }, { 8, 4, 0, 1, 2, 5, 3, 11, 7, 6, 9, 10 }, { 1, 0, 2, 3, 5, 4, 6, 7, 9, 8, 10, 11 }, { 10, 1, 11, 7, 3, 2, 5, 6, 4, 0, 8, 9 }, { 5, 3, 2, 0, 1, 11, 10, 8, 9, 7, 6, 4 }, { 4, 6, 5, 2, 3, 7, 11, 1, 10, 9, 8, 0 }, { 0, 8, 4, 5, 6, 9, 7, 3, 11, 10, 1, 2 }, { 2, 1, 0, 4, 8, 10, 9, 6, 7, 11, 3, 5 } };
    Vector4 planeNorm = new Vector4(0, 0, -1, 0);
/*
    public void onDisconnect(NetworkMessage msg)
    {
        if (clientID == msg.conn.connectionId) clientID = 0;
    }

    public void distributeMessage(raaMessage m)
    {
        if (m_ClusterMgr != null && m_ClusterMgr.IsServer())
        {
            vrValue val = vrValue.CreateList();
            val.AddListItem(new vrValue(m.data()));
            m_Command.Do(val);
        }
    }

    void OnApplicationQuit()
    {
        if (m_Command != null)
        {
            m_Command.Dispose();
            m_Command = null;
        }
    }
*/
    void buildMesh()
    {
        Vector3 vPos = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 vStep = new Vector3(1.0f / width, 1.0f / height, 1.0f / depth);
        List<Vector3> lverts = new List<Vector3>();
        List<Color> lcols = new List<Color>();
        List<int> lindicies = new List<int>();
        int iInd = 0;
        int iIndex = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, iInd++)
                {
                    Color c = voxels[iInd];
                    Vector3 pos = new Vector3();
                    pos[0] = vPos[0] + vStep[0] * x;
                    pos[1] = vPos[1] + vStep[1] * y;
                    pos[2] = vPos[2] + vStep[2] * z;
                    lverts.Add(pos);
                    lcols.Add(c);
                    lindicies.Add(iIndex++);
                }
            }
        }

        if (mesh == null) mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = lverts.ToArray();
        mesh.colors = lcols.ToArray();
        mesh.SetIndices(lindicies.ToArray(), MeshTopology.Points, 0, true);
    }

    void updateMesh(float fLower, float fUpper)
    {
        List<int> lindicies = new List<int>();
        int iCount = width * height * depth;

        for (int iInd = 0; iInd < iCount; iInd++)
        {
            Color c = voxels[iInd];
            if (c.a > fLower && c.a < fUpper) lindicies.Add(iInd);
        }

        mesh.SetIndices(lindicies.ToArray(), MeshTopology.Points, 0, true);
    }

    public void thresValue(uint id, float fLower, float fUpper)
    {
        updateMesh(fLower, fUpper);
    }

    public void thresState(uint id, bool bState)
    {
        updateMesh(0.0f, 1.0f);
    }


    // Use this for initialization
    void Start()
    {
        //Application.runInBackground = true;
        //uint shareID = g_shareID++;
        //string shareName = "VRShareTransform_" + shareID.ToString();
//        m_Command = new vrCommand(shareName, _CommandHandler); // include for octave
//        m_ClusterMgr = MiddleVR.VRClusterMgr; // include for octave

        float[] fRed = { 1.0f, 0.0f, 0.0f, 1.0f };
        float[] fGreen = { 0.0f, 1.0f, 0.0f, 1.0f };
        float[] fBlue = { 0.0f, 0.0f, 1.0f, 1.0f };

        /*
        im = new raaInterfaceManager();
        im.addSlider("Transparency", "_Trans", 1.0f, 0.0f, 1.0f);
        im.addSlider("Trans Power", "_TransP", 1.0f, -2.0f, 2.0f);
        //        im.addRangeSlider("Window", "_Thres", 0.0f, 1.0f, 0.0f, 1.0f, false);
//        im.addRangeSlider("Window", thresValue, thresState, 0.0f, 1.0f, 0.0f, 1.0f, false);
        im.addRangeSlider("Window", thresValue, thresState, 0.0f, 1.0f, 0.0f, 1.0f, false);
        im.addSlider("Contrast", "_Contrast", 1.0f, 0.0f, 5.0f);
        im.addSlider("Contrast Power", "_ContrastP", 1.0f, -2.0f, 2.0f);
        im.addSlider("Shift", "_Shift", 0.0f, -1.0f, 1.0f);
        im.addSlider("Scale", "_Scale", 1.0f, 0.0f, 3.0f);
        im.addSlider("Tile Slider", "_TileSize", 1.0f, 0.001f, 5.0f);
        im.addSlider("Relative Slider", "_RelSize", 1.0f, 0.001f, 3.0f);
        im.addSlider("Seperation", "_Seperation", 1.0f, -1.0f, 3.0f);
        string[] modes = { "Off", "Colour", "Subtract" };
        im.addRangeSliderCombo("Red Window", "_RedWindow", 0.2f, 0.4f, 0.0f, 1.0f, fRed, true, 0, modes);
        im.addRangeSliderCombo("Green Window", "_GreenWindow", 0.4f, 0.6f, 0.0f, 1.0f, fGreen, true, 0, modes);
        im.addRangeSliderCombo("Blue Window", "_BlueWindow", 0.6f, 0.8f, 0.0f, 1.0f, fBlue, true, 0, modes);
        */
/*
        if (m_ClusterMgr != null)
        {
            if (m_ClusterMgr.IsServer())
            {
                Debug.Log("IsServer");
                server = new raaAsyncServer(19723, processEvent);
            }
            else
            {
                Debug.Log("Not Server");
            }
        }
        else
        {
*/           // server = new raaAsyncServer(39723, processEvent);
//        }


        //        int width = 256;
        //        int height = 256;
        //        int depth = 99;

        slices = new Texture2D[depth];

        for (int i = 1; i <= depth; i++)
        {
            string number;

            if (i < 10) number = "mrbrain-8bit00" + i;
            else if (i < 100) number = "mrbrain-8bit0" + i;
            else number = "mrbrain-8bit" + i;

            slices[i - 1] = Resources.Load<Texture2D>("vol8/" + number); //stanford head
        }

        //        tex = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);

        voxels = new Color[width * height * depth];
        int idx = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, idx++)
                {
                    voxels[idx] = slices[z].GetPixel(x, y);
                }
            }
        }

        buildMesh();

        //im.initalise(GetComponent<Renderer>());

    }



    private void OnWillRenderObject()
    {
        float r = 1.0f / 512.0f;
        float u = 1.0f / 512.0f;
        float d = 1.0f / 200.0f;
        Vector4[] verts = new Vector4[8];
        Vector4[] v = new Vector4[6];
        Vector4[] _v = new Vector4[6];
        int iCount = 0;

        Vector3 cpos = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(new Vector3(0, 0, 0));
        
        verts[0] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(-r, -u, -d));
        verts[1] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(r, -u, -d));
        verts[2] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(r, u, -d));
        verts[3] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(-r, u, -d));
        verts[4] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(-r, -u, d));
        verts[5] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(r, -u, d));
        verts[6] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(r, u, d));
        verts[7] = Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.TransformPoint(-r, u, d));

        int nearest = 0;
        float t, l;
        Vector4 sideDir;

        for (int i = 1; i < 8; i++) if (verts[i].z < verts[nearest].z) nearest = i;

        for (int i = 0; i < 12; i++)
        {
            sideDir = (verts[endIndex[faceOrder[nearest, i]]] - verts[startIndex[faceOrder[nearest, i]]]);
            l = sideDir.magnitude;
            sideDir.Normalize();

            t = -(Vector3.Dot(verts[startIndex[faceOrder[nearest, i]]], planeNorm) + cpos.z) / Vector3.Dot(sideDir, planeNorm);

            if (t < l && t > 0.0f) v[iCount++] = transform.InverseTransformPoint(Camera.current.cameraToWorldMatrix.MultiplyPoint3x4(verts[startIndex[faceOrder[nearest, i]]] + (sideDir * t)));
        }

        if (iCount > 2)
        {
            int n = iCount - 1;
            int nn = 0;
            int i = 0;
            for (bool b = true; nn <= n; b = !b, i++) _v[i] = b ? v[n--] : v[nn++];

            GetComponent<Renderer>().material.SetVectorArray("_Verts", _v);
            GetComponent<Renderer>().material.SetInt("_Count", iCount);
        }
    }

   /*public void processEvent(raaEvent e)
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
//                distributeMessage(e.msg()); // include for octave
                im.process(GetComponent<Renderer>(), e.msg());
                break;
        }
    }
   */

    public void OnTransPowSlider()
    {
        renderer.material.SetFloat("_TransP", transPowerSlider.value);
    }
    public void OnTransparencySlider()
    {
        renderer.material.SetFloat("_Trans", transSlider.value);
    }
    public void OnScaleSlider()
    {
        renderer.material.SetFloat("_Scale", scaleSlider.value);
    }
    public void OnShiftSlider()
    {
        renderer.material.SetFloat("_Shift", shiftSlider.value);
    }

    public void OnContrastSlider()
    {
        renderer.material.SetFloat("_Contrast", contrastSlider.value);
    }
    public void OnContrastPowSlider()
    {
        renderer.material.SetFloat("_ContrastP", contrastPowSlider.value);
    }

    public void OnThresSlider()
    {
        float[] v = { thresholdSlider.LowValue, thresholdSlider.HighValue };
        renderer.material.SetFloatArray("_Thres", v);
    }

    public void OnTileSlider()
    {
        renderer.material.SetFloat("_TileSize", tileSlider.value);
    }

    public void OnRelSlider()
    {
        renderer.material.SetFloat("_RelSize", relativeSlider.value);
    }

    public void OnSepSlider()
    {
        Debug.Log($"Sepp {separationSlider.value}");
        renderer.material.SetFloat("_Sepparation", separationSlider.value);
    }

    void Update()
    {
        saveValues();
    }

    public void saveValues()
    {
        MainManager.Instance.lThresh = thresholdSlider.LowValue;
        MainManager.Instance.hThresh = thresholdSlider.HighValue;
        MainManager.Instance.trans = transSlider.value;
        MainManager.Instance.scale = scaleSlider.value;
        MainManager.Instance.shift = shiftSlider.value;
        //MainManager.Instance.transP = transPowerSlider.value;
        //MainManager.Instance.contrast = contrastSlider.value;
       // MainManager.Instance.contrastP = contrastPowSlider.value;
        MainManager.Instance.relSize = relativeSlider.value;
        MainManager.Instance.tileSize = tileSlider.value;
        MainManager.Instance.separation = separationSlider.value;
    }

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        renderer = GetComponent<Renderer>();



        if (transPowerSlider != null)
        {
            transPowerSlider.onValueChanged.AddListener(delegate { OnTransPowSlider(); });
            transPowerSlider.minValue = -2.0f;
            transPowerSlider.maxValue = 2.0f;
            transPowerSlider.value = MainManager.Instance.transP;
            OnTransPowSlider();
        }



        if (thresholdSlider != null)
        {
            thresholdSlider.OnValueChanged.AddListener(delegate { OnThresSlider(); });
            thresholdSlider.MinValue = 0.0f;
            thresholdSlider.MaxValue = 1.0f;
            thresholdSlider.LowValue = MainManager.Instance.lThresh;
            thresholdSlider.HighValue = MainManager.Instance.hThresh;
            OnThresSlider();
        }



        if (transSlider != null)
        {
            transSlider.onValueChanged.AddListener(delegate { OnTransparencySlider(); });
            transSlider.minValue = 0.0f;
            transSlider.maxValue = 1.0f;
            transSlider.value = MainManager.Instance.trans;
            OnTransparencySlider();
        }



        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(delegate { OnScaleSlider(); });
            scaleSlider.minValue = 0.0f;
            scaleSlider.maxValue = 3.0f;
            scaleSlider.value = MainManager.Instance.scale;
            OnScaleSlider();
        }



        if (shiftSlider != null)
        {
            shiftSlider.onValueChanged.AddListener(delegate { OnShiftSlider(); });
            shiftSlider.minValue = -1.0f;
            shiftSlider.maxValue = 1.0f;
            shiftSlider.value = MainManager.Instance.shift;
            OnShiftSlider();
        }

        if (contrastSlider != null)
        {
            contrastSlider.onValueChanged.AddListener(delegate { OnContrastSlider(); });
            contrastSlider.minValue = 0.0f;
            contrastSlider.maxValue = 5.0f;
            contrastSlider.value = MainManager.Instance.contrast;
        }

        if (contrastPowSlider != null)
        {
            contrastPowSlider.onValueChanged.AddListener(delegate { OnContrastPowSlider(); });
            contrastPowSlider.minValue = -2.0f;
            contrastPowSlider.maxValue = 2.0f;
            contrastPowSlider.value = MainManager.Instance.contrastP;
        }

        if(relativeSlider != null)
        {
            relativeSlider.onValueChanged.AddListener(delegate { OnRelSlider(); });
            relativeSlider.minValue = 0.001f;
            relativeSlider.maxValue = 3.0f;
            relativeSlider.value = MainManager.Instance.relSize;
        }


        if (tileSlider != null)
        {
            tileSlider.onValueChanged.AddListener(delegate { OnTileSlider(); });
            tileSlider.minValue = 0.001f;
            tileSlider.maxValue = 3.0f;
            tileSlider.value = MainManager.Instance.tileSize;
        }

        if (separationSlider != null)
        {
            separationSlider.onValueChanged.AddListener(delegate { OnSepSlider(); });
            separationSlider.minValue = -1.0f;
            separationSlider.maxValue = 3.0f;
            separationSlider.value = MainManager.Instance.separation;
        }
    }
}


/*
    private vrValue _CommandHandler(vrValue iValue)
    {
        if (m_ClusterMgr.IsClient())
        {
            raaMessage m = new raaMessage(iValue[0].GetBuffer());
            im.process(GetComponent<Renderer>(), m);
        }
        return null;
    }
*/

