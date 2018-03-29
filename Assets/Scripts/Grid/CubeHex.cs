using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CubeHex {
    // # Constants
    private const float _QX = 0.8660254037844386467637231707f;
    private const float _RX = 0.5f;
    private const float _RY = 1.0f;

    // # Fields
    // The three coordinates of the hex. q + r + s == 0.
    private int _q, _r, _s;

    // # Constructors
    public CubeHex(int q, int r) {
        _q = q;
        _r = r;
        _s = -q - r;
    }

    public CubeHex (int q, int r, int s) {
        _q = q;
        _r = r;
        _s = s;
    }

    // # Properties
    // Accessors for the coordinates.
    public int q {
        get { return _q; }
    }

    public int r {
        get { return _r; }
    }

    public int s {
        get { return _s; }
    }

    // The Cartesian position of the hex.
    public Vector2 position {
        get {
            return new Vector2(_QX*q + _RX*_r, _RY*_r);
        }
    }

    // The equivalent SkewHex.
    public SkewHex skewed_position {
        get {
            return new SkewHex(_q + _r, _r);
        }
    }

    // # Indexers
    public int this[int k] {
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
    public static CubeHex operator+(CubeHex a, CubeHex b) {
        return new CubeHex(a._q + b._q, a._r + b._r, a._s + b._s);
    }

    public static CubeHex operator-(CubeHex a, CubeHex b) {
        return new CubeHex(a._q - b._q, a._r - b._r, a._s - b._s);
    }

    public static CubeHex operator*(int k, CubeHex a) {
        return new CubeHex(k*a._q, k*a._r, k*a._s);
    }

    public static CubeHex operator*(CubeHex a, int k) {
        return new CubeHex(k*a._q, k*a._r, k*a._s);
    }

    public static bool operator==(CubeHex a, CubeHex b) {
        return a._q == b._q && a._r == b._r;
    }

    public static bool operator!=(CubeHex a, CubeHex b) {
        return a._q != b._q || a._r != b._r;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            CubeHex b = (CubeHex)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _q;
            result = result*496187739 + _r;
            result = result*496187739 + _s;
            return result;
        }
    }

    public override string ToString() {
        return "(" + _q + ", " + _r + ", " + _s + ")";
    }
}
