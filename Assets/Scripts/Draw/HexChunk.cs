using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexChunk : MonoBehaviour {
	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	public int Size;

	private Mesh mesh;
	private bool[,] HexMask;
	private Vector3[,] HexOffsets;
	private Color[,,] HexColors;
	private Vector3[,,] CornerOffsets;

	public void TriangulateMesh() {
		for (int v = 0; v < Size; v++) {
			for (int u = 0; u < Size; u++) {
				if (HexMask[u, v]) {
					TriangulateHex(new SkewHex(u, v));
				}
			}
		}

		FinalizeMesh();

		vertices.Clear();
		triangles.Clear();
		colors.Clear();
	}

	public void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Hex Chunk";

		HexMask = new bool[Size, Size];
		HexOffsets = new Vector3[Size, Size];
		HexColors = new Color[Size, Size, 7];
		CornerOffsets = new Vector3[Size + 1, Size + 1, 2];
	}

	public void SetMask(int u, int v, bool value) {
		HexMask[u, v] = value;
	}

	public void SetOffset(int u, int v, Vector3 value) {
		HexOffsets[u, v] = value;
	}

	public void SetOffset(int u, int v, int w, Vector3 value) {
		CornerOffsets[u + 1, v + 1, w] = value;
	}

	public void SetColor(int u, int v, int direction, Color value) {
		HexColors[u, v, direction] = value;
	}

	private void CreateTriangle(Vector3 r1, Color c1, Vector3 r2, Color c2, Vector3 r3, Color c3) {
		triangles.Add(vertices.Count);
		triangles.Add(vertices.Count + 1);
		triangles.Add(vertices.Count + 2);

		vertices.Add(r1);
		vertices.Add(r2);
		vertices.Add(r3);

		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	private void TriangulateHex(SkewHex hex) {
		Vector3 hex_position = (Vector3)hex.position + HexOffsets[hex.u, hex.v];
		Color hex_color = HexColors[hex.u, hex.v, 0];
		Vector3[] positions = new Vector3[6];
		Color[] colors = new Color[6];

		int i = 0;
		foreach (SkewCorner c in hex.corners) {
			positions[i] = (Vector3)c.position + CornerOffsets[c.u + 1, c.v + 1, (int)c.direction];
			colors[i] = HexColors[hex.u, hex.v, i + 1];
			i++;
		}

		for (i = 0; i < 6; i++) {
			int j = Utils.Mod(i + 1, 6);
			CreateTriangle(hex_position, hex_color, positions[i], colors[i], positions[j], colors[j]);
		}
	}

	private void FinalizeMesh() {
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}
