﻿using System;
using UnityEngine;

/// <summary>
/// 有关3D数学的计算
/// </summary>
public static class Math3d
{
    /// <summary>
    /// 给vector的长度加上size
    /// </summary>
	public static Vector3 AddVectorLength(Vector3 vector, float size)
	{
		float num = Vector3.Magnitude(vector);
		num += size;
		Vector3 a = Vector3.Normalize(vector);
		return Vector3.Scale(a, new Vector3(num, num, num));
	}

	/// <summary>
    /// 将vector的长度设为size
    /// </summary>
	public static Vector3 SetVectorLength(Vector3 vector, float size)
	{
		Vector3 a = Vector3.Normalize(vector);
		return a * size;
	}

    /// <summary>
    /// 两个面是否相交
    /// </summary>
    /// <param name="linePoint">交线上一点</param>
    /// <param name="lineVec">交线的方向</param>
    /// <param name="plane1Normal">平面1法线</param>
    /// <param name="plane1Position">平面1上一点</param>
    /// <param name="plane2Normal">平面2法线</param>
    /// <param name="plane2Position">平面2上一点</param>
    /// <returns>是否相交</returns>
    public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
	{
		linePoint = Vector3.zero;
		lineVec = Vector3.zero;
		lineVec = Vector3.Cross(plane1Normal, plane2Normal);
		Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
		float num = Vector3.Dot(plane1Normal, vector);
		if (Mathf.Abs(num) > 0.006f)
		{
			Vector3 rhs = plane1Position - plane2Position;
			float d = Vector3.Dot(plane1Normal, rhs) / num;
			linePoint = plane2Position + d * vector;
			return true;
		}
		return false;
	}

	/// <summary>
    /// 线面是否相交
    /// </summary>
    /// <param name="intersection">交点</param>
    /// <param name="linePoint"></param>
    /// <param name="lineVec"></param>
    /// <param name="planeNormal"></param>
    /// <param name="planePoint"></param>
    /// <returns>是否相交</returns>
	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{
		intersection = Vector3.zero;
		float num = Vector3.Dot(planePoint - linePoint, planeNormal);
		float num2 = Vector3.Dot(lineVec, planeNormal);
		if (num2 != 0f)
		{
			float size = num / num2;
			Vector3 b = Math3d.SetVectorLength(lineVec, size);
			intersection = linePoint + b;
			return true;
		}
		return false;
	}

