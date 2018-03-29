using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FractionalCube {
    // # Constants
    private const float _QX = 0.8660254037844386467637231707f;
    private const float _RX = 0.5f;
    private const float _RY = 1.0f;

    // # Fields
    // The three coordinates of the hex. q + r + s == 0.
    private float _q, _r, _s;

    // # Constructors
    public FractionalCube(float q, float r) {
        _q = q;
        _r = r;
        _s = -q - r;
    }

    public FractionalCube (float q, float r, float s) {
        _q = q;
        _r = r;
        _s = s;
    }

    // # Properties
    // Accessors for the coordinates.
    public float q {
        get { return _q; }
    }

    public float r {
        get { return _r; }
    }

    public float s {
        get { return _s; }
    }

    // The Cartesian position of the hex.
    public Vector2 position {
        get {
            return new Vector2(_QX*q + _RX*_r, _RY*_r);
        }
    }

    // Round to an integer hex.
    public CubeHex rounded {
        get {
            int qi = (int)Mathf.Round(_q);
            int ri = (int)Mathf.Round(_r);
            int si = (int)Mathf.Round(-s);
            float dq = _q - qi;
            float dr = _r - ri;
            float ds = _s - si;
            if (dq > dr && dq > ds) {
                qi = -ri - si;
            } else if (dr > ds) {
                ri = -qi - si;
            } else {
                si = -qi - ri;
            }
            return new CubeHex(qi, ri, si);
        }
    }

    // # Indexers
    public float this[int k] {
        get {
            if (k == 0) {
                return _q;
            } else if (k == 1) {
                return _r;
            } else if (k == 2) {
                return _s;
            } else {
                throw new System.IndexOutOfRangeException("Index was out of range. Must be 0, 1, or 2.");
            }
        }
    }

    // # Methods
    public static FractionalCube operator+(FractionalCube a, FractionalCube b) {
        return new FractionalCube(a._q + b._q, a._r + b._r, a._s + b._s);
    }

    public static FractionalCube operator-(FractionalCube a, FractionalCube b) {
        return new FractionalCube(a._q - b._q, a._r - b._r, a._s - b._s);
    }

    public static FractionalCube operator*(int k, FractionalCube a) {
        return new FractionalCube(k*a._q, k*a._r, k*a._s);
    }

    public static FractionalCube operator*(FractionalCube a, int k) {
        return new FractionalCube(k*a._q, k*a._r, k*a._s);
    }

    public static FractionalCube operator*(float k, FractionalCube a) {
        return new FractionalCube(k*a._q, k*a._r, k*a._s);
    }

    public static FractionalCube operator*(FractionalCube a, float k) {
        return new FractionalCube(k*a._q, k*a._r, k*a._s);
    }

    public static bool operator==(FractionalCube a, FractionalCube b) {
        return a._q == b._q && a._r == b._r;
    }

    public static bool operator!=(FractionalCube a, FractionalCube b) {
        return a._q != b._q || a._r != b._r;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            FractionalCube b = (FractionalCube)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _q.GetHashCode();
            result = result*496187739 + _r.GetHashCode();
            result = result*496187739 + _s.GetHashCode();
            return result;
        }
    }

    public override string ToString() {
        return "(" + _q + ", " + _r + ", " + _s + ")";
    }
}
