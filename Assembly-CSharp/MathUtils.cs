using System;
using UnityEngine;

public class MathUtils
{
	public static float GetQuatLength(Quaternion q)
	{
		return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
	}

	public static Quaternion GetQuatConjugate(Quaternion q)
	{
		return new Quaternion(-q.x, -q.y, -q.z, q.w);
	}

	public static Quaternion GetQuatLog(Quaternion q)
	{
		Quaternion result = q;
		result.w = 0f;
		if (Mathf.Abs(q.w) < 1f)
		{
			float num = Mathf.Acos(q.w);
			float num2 = Mathf.Sin(num);
			if ((double)Mathf.Abs(num2) > 0.0001 && num2 != 0f)
			{
				float num3 = num / num2;
				result.x = q.x * num3;
				result.y = q.y * num3;
				result.z = q.z * num3;
			}
		}
		return result;
	}

	public static Quaternion GetQuatExp(Quaternion q)
	{
		Quaternion result = q;
		float num = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
		float num2 = Mathf.Sin(num);
		result.w = Mathf.Cos(num);
		if ((double)Mathf.Abs(num2) > 0.0001 && num != 0f)
		{
			float num3 = num2 / num;
			result.x = num3 * q.x;
			result.y = num3 * q.y;
			result.z = num3 * q.z;
		}
		return result;
	}

	public static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
	{
		float t2 = 2f * t * (1f - t);
		Quaternion p = MathUtils.Slerp(q0, q1, t);
		Quaternion q2 = MathUtils.Slerp(a0, a1, t);
		return MathUtils.Slerp(p, q2, t2);
	}

	public static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
	{
		Quaternion quatConjugate = MathUtils.GetQuatConjugate(q1);
		Quaternion quatLog = MathUtils.GetQuatLog(quatConjugate * q0);
		Quaternion quatLog2 = MathUtils.GetQuatLog(quatConjugate * q2);
		Quaternion q3;
		q3..ctor(-0.25f * (quatLog.x + quatLog2.x), -0.25f * (quatLog.y + quatLog2.y), -0.25f * (quatLog.z + quatLog2.z), -0.25f * (quatLog.w + quatLog2.w));
		return q1 * MathUtils.GetQuatExp(q3);
	}

	public static float Ease(float t, float k1, float k2)
	{
		float num = 0f;
		float num2 = k1 * 2f / 3.14159274f + k2 - k1 + (1f - k2) * 2f / 3.14159274f;
		if (t < k1)
		{
			if (k1 != 0f)
			{
				num = k1 * 0.636619747f * (Mathf.Sin(t / k1 * 3.14159274f / 2f - 1.57079637f) + 1f);
			}
		}
		else if (t < k2)
		{
			num = 2f * k1 / 3.14159274f + t - k1;
		}
		else if (1f - k2 != 0f)
		{
			num = 2f * k1 / 3.14159274f + k2 - k1 + (1f - k2) * 0.636619747f * Mathf.Sin((t - k2) / (1f - k2) * 3.14159274f / 2f);
		}
		float result = 0f;
		if (num2 != 0f)
		{
			result = num / num2;
		}
		return result;
	}

	public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
	{
		float num = Quaternion.Dot(p, q);
		Quaternion result;
		if ((double)(1f + num) > 1E-05)
		{
			float num4;
			float num5;
			if ((double)(1f - num) > 1E-05)
			{
				float num2 = Mathf.Acos(num);
				float num3 = 1f / Mathf.Sin(num2);
				num4 = Mathf.Sin((1f - t) * num2) * num3;
				num5 = Mathf.Sin(t * num2) * num3;
			}
			else
			{
				num4 = 1f - t;
				num5 = t;
			}
			result.x = num4 * p.x + num5 * q.x;
			result.y = num4 * p.y + num5 * q.y;
			result.z = num4 * p.z + num5 * q.z;
			result.w = num4 * p.w + num5 * q.w;
		}
		else
		{
			float num6 = Mathf.Sin((1f - t) * 3.14159274f * 0.5f);
			float num7 = Mathf.Sin(t * 3.14159274f * 0.5f);
			result.x = num6 * p.x - num7 * p.y;
			result.y = num6 * p.y + num7 * p.x;
			result.z = num6 * p.z - num7 * p.w;
			result.w = p.z;
		}
		return result;
	}
}
