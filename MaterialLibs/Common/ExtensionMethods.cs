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

        internal static Vector3 NextVector3(this Random rnd, Vector3 value)
        {
            return new Vector3(rnd.Next(Convert.ToInt32(value.X)), rnd.Next(Convert.ToInt32(value.Y)), rnd.Next(Convert.ToInt32(value.Z)));
        }

        internal static T GetResult<T>(this Nullable<T> value) where T : struct
        {
            if (value.HasValue) return value.Value;
            else return default(T);
        }

        internal static Vector2 ToVector2(this string str)
        {
            try
            {
                float x, y;
                var arr = str.ToLower().Replace("f", "").Split(',');
                if (arr.Length == 1)
                {
                    return new Vector2(float.Parse(arr[0]));
                }
                if (arr.Length == 2)
                {
                    x = float.Parse(arr[0]);
                    y = float.Parse(arr[1]);
                    return new Vector2(x, y);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new ArgumentException($"Cannot convert \"{str}\" to Vector2");
            }
        }

        internal static Vector3 ToVector3(this string str)
        {
            try
            {
                float x, y, z;
                var arr = str.ToLower().Replace("f", "").Split(',');
                if (arr.Length == 1)
                {
                    return new Vector3(float.Parse(arr[0]));
                }
                if (arr.Length == 3)
                {
                    x = float.Parse(arr[0]);
                    y = float.Parse(arr[1]);
                    z = float.Parse(arr[2]);
                    return new Vector3(x, y, z);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new ArgumentException($"Cannot convert \"{str}\" to Vector3");
            }
        }

        internal static Vector4 ToVector4(this string str)
        {
            try
            {
                float w, x, y, z;
                var arr = str.ToLower().Replace("f", "").Split(',');
                if (arr.Length == 1)
                {
                    return new Vector4(float.Parse(arr[0]));
                }
                if (arr.Length == 4)
                {
                    w = float.Parse(arr[0]);
                    x = float.Parse(arr[1]);
                    y = float.Parse(arr[2]);
                    z = float.Parse(arr[3]);
                    return new Vector4(w, x, y, z);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new ArgumentException($"Cannot convert \"{str}\" to Vector4");
            }
        }
    }
}