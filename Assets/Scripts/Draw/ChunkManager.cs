using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

	public Color LandColor = Color.white, WaterColor = Color.black;
	public float SScale = 0.25f;
	public float WScale = 0.25f;
	public float Steepness = 4.0f;
	public float NoiseAmount = 1.0f;
	public int ChunkRadius = 4;
	public HexChunk ChunkPrefab;

	private Simplex2D simplex;
	private Worley2D worley;

	// Use this for initialization
	void Start()
	{
		simplex = new Simplex2D();
		worley = new Worley2D();
		for (int v = -ChunkRadius; v <= ChunkRadius; v++) {
			for (int u = -ChunkRadius; u <= ChunkRadius; u++) {
				CreateChunk(u, v);
			}
		}
	}

	private float GetValue(Vector2 position) {
		float s = simplex.GetFractalNoise(SScale*position);
		float w = Steepness*worley.GetNoise(WScale*position);
		w = 1.0f/(1.0f + w*w);
		w = 2.0f*w - 1.0f;
		s = 2.0f*s - 1.0f;
		return w + s*NoiseAmount;
	}

	private Vector3 CalculateCornerOffset(SkewCorner corner) {
		int i = 0;
		Vector3[] positions = new Vector3[3];
		foreach (SkewHex hex in corner.adjacent) {
			positions[i] = hex.position;
			positions[i].z = GetValue(positions[i]);
			i++;
		}
		if (positions[0].z >= 0.0f && positions[1].z >= 0.0f && positions[2].z >= 0.0f) {
			return new Vector3();
		} else if (positions[0].z < 0.0f && positions[1].z < 0.0f && positions[2].z < 0.0f) {
			return new Vector3();
		} else {
			Vector3 normal = Vector3.Cross(positions[1] - positions[0], positions[2] - positions[0]);
			Vector2 gradient = new Vector2(-normal.x/normal.z, -normal.y/normal.z);
			float c = Vector3.Dot(normal, positions[0] - (Vector3)corner.position);
			float t = -c/gradient.sqrMagnitude;
			return t*gradient;
		}
	}

	private void CreateChunk(int u0, int v0) {
		int U0 = u0*ChunkPrefab.Size;
		int V0 = v0*ChunkPrefab.Size;
		// Create new chunk.
		HexChunk chunk = Instantiate<HexChunk>(ChunkPrefab);
		chunk.name = "Chunk (" + u0 + ", " + v0 + ")";
		chunk.transform.localPosition = (new SkewHex(U0, V0)).position;
		chunk.transform.SetParent(transform, false);

		// Fill hex values.
		for (int v = 0; v < ChunkPrefab.Size; v++) {
			for (int u = 0; u < ChunkPrefab.Size; u++) {
				SkewHex hex = new SkewHex(u + U0, v + V0);
				Vector2 pos = hex.position;
				float z = GetValue(pos);
				chunk.SetOffset(u, v, new Vector3(0.0f, 0.0f, 0.0f));
				chunk.SetMask(u, v, true);
				if (z > 0.0f) {
					chunk.SetColor(u, v, 0, LandColor);
					int i = 1;
					foreach (SkewCorner corner in hex.corners) {
						chunk.SetColor(u, v, i, LandColor);
						i++;
					}
				} else {
					chunk.SetColor(u, v, 0, WaterColor);
					int i = 1;
					foreach (SkewCorner corner in hex.corners) {
						chunk.SetColor(u, v, i, WaterColor);
						i++;
					}
				}
			}
		}

		// Fill corner values.
		for (int v = -1; v < ChunkPrefab.Size; v++) {
			for (int u = -1; u < ChunkPrefab.Size; u++) {
				SkewCorner right = new SkewCorner(u + U0, v + V0, SkewCorner.Direction.R);
				SkewCorner left = new SkewCorner(u + U0, v + V0, SkewCorner.Direction.L);
				chunk.SetOffset(u, v, 0, CalculateCornerOffset(right));
				chunk.SetOffset(u, v, 1, CalculateCornerOffset(left));
			}
		}

		chunk.TriangulateMesh();
	}
}
