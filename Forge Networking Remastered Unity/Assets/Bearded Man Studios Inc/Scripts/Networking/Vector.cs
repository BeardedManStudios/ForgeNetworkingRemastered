using System;

namespace BeardedManStudios
{
	public struct Vector
	{
		public float x { get; set; }

		public float y { get; set; }

		public float z { get; set; }

		/// <summary>
		/// Get's the magnitude (pythagorean theorem) of this vector (the length
		/// of the hypotenuse of the right triangle produced by this vector)
		/// </summary>
		public float Magnitude
		{
			get { return (float)Math.Sqrt((x * x) + (y * y) + (z * z)); }
		}

        public float SqrMagnitude
        {
            get { return (float)((x * x) + (y * y) + (z * z)); }
        }

        /// <summary>
        /// This will return a new normalized $js.Vector3 of this vector
        /// </summary>
        public Vector Normalized
		{
			get
			{
				var tmp = new Vector(x, y, z);

				var mag = Magnitude;
				tmp.x = tmp.x / mag;
				tmp.y = tmp.y / mag;
				tmp.z = tmp.z / mag;

				return tmp;
			}
		}

		public Vector(float xyz) : this()
		{
			x = y = z = xyz;
		}

		public Vector(float x, float y, float z) : this()
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Get's the dot product of this vector and another
		/// </summary>
		/// <param name="vector">The vector to be multiplied with this vector</param>
		/// <returns>The result of dot product (vector multiplication)</returns>
		public float Dot(Vector vector)
		{
			return (x * vector.x) + (y * vector.y) + (z * vector.z);
		}

		/// <summary>
		/// Get's the cross product of this vector and another
		/// Note: The cross product is often done with a 3 dimensional vector, so in this case it will actually return a scalar
		/// </summary>
		/// <param name="vector">The vector to be multiplied with this vector</param>
		/// <returns>The result of cross product (vector multiplication)</returns>
		public Vector Cross(Vector vector)
		{
			return new Vector(
				(y * vector.z) - (z * vector.y),
				(z * vector.x) - (x * vector.z),
				(x * vector.y) - (y * vector.x)
			);
		}

		/// <summary>
		/// Will get the distance between this vector and another supplied vector
		/// </summary>
		/// <param name="vector">The vector to check the distance to</param>
		/// <returns>The distance between this Vector and the supplied Vector</returns>
		public float Distance(Vector vector)
		{
			return (float)Math.Sqrt(((vector.x - x) * (vector.x - x)) + ((y - vector.y) * (y - vector.y)) + ((z - vector.z) * (z - vector.z)));
		}

        public float DistanceSquared(Vector vector)
        {
            return new Vector(x - vector.x, y - vector.y, z - vector.z).SqrMagnitude;
        }

        public override bool Equals(object obj)
		{
			if (!(obj is Vector))
				return false;

			Vector other = (Vector)obj;
			return x == other.x && y == other.y && z == other.z;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// TODO:  Override the sum and difference opperators
	}
}