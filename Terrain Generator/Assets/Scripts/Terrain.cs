using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class Terrain : MonoBehaviour {

    private Vector3[] vertices, normals;
    private Vector2[] uv;
    private int[] tris;
    private Mesh mesh;
    private Material material;

    public Vector3[] Vertices
    {
        get { return vertices; }
        set { vertices = value; }
    }

    public Vector3[] Normals
    {
        get { return normals; }
        set { normals = value; }
    }

    public Vector2[] UV
    {
        get { return uv; }
        set { uv = value; }
    }

    public int[] Triangles
    {
        get { return tris; }
        set { tris = value; }
    }

    public Transform TerrainTransform
    {
        get { return transform; }
    }
    
    private void Awake()
    {
        material = new Material(GetComponent<MeshRenderer>().material);
        material.name = name + "Material";
        GetComponent<MeshRenderer>().material = material;
    }

    public void CreateMesh(string name)
    {
        mesh = new Mesh();
        mesh.name = name;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(vertices[vertices.Length - 1].x / 2, 0, vertices[vertices.Length - 1].z / 2) * -1;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = tris;
        material.mainTexture = SetTexture();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    /// <summary>
    /// Takes in the vertices of the mesh and creates a texture
    /// </summary>
    /// <returns>The generated 2D texture</returns>
    public Texture2D SetTexture()
    {
        int size = (int)Mathf.Sqrt(vertices.Length);
        Color[] colors = new Color[vertices.Length];
        Texture2D texture = new Texture2D(size, size);

        float maxHeight = 0, minHeight = 0;

        foreach(Vector3 vertex in vertices)
        {
            if(vertex.y > maxHeight)
            {
                maxHeight = vertex.y;
            }

            if(vertex.y < minHeight)
            {
                minHeight = vertex.y;
            }
        }

        for(int i = 0; i < vertices.Length; i++)
        {
            //int count = 0;
            //float avgHeight = 0;

            //if(i % size != 0)
            //{
            //    avgHeight += vertices[i - 1].y;
            //    count++;
            //}

            //if(i+1 % size != 0 && i + 1 < size * size)
            //{
            //    avgHeight == vertices[i + 1].y;
            //}

            float toScale = (vertices[i].y - minHeight) / (maxHeight - minHeight);
            
            colors[i] = new Color((int)toScale, (int)toScale, (int)toScale);
            //texture.SetPixel((int)(i / size), i % size, Color.black);
        }

        texture.SetPixels(colors);

        Debug.Log("Texture size: " + texture.width + ", " + texture.height + ") Color[0]: " + texture.GetPixel(texture.width / 2, texture.height / 2));
        Debug.Log("Vertex size: " + vertices.Length);

        return texture;
    }
}
