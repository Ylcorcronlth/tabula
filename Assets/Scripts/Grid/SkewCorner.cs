using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkewCorner {
    // # Constants
	private static Vector2[] _Offsets = {
		new Vector2(0.28867513459481288225457439025097872782f, 0.5f),
		new Vector2(-0.28867513459481288225457439025097872782f, 0.5f)
	};

    // # Fields
    private static SkewHex[] _Adjacent = {
        new SkewHex(0, 0),
        new SkewHex(1, 0),
        new SkewHex(1, 1),
        new SkewHex(0, 0),
        new SkewHex(1, 1),
        new SkewHex(0, 1)
    };

    private static SkewCorner[] _Touches = {
        new SkewCorner(1, 0, Direction.L),
        new SkewCorner(0, 0, Direction.L),
        new SkewCorner(0, -1, Direction.L),
        new SkewCorner(0, 0, Direction.R),
        new SkewCorner(0, 1, Direction.R),
        new SkewCorner(-1, 0, Direction.R)
    };

	private SkewHex _hex;
	private Direction _direction;

    // # Constructors
	public SkewCorner(SkewHex hex, Direction direction) {
		_hex = hex;
		_direction = direction;
	}

    public SkewCorner(int u, int v, Direction direction) {
        _hex = new SkewHex(u, v);
        _direction = direction;
    }

	// # Enums
	public enum Direction { R, L };

    // # Properties
	public SkewHex hex {
		get { return _hex; }
	}

    public int u {
        get { return _hex.u; }
    }

    public int v {
        get { return _hex.v; }
    }

	public Direction direction {
		get { return _direction; }
	}

	public Vector2 position {
		get { return _hex.position + _Offsets[(int)_direction]; }
	}

    public IEnumerable<SkewHex> adjacent {
        get {
            for (int i = 0; i < 3; i++) {
                SkewHex offset = _Adjacent[i + 3*(int)_direction];
                yield return offset + _hex;
            }
        }
    }

    public IEnumerable<SkewCorner> touches {
        get {
            for (int i = 0; i < 3; i++) {
                SkewCorner offset = _Touches[i + 3*(int)_direction];
                yield return new SkewCorner(_hex + offset._hex, offset._direction);
            }
        }
    }

    // # Methods
    public static bool operator==(SkewCorner a, SkewCorner b) {
        return a._hex == b._hex && a._direction == b._direction;
    }

    public static bool operator!=(SkewCorner a, SkewCorner b) {
        return a._hex != b._hex || a._direction != b._direction;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        } else {
            SkewCorner b = (SkewCorner)obj;
            return this == b;
        }
    }

    public override int GetHashCode() {
        unchecked {
            int result = 99989;
            result = result*496187739 + _hex.GetHashCode();
            result = result*496187739 + (int)_direction;
            return result;
        }
    }
}
