using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worley2D {
	private uint[] K = { 0xae32302b, 0x21ff7bb2, 0x312191f, 0xac27248e };

	public Worley2D() {
	}

	public Worley2D(uint[] k) {
		K[0] = k[0];
		K[1] = k[1];
		K[2] = k[2];
		K[3] = k[3];
	}

	private Vector2 GetPoint(int i, int j) {
		uint ui = (uint)i;
		uint uj = (uint)j;
		Utils.TinyEncrypt(ref ui, ref uj, 8);
		return new Vector2(
			i + (float)ui/(float)(0xffffffff),
			j + (float)uj/(float)(0xffffffff)
		);
	}

	public float GetNoise(Vector2 point, float c1 = 1.0f, float c2 = 0.0f) {
		int i = Utils.Fastfloor(point.x);
		int j = Utils.Fastfloor(point.y);
		float[] distances = {
			(GetPoint(i, j) - point).sqrMagnitude,
			(GetPoint(i + 1, j) - point).sqrMagnitude,
			(GetPoint(i, j + 1) - point).sqrMagnitude,
			(GetPoint(i - 1, j) - point).sqrMagnitude,
			(GetPoint(i, j - 1) - point).sqrMagnitude,
			(GetPoint(i + 1, j + 1) - point).sqrMagnitude,
			(GetPoint(i - 1, j + 1) - point).sqrMagnitude,
			(GetPoint(i + 1, j - 1) - point).sqrMagnitude,
			(GetPoint(i - 1, j - 1) - point).sqrMagnitude,
		};

		float r1 = 16.0f;
		float r2 = 16.0f;
		for (int k = 0; k <= 8; k++) {
			if (distances[k] < r1) {
				r2 = r1;
				r1 = distances[k];
			} else if (distances[k] < r2) {
				r2 = distances[k];
			}
		}

		return c1*Mathf.Sqrt(r1) + c2*Mathf.Sqrt(r2);
	}
}
