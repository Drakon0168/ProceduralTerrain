using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

    private Terrain[] terrains;
    [SerializeField]
    private GameObject terrainPrefab;
    private ArrayList tris;
    [SerializeField]
    private float startingShift;
    [SerializeField][Range(0,1)]
    private float shiftChange;
    private float shift;
    private int generationNumber;

    void Start() {
        terrains = new Terrain[6];

        for (int i = 0; i < terrains.Length; i++)
        {
            terrains[i] = Instantiate(terrainPrefab, transform).GetComponent<Terrain>();
            terrains[i].name = "Terrain " + i;
        }

        Reset();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            generationNumber++;
            for(int i = 0; i < terrains.Length; i++)
            {
                generateTerrain(shift, terrains[i]);
            }
            shift *= shiftChange;

            createCube();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
            
            for (int i = 0; i < 7; i++)
            {
                generationNumber++;
                for (int j = 0; j < terrains.Length; j++)
                {
                    generateTerrain(shift, terrains[j]);
                }
                shift *= shiftChange;
            }

            createCube();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Reset();
        }
    }

    public void Reset()
    {
        List<List<Vector3>> verts = new List<List<Vector3>>();

        shift = startingShift;
        generationNumber = 0;
        verts = new List<List<Vector3>>();
        verts.Add(new List<Vector3>());
        verts.Add(new List<Vector3>());
        verts[0].Add(new Vector3(-1, 0, 1));
        verts[0].Add(new Vector3(1, 0, 1));
        verts[1].Add(new Vector3(-1, 0, -1));
        verts[1].Add(new Vector3(1, 0, -1));

        for(int i = 0; i < terrains.Length; i++)
        {
            terrains[i].Vertices = convertDown(verts);

            terrains[i].Triangles = setTriangles(terrains[i]);

            terrains[i].Normals = setNormals(Vector3.up, terrains[i].Vertices);

            terrains[i].UV = setUV(terrains[i]);

            terrains[i].CreateMesh("TerrainMesh");
        }

        createCube();
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
    public void generateTerrain(float maxShift, Terrain terrain)
    {
        List<List<Vector3>> verts = convertUp(terrain.Vertices);

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

                    //prevent the outer vertices from offsetting
                    if (i == 0 || j == 0 || i == newVerts.Count || j == newVerts[0].Count)
                    {
                        newVerts[i][j] = new Vector3(i, 0, j);
                    }
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

                //prevent the outer vertices from offsetting
                if (i == 0 || j == 0 || i == newVerts.Count - 1 || j == newVerts[0].Count - 1)
                {
                    newVerts[i][j] = new Vector3(i, 0, j);
                }
            }
        }

        //Transfer new vertices
        verts = newVerts;

        //set triangles set normals set UV
        terrain.Vertices = convertDown(verts);
        terrain.Triangles = setTriangles(terrain);
        terrain.Normals = setNormals(Vector3.up, terrain.Vertices);
        terrain.UV = setUV(terrain);

        terrain.CreateMesh("TerrainMesh");
    }

    /// <summary>
    /// Converts the 2D vertex list into a 1D vertex array
    /// </summary>
    /// <param name="vert2DArray">The 2D list to be converted</param>
    /// <returns>A 1D version of the 2D list</returns>
    public Vector3[] convertDown(List<List<Vector3>> vert2DArray)
    {
        Vector3[] vertArray = new Vector3[vert2DArray.Count * vert2DArray[0].Count];

        int index = 0;
        foreach (List<Vector3> points in vert2DArray)
        {
            foreach(Vector3 point in points){
                vertArray[index] = point;
                index++;
            }
        }
        return vertArray;
    }

    /// <summary>
    /// Converts the 1D Vertex list into a 2D vertex array
    /// </summary>
    /// <param name="vert1DArray">The 1D array to be converted</param>
    /// <returns>A 2D square version of the 1D array</returns>
    public List<List<Vector3>> convertUp(Vector3[] vert1DArray)
    {
        int size = (int)Mathf.Sqrt(vert1DArray.Length);

        List<List<Vector3>> verts = new List<List<Vector3>>();

        for(int i = 0; i < size; i++)
        {
            verts.Add(new List<Vector3>());
            for(int j = 0; j < size; j++)
            {
                verts[i].Add(vert1DArray[(i * size) + j]);
            }
        }

        return verts;
    }
    
    public float generateNoise(float x, float y, int key){
        float noise = 0f;
        return noise;
    }

    public Vector3[] setNormals(Vector3 normal, Vector3[] vertices)
    {
        Vector3[] normals = new Vector3[vertices.Length];
        List<List<Vector3>> verts = convertUp(vertices);

        for(int i = 0; i < verts.Count; i++)
        {
            for(int j = 0; j < verts[0].Count; j++)
            {
                //Get the vectors for each edge

                List<Vector3> edges = new List<Vector3>();

                if(i > 0)
                {
                    edges.Add(verts[i - 1][j] - verts[i][j]);
                }
                
                if(i < verts.Count - 1)
                {
                    edges.Add(verts[i + 1][j] - verts[i][j]);
                }

                if (j > 0)
                {
                    edges.Add(verts[i][j - 1] - verts[i][j]);
                }

                if (j < verts[0].Count - 1)
                {
                    edges.Add(verts[i][j + 1] - verts[i][j]);
                }

                //Get all of the cross products for the correct orientation (No overhangs so the y component should always be positive)
                List<Vector3> crosses = new List<Vector3>();

                for(int x = 0; x < edges.Count; x++)
                {
                    for(int y = x + 1; y < edges.Count; y++)
                    {
                        crosses.Add(Vector3.Cross(edges[x], edges[y]));
                    }
                }

                //Average cross products
                normal = Vector3.zero;

                foreach(Vector3 cross in crosses)
                {
                    if(cross.y >= 0)
                    {
                        normal += cross;
                    }
                    else
                    {
                        normal -= cross;
                    }
                }

                normal = normal / crosses.Count;

                normal.Normalize();

                //Set the normal
                normals[(i * verts[0].Count) + j] = normal;
            }
        }

        return normals;
    }

    public Vector2[] setUV(Terrain terrain)
    {
        List<List<Vector3>> verts = convertUp(terrain.Vertices);
        Vector2[] UV = new Vector2[verts.Count * verts[0].Count];

        for (int i = 0; i < verts.Count; i++)
        {
            for (int j = 0; j < verts[0].Count; j++)
            {
                UV[(i * verts[0].Count) + j] = new Vector2((float)j / (verts[0].Count - 1), (float)i / (verts.Count - 1));
            }
        }

        return UV;
    }

    public int[] setTriangles(Terrain terrain)
    {
        tris = new ArrayList();
        List<List<Vector3>> verts = convertUp(terrain.Vertices);

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

        int[] triangles = new int[tris.Count];

        for (int i = 0; i < tris.Count; i++)
        {
            triangles[i] = (int)tris[i];
        }

        return triangles;
    }

    /// <summary>
    /// Turns the 6 planes into a cube
    /// </summary>
    public void createCube()
    {
        float size = Mathf.Pow(2, generationNumber);
        Vector3 direction = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        for(int i = 0; i < terrains.Length; i++)
        {
            switch (i)
            {
                case 0:
                case 1:
                    direction = new Vector3(1,0,0);
                    rotation = new Vector3(0,0,-90);
                    break;
                case 2:
                case 3:
                    direction = new Vector3(0, 1, 0);
                    rotation = new Vector3(0,0,0);
                    break;
                case 4:
                case 5:
                    direction = new Vector3(0, 0, 1);
                    rotation = new Vector3(90,0,0);
                    break;
            }

            if(i % 2 == 0)
            {
                direction *= -1;
                rotation *= -1;
            }

            terrains[i].TerrainTransform.position = Vector3.zero;
            terrains[i].TerrainTransform.rotation = Quaternion.Euler(0, 0, 0);

            terrains[i].TerrainTransform.Translate(direction * size / 2);
            terrains[i].TerrainTransform.Rotate(rotation);
        }

        terrains[2].TerrainTransform.Rotate(new Vector3(180, 0, 0));
    }

    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.white;
        //for(int i = 0; i < terrains.Length; i++)
        //{
        //    for (int j = 0; j < terrains[i].Vertices.Length; j++)
        //    {
        //        Gizmos.DrawRay(terrains[i].Vertices[j], terrains[i].Normals[j]);
        //    }
        //}
    }
}
