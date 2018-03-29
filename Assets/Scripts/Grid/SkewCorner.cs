using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkewCorner {
    // # Constants
	private static Vector2[] _Offsets = new Vector2[] {
		new Vector2(0.28867513459481288225457439025097872782f, 0.5f),
		new Vector2(-0.28867513459481288225457439025097872782f, 0.5f)
	};

    // # Fields
	private SkewHex _hex;
	private Direction _direction;

    // # Constructors
	public SkewCorner(SkewHex hex, Direction direction) {
		_hex = hex;
		_direction = direction;
	}

	// # Enums
	public enum Direction { R, L };

    // # Properties
	public SkewHex hex {
		get { return _hex; }
	}

	public Direction direction {
		get { return _direction; }
	}

	public Vector2 position {
		get { return _hex.position + _Offsets[(int)_direction]; }
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
