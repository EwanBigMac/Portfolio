using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raaFlipNormals : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < normals.Length; i++) normals[i] = -1.0f * normals[i];
        mesh.normals = normals;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j += 3)
            {
                int t = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = t;
            }
            mesh.SetTriangles(tris, i);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
