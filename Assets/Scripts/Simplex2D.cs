using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simplex2D {
	public int Iterations = 4;

    private static float SQRT3 = 1.732050807568877293527446341505872366942805253810380628055f;
    private static float MAXVAL = 0.62f;
    private static float RADIUS = 0.866f;

	private uint[] K = { 0xae32302b, 0x21ff7bb2, 0x312191f, 0xac27248e };

	// Gradient table.
	private static Vector2[] GRAD2 = new Vector2[]
	{
		new Vector2(0.5f, 0.5f),
		new Vector2(0.412707f, 0.582478f),
		new Vector2(0.321439f, 0.659346f),
		new Vector2(0.227076f, 0.729864f),
		new Vector2(0.130526f, 0.793353f),
		new Vector2(0.032719f, 0.849202f),
		new Vector2(-0.065403f, 0.896873f),
		new Vector2(-0.162895f, 0.935906f),
		new Vector2(-0.258819f, 0.965926f),
		new Vector2(-0.35225f, 0.986643f),
		new Vector2(-0.442289f, 0.997859f),
		new Vector2(-0.528068f, 0.999465f),
		new Vector2(-0.608761f, 0.991445f),
		new Vector2(-0.683592f, 0.973877f),
		new Vector2(-0.75184f, 0.94693f),
		new Vector2(-0.812847f, 0.910864f),
		new Vector2(-0.866025f, 0.866025f),
		new Vector2(-0.910864f, 0.812847f),
		new Vector2(-0.94693f, 0.75184f),
		new Vector2(-0.973877f, 0.683592f),
		new Vector2(-0.991445f, 0.608761f),
		new Vector2(-0.999465f, 0.528068f),
		new Vector2(-0.997859f, 0.442289f),
		new Vector2(-0.986643f, 0.35225f),
		new Vector2(-0.965926f, 0.258819f),
		new Vector2(-0.935906f, 0.162895f),
		new Vector2(-0.896873f, 0.065403f),
		new Vector2(-0.849202f, -0.032719f),
		new Vector2(-0.793353f, -0.130526f),
		new Vector2(-0.729864f, -0.227076f),
		new Vector2(-0.659346f, -0.321439f),
		new Vector2(-0.582478f, -0.412707f),
		new Vector2(-0.5f, -0.5f),
		new Vector2(-0.412707f, -0.582478f),
		new Vector2(-0.321439f, -0.659346f),
		new Vector2(-0.227076f, -0.729864f),
		new Vector2(-0.130526f, -0.793353f),
		new Vector2(-0.032719f, -0.849202f),
		new Vector2(0.065403f, -0.896873f),
		new Vector2(0.162895f, -0.935906f),
		new Vector2(0.258819f, -0.965926f),
		new Vector2(0.35225f, -0.986643f),
		new Vector2(0.442289f, -0.997859f),
		new Vector2(0.528068f, -0.999465f),
		new Vector2(0.608761f, -0.991445f),
		new Vector2(0.683592f, -0.973877f),
		new Vector2(0.75184f, -0.94693f),
		new Vector2(0.812847f, -0.910864f),
		new Vector2(0.866025f, -0.866025f),
		new Vector2(0.910864f, -0.812847f),
		new Vector2(0.94693f, -0.75184f),
		new Vector2(0.973877f, -0.683592f),
		new Vector2(0.991445f, -0.608761f),
		new Vector2(0.999465f, -0.528068f),
		new Vector2(0.997859f, -0.442289f),
		new Vector2(0.986643f, -0.35225f),
		new Vector2(0.965926f, -0.258819f),
		new Vector2(0.935906f, -0.162895f),
		new Vector2(0.896873f, -0.065403f),
		new Vector2(0.849202f, 0.032719f),
		new Vector2(0.793353f, 0.130526f),
		new Vector2(0.729864f, 0.227076f),
		new Vector2(0.659346f, 0.321439f),
		new Vector2(0.582478f, 0.412707f),
	};

	public Simplex2D()
	{
	}

	public Simplex2D(uint seed) {
		K[0] = seed;
	}

	public Simplex2D(uint[] k)
	{
		K[0] = k[0];
		K[1] = k[1];
		K[2] = k[2];
		K[3] = k[3];
	}

	public void SetK(uint[] k) {
		K[0] = k[0];
		K[1] = k[1];
		K[2] = k[2];
		K[3] = k[3];
	}

    private Vector2 GradientLookup(int i, int j)
    {
        uint ui = (uint)i;
        uint uj = (uint)j;
        Utils.TinyEncrypt(ref ui, ref uj, K);
        return GRAD2[(ui ^ uj) & 0x3f];
    }

    private float SmootherstepDeriv(float t)
    {
        if (t < 0.0f) { t = 0.0f; }
        else if (t > 1.0f) { t = 1.0f; }
        return t * t * ((30.0f * t - 60.0f) * t + 30.0f);
    }

    private float Interpolant(float t)
    {
        return Utils.Smootherstep(1.0f - t);
    }

    private float InterpolantDeriv(float t)
    {
        return -SmootherstepDeriv(1.0f - t);
    }

	public float GetNoise(Vector2 pos) {
		return GetNoise(pos.x, pos.y);
	}

    public float GetNoise(float x, float y)
    {
        return GetNoiseSkewed(x + y/SQRT3, 2.0f*y/SQRT3);
    }

    public Vector2 GetNoiseGradient(float x, float y)
    {
        return GetNoiseGradientSkewed(x + y/SQRT3, 2.0f*y/SQRT3);
    }

    private float GetNoiseSkewed(float x, float y)
    {
		// Find corners.
        int i1, j1, i2, j2, i3, j3;
        i1 = Utils.Fastfloor(x);
        j1 = Utils.Fastfloor(y);
        i2 = i1 + 1;
        j2 = j1 + 1;

        if (x - i1 > y - j1)
        {
            i3 = i1 + 1;
            j3 = j1;
        } else
        {
            i3 = i1;
            j3 = j1 + 1;
        }

		// Compute distances to each corner.
        float dx1, dy1, dx2, dy2, dx3, dy3;
        dx1 = x - i1;
        dy1 = y - j1;
        dx2 = x - i2;
        dy2 = y - j2;
        dx3 = x - i3;
        dy3 = y - j3;
        float d1, d2, d3;
		d1 = Mathf.Sqrt(dx1*dx1 + dy1*dy1 - dx1*dy1)/RADIUS;
		d2 = Mathf.Sqrt(dx2*dx2 + dy2*dy2 - dx2*dy2)/RADIUS;
		d3 = Mathf.Sqrt(dx3*dx3 + dy3*dy3 - dx3*dy3)/RADIUS;
        Vector2 g1 = GradientLookup(i1, j1);
        Vector2 g2 = GradientLookup(i2, j2);
        Vector2 g3 = GradientLookup(i3, j3);

		// Sum contributions and return.
		float n1 = (g1.x*dx1 + g1.y*dy1)*Utils.Smootherstep(1.0f - d1);
		float n2 = (g2.x*dx2 + g2.y*dy2)*Utils.Smootherstep(1.0f - d2);
		float n3 = (g3.x*dx3 + g3.y*dy3)*Utils.Smootherstep(1.0f - d3);
        return (n1 + n2 + n3) / MAXVAL;
    }

    public Vector2 GetNoiseGradientSkewed(Vector2 spos)
    {
        return GetNoiseGradientSkewed(spos.x, spos.y);
    }

    public Vector2 GetNoiseGradientSkewed(float x, float y)
    {
        int i1, j1, i2, j2, i3, j3;
        i1 = Utils.Fastfloor(x);
        j1 = Utils.Fastfloor(y);
        i2 = i1 + 1;
        j2 = j1 + 1;

        if (x - i1 > y - j1)
        {
            i3 = i1 + 1;
            j3 = j1;
        }
        else
        {
            i3 = i1;
            j3 = j1 + 1;
        }

        float dx1, dy1, dx2, dy2, dx3, dy3;
        dx1 = x - i1;
        dy1 = y - j1;
        dx2 = x - i2;
        dy2 = y - j2;
        dx3 = x - i3;
        dy3 = y - j3;
        float d1, d2, d3;
        d1 = Mathf.Sqrt(dx1 * dx1 + dy1 * dy1 - dx1 * dy1) / RADIUS;
        d2 = Mathf.Sqrt(dx2 * dx2 + dy2 * dy2 - dx2 * dy2) / RADIUS;
        d3 = Mathf.Sqrt(dx3 * dx3 + dy3 * dy3 - dx3 * dy3) / RADIUS;
        Vector2 g1 = GradientLookup(i1, j1);
        Vector2 g2 = GradientLookup(i2, j2);
        Vector2 g3 = GradientLookup(i3, j3);

		Vector2 n1 = new Vector2(g1.x, (g1.x + 2.0f*g1.y)/SQRT3)*Interpolant(d1) +
		                   InterpolantDeriv(d1)*(g1.x*dx1 + g1.y*dy1)*new Vector2(dx1 - dy1/2.0f, SQRT3*dy1/2.0f)/RADIUS/d1;
		Vector2 n2 = new Vector2(g2.x, (g2.x + 2.0f*g2.y)/SQRT3)*Interpolant(d2) -
		                   InterpolantDeriv(d2)*(g2.x*dx2 + g2.y*dy2)*new Vector2(dx2 - dy2/2.0f, SQRT3*dy2/2.0f)/RADIUS/d2;
		Vector2 n3 = new Vector2(g3.x, (g3.x + 2.0f*g3.y)/SQRT3)*Interpolant(d3) -
		                   InterpolantDeriv(d3)*(g3.x*dx3 + g3.y*dy3)*new Vector2(dx3 - dy3/2.0f, SQRT3*dy3/2.0f)/RADIUS/d3;
        return (n1 + n2 + n3) / MAXVAL;
    }

	public float GetFractalNoise(Vector2 position, int octaves = 8, float gain = 0.5f, float lacunarity = 2.0f) {
		return GetFractalNoise(position.x, position.y, octaves, gain, lacunarity);
	}

	public float GetFractalNoise(float x, float y, int octaves = 8, float gain = 0.5f, float lacunarity = 2.0f)
	{
		uint ui = 1, uj = 0;
		Utils.TinyEncrypt(ref ui, ref uj);
		float xp = (x + y / SQRT3);
		float yp = (2.0f * y / SQRT3);
		float value = GetNoiseSkewed(xp, yp);
		float maxval = 1.0f;
		float amplitude = 0.5f;
		xp *= lacunarity;
		yp *= lacunarity;
		for (int i = 1; i < octaves; i++)
		{
			value += amplitude * GetNoiseSkewed(xp + (float)ui/(float)(0xffffffff), yp + (float)uj/(float)(0xffffffff));
			maxval += amplitude*amplitude;
			xp *= lacunarity;
			yp *= lacunarity;
			amplitude *= gain;
			ui = ui ^ 1;
			Utils.TinyEncrypt(ref ui, ref uj, K);
		}
		return (1.0f + value/Mathf.Sqrt(maxval))/2.0f;
	}

	public float GetMultifractalNoise(Vector2 position, int octaves, float gain = 0.5f, float lacunarity = 2.0f, float offset = 1.0f, float exponent = 2.0f) {
		return GetMultifractalNoise(position.x, position.y, octaves, gain, lacunarity, offset, exponent);
	}

	public float GetMultifractalNoise(float x, float y, int octaves, float gain = 0.5f, float lacunarity = 2.0f, float offset = 1.0f, float exponent = 2.0f) {
		uint ui = 0, uj = 0;
		float xp = (x + y / SQRT3);
		float yp = (2.0f * y / SQRT3);
		float maxval = 1.0f;
		float amplitude = 0.5f;
		float value = Mathf.Pow(offset - Mathf.Abs(GetNoiseSkewed(xp, yp)), exponent);
		xp *= lacunarity;
		yp *= lacunarity;
		for (int i = 1; i < octaves; i++)
		{
			value += value*amplitude*Mathf.Pow(offset - Mathf.Abs(GetNoiseSkewed(xp + (float)ui/(float)(0xffffffff), yp + (float)uj/(float)(0xffffffff))), exponent);
			maxval += amplitude;
			xp *= lacunarity;
			yp *= lacunarity;
			amplitude *= gain;
			ui = ui ^ 1;
			Utils.TinyEncrypt(ref ui, ref uj, K);
		}
		return value/maxval;
	}

	public Vector2 GetFractalNoiseGradient(float x, float y, float scale, int octaves, float gain = 0.5f, float lacunarity = 2.0f)
	{
		uint ui = 0, uj = 0;
		Vector2 value = new Vector2(0.0f, 0.0f);
		float maxval = 0.0f;
		float amplitude = 1.0f;
		float xp = scale * (x + y / SQRT3);
		float yp = scale * (2.0f * y / SQRT3);
		for (int i = 0; i < octaves; i++)
		{
			value += amplitude * GetNoiseGradientSkewed(xp + scale * (float)ui / (float)(0xffffffff), yp + scale * (float)uj / (float)(0xffffffff));
			maxval += amplitude;
			xp *= lacunarity;
			yp *= lacunarity;
			amplitude *= gain;
			Utils.TinyEncrypt(ref ui, ref uj, K);
		}
		return value/maxval/2.0f;
	}

}
