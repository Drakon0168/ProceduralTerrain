using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

    private Mesh terrainMesh;
    private Vector3[] vertices;
    private Vector2[] UV;
    private Vector3[] normals;
    private List<List<Vector3>> verts;
    private int[] triangles;
    private ArrayList tris;
    public float startingShift, shiftChange;
    private float shift;
    private int generationNumber;

    void Start() {

        terrainMesh = new Mesh();
        shift = startingShift;
        GetComponent<MeshFilter>().mesh = terrainMesh;

        verts = new List<List<Vector3>>();
        verts.Add(new List<Vector3>());
        verts.Add(new List<Vector3>());
        verts[0].Add(new Vector3(-1, 0, 1));
        verts[0].Add(new Vector3(1, 0, 1));
        verts[1].Add(new Vector3(-1, 0, -1));
        verts[1].Add(new Vector3(1, 0, -1));

        vertices = convertVertices(verts);

        setTriangles();

        setNormals(Vector3.up);

        setUV();

        terrainMesh.vertices = vertices;
        terrainMesh.uv = UV;
        terrainMesh.normals = normals;
        terrainMesh.triangles = triangles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            generationNumber++;
            generateTerrain(shift);
            shift *= shiftChange;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Reset();
        }
    }

    public void Reset()
    {
        shift = startingShift;
        generationNumber = 0;
        verts = new List<List<Vector3>>();
        verts.Add(new List<Vector3>());
        verts.Add(new List<Vector3>());
        verts[0].Add(new Vector3(-1, 0, 1));
        verts[0].Add(new Vector3(1, 0, 1));
        verts[1].Add(new Vector3(-1, 0, -1));
        verts[1].Add(new Vector3(1, 0, -1));

        vertices = convertVertices(verts);

        setTriangles();

        setNormals(Vector3.up);

        setUV();

        terrainMesh.vertices = vertices;
        terrainMesh.uv = UV;
        terrainMesh.normals = normals;
        terrainMesh.triangles = triangles;
    }

    public void printArray(List<List<Vector3>> list)
    {
        string values = "";
        for(int i = 0; i < list.Count; i++)
        {
            for(int j = 0; j < list[0].Count; j++)
            {
                values += list[i][j].y + ", ";
            }
            values += "\n";
        }
        Debug.Log(values);
    }

    //Square step find four corners create value in the center based on corner values
    //Diamond step create values each value sets its own value based on those around it
    //Reset Triangles
    //Change all values in mesh as well as in variables
    //Repeat
    public void generateTerrain(float maxShift)
    {
        //Set new Array
        List<List<Vector3>> newVerts = new List<List<Vector3>>();
        for(int i = 0; i < (verts.Count * 2) - 1; i++)
        {
            newVerts.Add(new List<Vector3>());
            for (int j = 0; j < (verts[0].Count * 2) - 1; j++)
            {
                newVerts[i].Add(new Vector3());
                if (i % 2 == 0 && j % 2 == 0)
                {
                    newVerts[i][j] = new Vector3(i, verts[i / 2][j / 2].y, j);
                }
                else
                {
                    newVerts[i][j] = new Vector3(i, 0, j);
                }
            }
        }
        // Square Step
        for(int i = 0; i < newVerts.Count; i++)
        {
            for(int j = 0; j < newVerts[0].Count; j++)
            {
                if(i % 2 != 0 && j % 2 != 0)
                {
                    float average = (newVerts[i - 1][j - 1].y + newVerts[i - 1][j + 1].y + newVerts[i + 1][j + 1].y + newVerts[i + 1][j - 1].y) / 4;
                    float offset = (Random.value * maxShift * 2) - maxShift;
                    newVerts[i][j] = new Vector3(i, average + offset, j);
                }
            }
        }
        // Diamond Step
        for (int i = 0; i < newVerts.Count; i++)
        {
            for (int j = 0; j < newVerts[0].Count; j++)
            {
                if (i % 2 != j % 2)
                {
                    int validPoints = 0;
                    float average = 0;
                    float offset = (Random.value * maxShift * 2) - maxShift;
                    if (i > 0)
                    {
                        validPoints++;
                        average += newVerts[i-1][j].y;
                    }
                    if(i < newVerts.Count - 1)
                    {
                        validPoints++;
                        average += newVerts[i + 1][j].y;
                    }
                    if (j > 0)
                    {
                        validPoints++;
                        average += newVerts[i][j - 1].y;
                    }
                    if (j < newVerts[0].Count - 1)
                    {
                        validPoints++;
                        average += newVerts[i][j + 1].y;
                    }
                    average /= validPoints;
                    newVerts[i][j] = new Vector3(i, average + offset, j);
                }
            }
        }
        //Transfer new vertices
        verts = newVerts;
        //set triangles set normals set UV
        vertices = convertVertices(verts);
        setTriangles();
        setNormals(Vector3.up);
        setUV();
        //Store new values in mesh
        terrainMesh.vertices = vertices;
        terrainMesh.uv = UV;
        terrainMesh.normals = normals;
        terrainMesh.triangles = triangles;
    }

    public Vector3[] convertVertices(List<List<Vector3>> vert2DArray)
    {
        Vector3[] vertArray = new Vector3[vert2DArray.Count * vert2DArray[0].Count];

        int index = 0;
        foreach (List<Vector3> points in verts)
        {
            foreach(Vector3 point in points){
                vertArray[index] = point;
                index++;
            }
        }
        return vertArray;
    }

    public void setNormals(Vector3 normal)
    {
        normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = normal;
        }
    }

    public void setUV()
    {
        UV = new Vector2[verts.Count * verts[0].Count];
        for (int i = 0; i < verts.Count; i++)
        {
            for (int j = 0; j < verts[0].Count; j++)
            {
                UV[(i * verts[0].Count) + j] = new Vector2((float)j / (verts[0].Count - 1), (float)i / (verts.Count - 1));
            }
        }
    }

    public void setTriangles()
    {
        tris = new ArrayList();
        int index = 0;
        for (int i = 0; i < verts.Count - 1; i++)
        {
            for (int j = 0; j < verts[0].Count - 1; j++)
            {
                tris.Add(index);
                tris.Add(index + 1);
                tris.Add(index + verts.Count + 1);
                tris.Add(index);
                tris.Add(index + verts.Count + 1);
                tris.Add(index + verts.Count);
                index++;
            }
            index++;
        }

        triangles = new int[tris.Count];

        for (int i = 0; i < tris.Count; i++)
        {
            triangles[i] = (int)tris[i];
        }
    }
}
