using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour {

    private MeshRenderer renderer;
    private MeshFilter filter;
    private Mesh mesh;
    private Vector3[] vertices, normals;
    private Vector2[] uv;
    private int[] triangles;

    [SerializeField]
    private float size;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        Reset();
	}
    
    /// <summary>
    /// Sets the mesh to a basic eight point cube with a center in the origin.
    /// </summary>
    private void Reset()
    {
        vertices = new Vector3[8];
        normals = new Vector3[8];
        uv = new Vector2[8];
        triangles = new int[36];

        //Set the vertices and normals
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                for(int k = 0; k < 2; k++)
                {
                    int index = (i * 4) + (j * 2) + k;
                    vertices[index] = new Vector3(i * size, j * size, k * size) - new Vector3(size / 2, size / 2, size / 2);
                    normals[index] = vertices[index].normalized;
                    uv[index] = Vector2.zero;
                }
            }
        }

        //Set the triangles
        for(int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = (int)Mathf.Sqrt(i);
        }

        //set the UVs

        //Finalize the mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDrawGizmos()
    {
        foreach(Vector3 vertex in vertices)
        {
            Gizmos.DrawSphere(vertex, 0.25f);
        }
    }
}
