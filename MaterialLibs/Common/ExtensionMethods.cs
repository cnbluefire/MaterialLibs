using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Common
{
    internal static class ExtensionMethods
    {
        private static readonly Random rnd = new Random(new Guid().GetHashCode());

        internal static Vector2 ToVector2(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        internal static double RandomNegative(this double value)
        {
            if (rnd.Next(0, 2) == 0) return -value;
            else return value;
        }

        public static Vector3 NextVector3(this Random rnd, Vector3 value)
        {
            return new Vector3(rnd.Next(Convert.ToInt32(value.X)), rnd.Next(Convert.ToInt32(value.Y)), rnd.Next(Convert.ToInt32(value.Z)));
        }
    }
}
