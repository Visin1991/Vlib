using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class MeshData
    {

        public Vector3[] vertices;
        public int[] trianglesVertexIndices;
        public Vector2[] uvs;
        int oneThridTriangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            trianglesVertexIndices = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[vertices.Length];
            int triangleCount = trianglesVertexIndices.Length / 3;

            //Each vertex's normal : add all of the related triangle's normal togather. and then normalize it.
            for (int i = 0; i < triangleCount; i++)
            {
                int triangleVertexIndex = i * 3;
                int vertexIndexA = trianglesVertexIndices[triangleVertexIndex];
                int vertexIndexB = trianglesVertexIndices[triangleVertexIndex + 1];
                int vertexIndexC = trianglesVertexIndices[triangleVertexIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }
            //After we add the averages of related triangle normal. we need to normalize it.
            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }
            return vertexNormals;
        }

        Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 vertexA = vertices[indexA];
            Vector3 vertexB = vertices[indexB];
            Vector3 vertexC = vertices[indexC];

            Vector3 sideAB = vertexB - vertexA;
            Vector3 sideAC = vertexC - vertexA;
            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        /// <summary>
        ///     Each time when we create a triangle we need 3 vertex
        /// each oneThridTrianglesVertexIndex represent the index of one vertex's index in vertices array 
        /// </summary>
        /// <param name="a">trianglesVertexIndices[0] = a; triangle0Vertex0 = vertices[a]</param>
        /// <param name="b">trianglesVertexIndices[1] = b; triangle0Vertex1 = vertices[b]</param>
        /// <param name="c">trianglesVertexIndices[2] = c; triangle0Vertex2 = vertices[c]</param>
        /// <summary>
        ///     triangles0 = Triangle(triangle0Vertex0,triangle0Vertex1,triangle0Vertex2)
        /// </summary>
        public void AddTriangle(int a, int b, int c)
        {
            trianglesVertexIndices[oneThridTriangleIndex] = a;
            trianglesVertexIndices[oneThridTriangleIndex + 1] = b;
            trianglesVertexIndices[oneThridTriangleIndex + 2] = c;
            oneThridTriangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = trianglesVertexIndices;
            mesh.uv = uvs;
            mesh.normals = CalculateNormals();
            //Debug.Log(vertices.Length);
            //Debug.Log(trianglesVertexIndices.Length);
            //Debug.Log(mesh.normals.Length);

            return mesh;
        }
    }

    public static class TerrainMeshGenerator
    {
        public static MeshData GenerateMeshFromHeightMapData(float[,] heightMap, float heightMultiplier, AnimationCurve _meshHeightCurve, int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;
            AnimationCurve meshHeightCurve = new AnimationCurve(_meshHeightCurve.keys);//each mesh should have it's own meshHeightCurve, otherwise we will get some bug.

            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = ((width - 1) / meshSimplificationIncrement) + 1;

            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int vertexIndex = 0;

            for (int y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (int x = 0; x < width; x += meshSimplificationIncrement)
                {

                    lock (meshHeightCurve)
                    {
                        meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, meshHeightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                    }

                    meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                    if (x < width - 1 && y < width - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + verticesPerLine + 1);
                    }
                    vertexIndex++;
                }
            }
            return meshData;
        }

        public static MeshData GenerateMeshFromHeightMapTexture(Texture2D heightMap, int verticesSize, float maxHeight)
        {
            float topLeftX = (verticesSize - 1) / -2f;
            float topLeftZ = (verticesSize - 1) / 2f;

            MeshData meshData = new MeshData(verticesSize, verticesSize);

            int vertexIndex = 0;
            for (int y = 0; y < verticesSize; y += 1)
            {
                for (int x = 0; x < verticesSize; x += 1, vertexIndex++)
                {
                    float u = (float)x / (verticesSize - 1);
                    float v = (float)y / (verticesSize - 1);
                    meshData.uvs[vertexIndex] = new Vector2(u, v);

                    float height = heightMap.GetPixelBilinear(u, v).r * maxHeight;

                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, height, topLeftZ - y);

                    if (x < verticesSize - 1 && y < verticesSize - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesSize + 1, vertexIndex + verticesSize);
                        meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + verticesSize + 1);
                    }
                }
            }
            return meshData;
        }
    }

    public static class PlaneMeshGenerator
    {
        public static Mesh PlaneMash(int numberOfRows)
        {
            Mesh meshyMcMeshFace = new Mesh();
            Vector3[] verts = new Vector3[numberOfRows * numberOfRows];
            Vector2[] uvs = new Vector2[numberOfRows * numberOfRows];

            int numSuqares = numberOfRows - 1;
            int[] tris = new int[numSuqares * numSuqares * 2 * 3];//trianges

            int i = 0;
            int t = 0;

            for (float x = 0; x < numberOfRows; ++x)
            {
                for (float z = 0; z < numberOfRows; ++z)
                {                                         // unity plane default vertices is 11 vertices. 10 squre
                    verts[i].x = 10f * x / (numberOfRows - 1) - 5; //We move the origin to the center of the mesh.
                    verts[i].y = 0;
                    verts[i].z = z * 10f / (numberOfRows - 1) - 5; //Based on the percentgy

                    uvs[i].x = (float)x / (numberOfRows - 1);
                    uvs[i].y = (float)z / (numberOfRows - 1);


                    if (x == numberOfRows - 1 || z == numberOfRows - 1)
                    {
                        ++i;
                        continue;
                    }

                    tris[t] = i;
                    tris[t + 1] = i + 1;
                    tris[t + 2] = i + numberOfRows + 1;

                    tris[t + 3] = i;
                    tris[t + 4] = i + numberOfRows + 1;
                    tris[t + 5] = i + numberOfRows;

                    t += 6;
                    ++i;
                }
            }
            meshyMcMeshFace.name = "The Generated M.D.";
            meshyMcMeshFace.vertices = verts;
            meshyMcMeshFace.uv = uvs;
            meshyMcMeshFace.triangles = tris;
            meshyMcMeshFace.RecalculateBounds();
            meshyMcMeshFace.RecalculateNormals();
            return meshyMcMeshFace;
        }

        public static Mesh PlaneMash2(int numberOfRows)
        {

            float botLeftX = (numberOfRows - 1) / -2f;
            float botLeftZ = (numberOfRows - 1) / -2f;

            Mesh meshyMcMeshFace = new Mesh();
            Vector3[] verts = new Vector3[numberOfRows * numberOfRows];
            Vector2[] uvs = new Vector2[numberOfRows * numberOfRows];

            int numSuqares = numberOfRows - 1;
            int[] tris = new int[numSuqares * numSuqares * 2 * 3];//trianges

            int i = 0;
            int t = 0;

            for (float x = 0; x < numberOfRows; ++x)
            {
                for (float z = 0; z < numberOfRows; ++z)
                {                                         // unity plane default vertices is 11 vertices. 10 squre
                    verts[i].x = botLeftX + x;
                    verts[i].y = 0;
                    verts[i].z = botLeftZ + z; //Based on the percentgy

                    uvs[i].x = (float)x / (numberOfRows - 1);
                    uvs[i].y = (float)z / (numberOfRows - 1);


                    if (x == numberOfRows - 1 || z == numberOfRows - 1)
                    {
                        ++i;
                        continue;
                    }

                    tris[t] = i;
                    tris[t + 1] = i + 1;
                    tris[t + 2] = i + numberOfRows + 1;

                    tris[t + 3] = i;
                    tris[t + 4] = i + numberOfRows + 1;
                    tris[t + 5] = i + numberOfRows;

                    t += 6;
                    ++i;
                }
            }
            meshyMcMeshFace.name = "The Generated M.D.";
            meshyMcMeshFace.vertices = verts;
            meshyMcMeshFace.uv = uvs;
            meshyMcMeshFace.triangles = tris;
            meshyMcMeshFace.RecalculateBounds();
            meshyMcMeshFace.RecalculateNormals();
            return meshyMcMeshFace;
        }
    }
}