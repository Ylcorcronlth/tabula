
public class Utils
{
	public static float SQRT3 = 1.732050807568877293527446341505872366942805253810380628055f;
	public static float SQRT3_2 = 0.866025403784438646763723170752936183471402626905190314027f;
    private static uint DELTA = 0x9e3779b9;
    private static uint K0 = 0xae32302b, K1 = 0x21ff7bb2, K2 = 0x312191f, K3 = 0xac27248e;

	public static void TinyEncrypt(ref uint x, ref uint y, int iterations = 4)
    {
        // Tiny Encryption Algorithm.
        unchecked
        {
            uint s = 0;
            for (int _ = 0; _ < iterations; _++)
            {
                s += DELTA;
                x += ((y << 4) + K0) ^ (y + s) ^ ((y >> 5) + K1);
                y += ((x << 4) + K2) ^ (x + s) ^ ((x >> 5) + K3);
            }
        }
    }

	public static void TinyEncrypt(ref uint x, ref uint y, uint[] k, int iterations = 4)
	{
		// Tiny Encryption Algorithm.
		unchecked
		{
			uint s = 0;
			for (int _ = 0; _ < iterations; _++)
			{
				s += DELTA;
				x += ((y << 4) + k[0]) ^ (y + s) ^ ((y >> 5) + k[1]);
				y += ((x << 4) + k[2]) ^ (x + s) ^ ((x >> 5) + k[3]);
			}
		}
	}

    public static int Fastfloor(float x) {
        int xi = (int)x;
        if (xi <= x)
        {
            return xi;
        }
        else
        {
            return xi - 1;
        }
    }

    public static int Fastceil(float x) {
        int xi = (int)x;
        if (xi >= x)
        {
            return xi;
        }
        else
        {
            return xi + 1;
        }
    }

	public static int Abs(int x) {
		return (x < 0 ? -x : x);
	}

	public static int Mod(int x, int y) {
		int z = x%y;
		return (z >= 0 ? z : z + y);
	}

	public static float Smoothstep(float t) {
		if (t > 1.0f) {
			return 1.0f;
		} else if (t < 0.0f) {
			return 0.0f;
		} else {
			return (3.0f - 2.0f*t)*t*t;
		}
	}

	public static float Smootherstep(float t) {
		if (t > 1.0f) {
			return 1.0f;
		} else if (t < 0.0f) {
			return 0.0f;
		} else {
			return t*t*t*((6.0f*t - 15.0f)*t + 10.0f);
		}
	}
}
