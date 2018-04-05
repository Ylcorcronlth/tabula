using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGridLines : MonoBehaviour {
	private static List<Vector3> vertices = new List<Vector3>();
	private static List<int> triangles = new List<int>();
	private static List<Color> colors = new List<Color>();

	public int Size;
	public Color LineColor;

	private Mesh mesh;
	private bool[,,] HexMask;

	public void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Hex Grid";

		TriangulateMesh();
	}

	public void TriangulateMesh() {
		for (int v = 0; v < Size; v++) {
			for (int u = 0; u < Size; u++) {
				TriangulateHex(new SkewHex(u, v));
			}
		}

		FinalizeMesh();

		vertices.Clear();
		triangles.Clear();
		colors.Clear();
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
		Vector3 hex_position = (Vector3)hex.position;
		Vector3[] positions = new Vector3[6];
		int i = 0;
		foreach (SkewCorner c in hex.corners) {
			positions[i] = (Vector3)c.position;
			i++;
		}

		Color opaque = new Color(LineColor.r, LineColor.g, LineColor.b, 1.0f);
		Color transparent = new Color(LineColor.r, LineColor.g, LineColor.b, 0.0f);

		for (i = 0; i < 6; i++) {
			int j = Utils.Mod(i + 1, 6);
			CreateTriangle(hex_position, opaque, positions[i], transparent, positions[j], transparent);
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
