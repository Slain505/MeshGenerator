using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    
    Vector3[] vertices;
    Vector2[] uvs;
    Color[] colors;
    
    int[] triangles;

    private int xSize = 120;
    private int zSize = 120;

    private float maxTerrainHeight;
    private float minTerrainHeight;
    
    public Gradient gradient;
    
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
    }

    private void Update()
    {
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * Random.Range(0, 2f);
                vertices[i] = new Vector3(x, y, z);
                
                if(y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if(y < minTerrainHeight)
                    minTerrainHeight = y;
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int i = 0; i < xSize; i++)
            {
                triangles[tris + 0] = vert + 0; 
                triangles[tris + 1] = vert + xSize + 1; 
                triangles[tris + 2] = vert + 1; 
                triangles[tris + 3] = vert + 1; 
                triangles[tris + 4] = vert + xSize + 1; 
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            
            vert++;
        }
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        { 
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            } 
        } 
        // uvs mapping
        //
        // uvs = new Vector2[vertices.Length];
        // for (int i = 0, z = 0; z <= zSize; z++)
        // {
        //     for (int x = 0; x <= xSize; x++)
        //     {
        //         uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
        //         i++;
        //     }
        // } 
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        
        mesh.RecalculateNormals();
    }
}