	/// <summary>
    /// 判断线与线之间的相交
    /// </summary>
    /// <param name="intersection">交点</param>
    /// <param name="linePoint1">直线1上一点</param>
    /// <param name="lineVec1">直线1方向</param>
    /// <param name="linePoint2">直线2上一点</param>
    /// <param name="lineVec2">直线2方向</param>
    /// <returns>是否相交</returns>
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		intersection = Vector3.zero;
		Vector3 lhs = linePoint2 - linePoint1;
		Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
		Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
		float num = Vector3.Dot(lhs, rhs);
		if (num >= 1E-05f || num <= -1E-05f)
		{
			return false;
		}
		float num2 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
		if (num2 >= 0f && num2 <= 1f)
		{
			intersection = linePoint1 + lineVec1 * num2;
			return true;
		}
		return false;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000BA100 File Offset: 0x000B8300
	public static bool LineLineIntersection(out float tLine1, out float tLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		tLine1 = float.PositiveInfinity;
		tLine2 = float.PositiveInfinity;
		Vector3 lhs = linePoint2 - linePoint1;
		Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
		Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
		Vector3 lhs3 = Vector3.Cross(lhs, lineVec1);
		float num = Vector3.Dot(lhs, rhs);
		if (num >= 1E-05f || num <= -1E-05f)
		{
			return false;
		}
		tLine1 = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
		tLine2 = Vector3.Dot(lhs3, rhs) / rhs.sqrMagnitude;
		return true;
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x000BA184 File Offset: 0x000B8384
	public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
		float num = Vector3.Dot(lineVec1, lineVec1);
		float num2 = Vector3.Dot(lineVec1, lineVec2);
		float num3 = Vector3.Dot(lineVec2, lineVec2);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint1 - linePoint2;
			float num5 = Vector3.Dot(lineVec1, rhs);
			float num6 = Vector3.Dot(lineVec2, rhs);
			float d = (num2 * num6 - num5 * num3) / num4;
			float d2 = (num * num6 - num5 * num2) / num4;
			closestPointLine1 = linePoint1 + lineVec1 * d;
			closestPointLine2 = linePoint2 + lineVec2 * d2;
			return true;
		}
		return false;
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x000BA23C File Offset: 0x000B843C
	public static bool ClosestPointsOnTwoLines(out float s, out float t, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		t = (s = 0f);
		float num = Vector3.Dot(lineVec1, lineVec1);
		float num2 = Vector3.Dot(lineVec1, lineVec2);
		float num3 = Vector3.Dot(lineVec2, lineVec2);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint1 - linePoint2;
			float num5 = Vector3.Dot(lineVec1, rhs);
			float num6 = Vector3.Dot(lineVec2, rhs);
			s = (num2 * num6 - num5 * num3) / num4;
			t = (num * num6 - num5 * num2) / num4;
			return true;
		}
		return false;
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x000BA2C4 File Offset: 0x000B84C4
	public static bool ClosestPointsOnLineSegment(out Vector3 closestPointLine, out Vector3 closestPointSegment, out float lineT, out float segmentT, Vector3 linePoint, Vector3 lineVec, Vector3 segmentPoint1, Vector3 segmentPoint2)
	{
		Vector3 vector = segmentPoint2 - segmentPoint1;
		closestPointLine = Vector3.zero;
		closestPointSegment = Vector3.zero;
		segmentT = 0f;
		lineT = 0f;
		float num = Vector3.Dot(lineVec, lineVec);
		float num2 = Vector3.Dot(lineVec, vector);
		float num3 = Vector3.Dot(vector, vector);
		float num4 = num * num3 - num2 * num2;
		if (num4 != 0f)
		{
			Vector3 rhs = linePoint - segmentPoint1;
			float num5 = Vector3.Dot(lineVec, rhs);
			float num6 = Vector3.Dot(vector, rhs);
			float num7 = (num2 * num6 - num5 * num3) / num4;
			float value = (num * num6 - num5 * num2) / num4;
			lineT = num7;
			segmentT = Mathf.Clamp01(value);
			closestPointLine = linePoint + lineVec * num7;
			closestPointSegment = segmentPoint1 + vector * segmentT;
			return true;
		}
		return false;
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x000BA3A8 File Offset: 0x000B85A8
	public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{
		Vector3 lhs = point - linePoint;
		float d = Vector3.Dot(lhs, lineVec);
		return linePoint + lineVec * d;
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000BA3D4 File Offset: 0x000B85D4
	public static Vector3 ProjectPointOnLine(out float t, Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{
		Vector3 lhs = point - linePoint;
		t = Vector3.Dot(lhs, lineVec);
		return linePoint + lineVec * t;
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x000BA400 File Offset: 0x000B8600
	public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 vector = Math3d.ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
		int num = Math3d.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector);
		if (num == 0)
		{
			return vector;
		}
		if (num == 1)
		{
			return linePoint1;
		}
		if (num == 2)
		{
			return linePoint2;
		}
		return Vector3.zero;
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x000BA44C File Offset: 0x000B864C
	public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		float num = Math3d.SignedDistancePlanePoint(planeNormal, planePoint, point);
		num *= -1f;
		Vector3 b = Math3d.SetVectorLength(planeNormal, num);
		return point + b;
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000BA47C File Offset: 0x000B867C
	public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{
		return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x000BA494 File Offset: 0x000B8694
	public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		return Vector3.Dot(planeNormal, point - planePoint);
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000BA4A4 File Offset: 0x000B86A4
	public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
	{
		Vector3 lhs = Vector3.Cross(normal, vectorA);
		return Vector3.Dot(lhs, vectorB);
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000BA4C4 File Offset: 0x000B86C4
	public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
	{
		Vector3 lhs = Vector3.Cross(normal, referenceVector);
		float num = Vector3.Angle(referenceVector, otherVector);
		return num * Mathf.Sign(Vector3.Dot(lhs, otherVector));
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000BA4F4 File Offset: 0x000B86F4
	public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
	{
		float num = Vector3.Dot(vector, normal);
		float num2 = (float)Math.Acos((double)num);
		return 1.57079637f - num2;
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x000BA51C File Offset: 0x000B871C
	public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
	{
		double num = (double)Vector3.Dot(vec1, vec2);
		if (num < -1.0)
		{
			num = -1.0;
		}
		if (num > 1.0)
		{
			num = 1.0;
		}
		double num2 = Math.Acos(num);
		return (float)num2;
	}

	/// <summary>
    /// 三个点构造一个平面
    /// </summary>
	public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		planeNormal = Vector3.zero;
		planePoint = Vector3.zero;
		Vector3 vector = pointB - pointA;
		Vector3 vector2 = pointC - pointA;
		planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
		Vector3 vector3 = pointA + vector / 2f;
		Vector3 vector4 = pointA + vector2 / 2f;
		Vector3 lineVec = pointC - vector3;
		Vector3 lineVec2 = pointB - vector4;
		Vector3 vector5;
		Math3d.ClosestPointsOnTwoLines(out planePoint, out vector5, vector3, lineVec, vector4, lineVec2);
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000BA5FC File Offset: 0x000B87FC
	public static Vector3 GetForwardVector(Quaternion q)
	{
		return q * Vector3.forward;
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x000BA60C File Offset: 0x000B880C
	public static Vector3 GetUpVector(Quaternion q)
	{
		return q * Vector3.up;
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000BA61C File Offset: 0x000B881C
	public static Vector3 GetRightVector(Quaternion q)
	{
		return q * Vector3.right;
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000BA62C File Offset: 0x000B882C
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x000BA658 File Offset: 0x000B8858
	public static Vector3 PositionFromMatrix(Matrix4x4 m)
	{
		Vector4 column = m.GetColumn(3);
		return new Vector3(column.x, column.y, column.z);
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x000BA688 File Offset: 0x000B8888
	public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
	{
		Quaternion lhs = Quaternion.LookRotation(alignWithVector, alignWithNormal);
		Quaternion rotation = Quaternion.LookRotation(customForward, customUp);
		gameObjectInOut.transform.rotation = lhs * Quaternion.Inverse(rotation);
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000BA6C0 File Offset: 0x000B88C0
	public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
	{
		Math3d.LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
		Vector3 b = gameObjectInOut.transform.TransformPoint(trianglePosition);
		Vector3 translation = alignWithPosition - b;
		gameObjectInOut.transform.Translate(translation, Space.World);
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000BA700 File Offset: 0x000B8900
	private static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
	{
		gameObjectInOut.transform.position = positionVector;
		gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x000BA730 File Offset: 0x000B8930
	public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{
		Vector3 rhs = linePoint2 - linePoint1;
		Vector3 lhs = point - linePoint1;
		float num = Vector3.Dot(lhs, rhs);
		if (num <= 0f)
		{
			return 1;
		}
		if (lhs.magnitude <= rhs.magnitude)
		{
			return 0;
		}
		return 2;
	}

	/// <summary>
    /// 鼠标位置到一条线段的距离
    /// </summary>
	public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 linePoint3 = main.WorldToScreenPoint(linePoint1);
		Vector3 linePoint4 = main.WorldToScreenPoint(linePoint2);
		Vector3 a = Math3d.ProjectPointOnLineSegment(linePoint3, linePoint4, mousePosition);
		a = new Vector3(a.x, a.y, 0f);
		return (a - mousePosition).magnitude;
	}

	/// <summary>
    /// 鼠标位置和圆心位置的距离
    /// </summary>
    /// <param name="point">圆心点</param>
    /// <param name="radius">半径</param>
	public static float MouseDistanceToCircle(Vector3 point, float radius)
	{
		Camera main = Camera.main;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 a = main.WorldToScreenPoint(point);
		a = new Vector3(a.x, a.y, 0f);
		float magnitude = (a - mousePosition).magnitude;
		return magnitude - radius;
	}

    /// <summary>
    /// 判断线段是否在矩形内
    /// </summary>
	public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
	{
		bool flag = false;
		bool flag2 = Math3d.IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
		if (!flag2)
		{
			flag = Math3d.IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
		}
		if (!flag2 && !flag)
		{
			bool flag3 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
			bool flag4 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
			bool flag5 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
			bool flag6 = Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);
			return flag3 || flag4 || flag5 || flag6;
		}
		return true;
	}

	/// <summary>
    /// 判断点point是否在矩形内
    /// </summary>
	public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
	{
		Vector3 vector = rectC - rectA;
		float size = -(vector.magnitude / 2f);
		vector = Math3d.AddVectorLength(vector, size);
		Vector3 linePoint = rectA + vector;
		Vector3 vector2 = rectB - rectA;
		float num = vector2.magnitude / 2f;
		Vector3 vector3 = rectD - rectA;
		float num2 = vector3.magnitude / 2f;
		Vector3 a = Math3d.ProjectPointOnLine(linePoint, vector2.normalized, point);
		float magnitude = (a - point).magnitude;
		a = Math3d.ProjectPointOnLine(linePoint, vector3.normalized, point);
		float magnitude2 = (a - point).magnitude;
		return magnitude2 <= num && magnitude <= num2;
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x000BA974 File Offset: 0x000B8B74
	public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
	{
		Vector3 vector = pointA2 - pointA1;
		Vector3 vector2 = pointB2 - pointB1;
		Vector3 point;
		Vector3 point2;
		bool flag = Math3d.ClosestPointsOnTwoLines(out point, out point2, pointA1, vector.normalized, pointB1, vector2.normalized);
		if (flag)
		{
			int num = Math3d.PointOnWhichSideOfLineSegment(pointA1, pointA2, point);
			int num2 = Math3d.PointOnWhichSideOfLineSegment(pointB1, pointB2, point2);
			return num == 0 && num2 == 0;
		}
		return false;
	}
}