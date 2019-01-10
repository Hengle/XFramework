using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XDEDZL.Mathematics
{
    public static class Math2d
    {
        #region 通过Vector3获取Vector2

        public static Vector2 WithoutX(this Vector3 vec)
        {
            return new Vector2(vec.y, vec.z);
        }

        public static Vector2 WithoutY(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector2 WithoutZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        #endregion

        #region 通过Vector2获取Vector3

        public static Vector3 AddX(this Vector2 vec, float value = 0)
        {
            return new Vector3(value, vec.x, vec.y);
        }

        public static Vector3 AddY(this Vector2 vec, float value = 0)
        {
            return new Vector3(vec.x, value, vec.y);
        }

        public static Vector3 AddZ(this Vector2 vec, float value = 0)
        {
            return new Vector3(vec.x, vec.y, value);
        }

        #endregion

        /// <summary>
        /// 判断两线段的是否相交
        /// </summary>
        /// <param name="intersection">交点</param>
        /// <param name="point_0"></param>
        /// <param name="point_1"></param>
        /// <param name="point_2"></param>
        /// <param name="point_3"></param>
        /// <returns></returns>
        public static bool SegmentIntersection(out Vector2 intersection, Vector2 p0, Vector2 p1, Vector2 q0, Vector2 q1)
        {
            // 判断相交性
            Vector3 vec_0 = q1 - q0;   // 线段q的方向
            Vector3 vec_1 = q0 - p0;   // 线段q到p两个端点连线的方向
            Vector3 vec_2 = q0 - p1;
            if (Vector3.Dot(Vector3.Cross(vec_0, vec_1), Vector3.Cross(vec_0, vec_2)) > 0)
            {
                intersection = Vector2.zero;
                return false;
            }




            intersection = Vector2.zero;
            return false;
        }
    }
}
