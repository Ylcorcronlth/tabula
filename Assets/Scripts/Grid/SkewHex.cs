using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkewHex {
    // # Constants
    private const float _CX = 0.8660254037844386467637231707f;
    private const float _CY = 0.5f;

    // # Fields
    private static SkewHex[] _Neighbors = {
        new SkewHex(1, 0),
        new SkewHex(1, 1),
        new SkewHex(0, 1),
        new SkewHex(-1, 0),
        new SkewHex(-1, -1),
        new SkewHex(0, -1)
    };

    private static SkewCorner[] _Corners = {
        new SkewCorner(0, -1, SkewCorner.Direction.L),
        new SkewCorner(0, 0, SkewCorner.Direction.R),
        new SkewCorner(0, 0, SkewCorner.Direction.L),
        new SkewCorner(-1, 0, SkewCorner.Direction.R),
        new SkewCorner(-1, -1, SkewCorner.Direction.L),
        new SkewCorner(-1, -1, SkewCorner.Direction.R),
    };

    private int _u, _v;

    // # Constructors
    public SkewHex(int u, int v) {
        _u = u;
        _v = v;
    }

    // # Properties
    public int u {
        get { return _u; }
    }

    public int v {
        get { return _v; }
    }

    public Vector2 position {
        get {
            return new Vector2(_CX*(_u - _v), _CY*(_u + _v));
        }
    }

    public IEnumerable<SkewHex> neighbors {
        get {
            for (int i = 0; i < 6; i++) {
                yield return this + _Neighbors[i];
            }
        }
    }

    public IEnumerable<SkewCorner> corners {
        get {
            for (int i = 0; i < 6; i++) {
                SkewCorner offset = _Corners[i];
                yield return new SkewCorner(this + offset.hex, offset.direction);
            }
        }
    }

    // # Indexers
    public int this[int k] {
        get {
            if (k == 0) {
                return _u;
            } else if (k == 1) {
                return _v;
            } else {
                throw new System.IndexOutOfRangeException("Index was out of range. Must be 0, 1, or 2.");
            }
        }
    }

    // # Methods
    public static SkewHex operator+(SkewHex a, SkewHex b) {
        return new SkewHex(a._u + b._u, a._v + b._v);
    }

    public static SkewHex operator-(SkewHex a, SkewHex b) {
        return new SkewHex(a._u - b._u, a._v - b._v);
    }

    public static SkewHex operator*(int k, SkewHex a) {
        return new SkewHex(k*a._u, k*a._v);
    }

    public static SkewHex operator*(SkewHex a, int k) {
        return new SkewHex(k*a._u, k*a._v);
    }

    public static bool operator==(SkewHex a, SkewHex b) {
        return a._u == b._u && a._v == b._v;
    }

    public static bool operator!=(SkewHex a, SkewHex b) {
        return a._u != b._u || a._v != b._v;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            SkewHex b = (SkewHex)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _u;
            result = result*496187739 + _v;
            return result;
        }
    }

    public override string ToString() {
        return "(" + _u + ", " + _v + ")";
    }
}
