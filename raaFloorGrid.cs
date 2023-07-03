using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class raaFloorGrid : MonoBehaviour
{

    Mesh mesh;
    public int Divisons = 5;

    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();

        int limit = 5;
        if (Divisons > 1)
        {
            if (Divisons % 2 == 1) limit = (Divisons - 1) / 2;
            else limit = Divisons / 2;
        }

        List<Vector3> lverts = new List<Vector3>();
        List<Color> lcols = new List<Color>();
        List<int> lIndex = new List<int>();

        Debug.Log("Limit -> " + limit);

        int count = 0;
        for (int i = -limit; i <= limit; i++)
        {
            lcols.Add(new Color(0.0f, 0.4f, 0.4f, 0.4f));
            lverts.Add(new Vector3(limit, 0.0f, i));
            lIndex.Add(count);
            count++;

            lcols.Add(new Color(0.0f, 0.4f, 0.4f, 0.4f));
            lverts.Add(new Vector3(-limit, 0.0f, i));
            lIndex.Add(count);
            count++;

            lcols.Add(new Color(0.0f, 0.4f, 0.4f, 0.4f));
            lverts.Add(new Vector3(i, 0.0f, limit));
            lIndex.Add(count);
            count++;

            lcols.Add(new Color(0.0f, 0.4f, 0.4f, 0.4f));
            lverts.Add(new Vector3(i, 0.0f, -limit));
            lIndex.Add(count);
            count++;

            if (i != limit)
            {
                for (float f = 0.1f; f < 0.95f; f += 0.1f)
                {
                    lcols.Add(new Color(0.0f, 0.1f, 0.0f, 0.1f));
                    lverts.Add(new Vector3(limit, 0.0f, (float)i + f));
                    lIndex.Add(count);
                    count++;

                    lcols.Add(new Color(0.0f, 0.1f, 0.0f, 0.1f));
                    lverts.Add(new Vector3(-limit, 0.0f, (float)i + f));
                    lIndex.Add(count);
                    count++;

                    lcols.Add(new Color(0.0f, 0.1f, 0.0f, 0.1f));
                    lverts.Add(new Vector3((float)i + f, 0.0f, limit));
                    lIndex.Add(count);
                    count++;

                    lcols.Add(new Color(0.0f, 0.1f, 0.0f, 0.1f));
                    lverts.Add(new Vector3((float)i + f, 0.0f, -limit));
                    lIndex.Add(count);
                    count++;
                }
            }
        }


        //        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = lverts.ToArray();
        mesh.colors = lcols.ToArray();
        mesh.SetIndices(lIndex.ToArray(), MeshTopology.Lines, 0, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
