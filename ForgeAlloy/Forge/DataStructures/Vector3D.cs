using System;

namespace Forge.DataStructures
{
	public struct Vector3D
	{
		public float X { get; set; }

		public float Y { get; set; }

		public float Z { get; set; }

		/// <summary>
		/// Gets the magnitude (Pythagorean theorem) of this vector (the length
		/// of the hypotenuse of the right triangle produced by this vector)
		/// </summary>
		public float Magnitude
		{
			get { return (float)System.Math.Sqrt((X * X) + (Y * Y) + (Z * Z)); }
		}

		public float SqrMagnitude
		{
			get { return (float)((X * X) + (Y * Y) + (Z * Z)); }
		}

		/// <summary>
		/// This will return a new normalized $js.Vector3 of this vector
		/// </summary>
		public Vector3D Normalized
		{
			get
			{
				var tmp = new Vector3D(X, Y, Z);

				var mag = Magnitude;
				tmp.X = tmp.X / mag;
				tmp.Y = tmp.Y / mag;
				tmp.Z = tmp.Z / mag;

				return tmp;
			}
		}

		public Vector3D(float xyz) : this()
		{
			X = Y = Z = xyz;
		}

		public Vector3D(float x, float y, float z) : this()
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Get's the dot product of this vector and another
		/// </summary>
		/// <param name="vector">The vector to be multiplied with this vector</param>
		/// <returns>The result of dot product (vector multiplication)</returns>
		public float Dot(Vector3D vector)
		{
			return (X * vector.X) + (Y * vector.Y) + (Z * vector.Z);
		}

		/// <summary>
		/// Get's the cross product of this vector and another
		/// Note: The cross product is often done with a 3 dimensional vector, so in this case it will actually return a scalar
		/// </summary>
		/// <param name="vector">The vector to be multiplied with this vector</param>
		/// <returns>The result of cross product (vector multiplication)</returns>
		public Vector3D Cross(Vector3D vector)
		{
			return new Vector3D(
				(Y * vector.Z) - (Z * vector.Y),
				(Z * vector.X) - (X * vector.Z),
				(X * vector.Y) - (Y * vector.X)
			);
		}

		/// <summary>
		/// Will get the distance between this vector and another supplied vector
		/// </summary>
		/// <param name="vector">The vector to check the distance to</param>
		/// <returns>The distance between this Vector and the supplied Vector</returns>
		public float Distance(Vector3D vector)
		{
			return (float)System.Math.Sqrt(((vector.X - X) * (vector.X - X)) + ((Y - vector.Y) * (Y - vector.Y)) + ((Z - vector.Z) * (Z - vector.Z)));
		}

		public float DistanceSquared(Vector3D vector)
		{
			return new Vector3D(X - vector.X, Y - vector.Y, Z - vector.Z).SqrMagnitude;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector3D))
				return false;

			Vector3D other = (Vector3D)obj;
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static Vector3D operator +(Vector3D current, Vector3D other)
		{
			return new Vector3D(current.X + other.X, current.Y + other.Y, current.Z + other.Z);
		}

		public static Vector3D operator -(Vector3D current, Vector3D other)
		{
			return new Vector3D(current.X - other.X, current.Y - other.Y, current.Z - other.Z);
		}
	}
}
