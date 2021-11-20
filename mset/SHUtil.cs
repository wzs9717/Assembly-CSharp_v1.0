using UnityEngine;

namespace mset
{
	public class SHUtil
	{
		private static float project_l0_m0(Vector3 u)
		{
			return SHEncoding.sEquationConstants[0];
		}

		private static float project_l1_mneg1(Vector3 u)
		{
			return SHEncoding.sEquationConstants[1] * u.y;
		}

		private static float project_l1_m0(Vector3 u)
		{
			return SHEncoding.sEquationConstants[2] * u.z;
		}

		private static float project_l1_m1(Vector3 u)
		{
			return SHEncoding.sEquationConstants[3] * u.x;
		}

		private static float project_l2_mneg2(Vector3 u)
		{
			return SHEncoding.sEquationConstants[4] * u.y * u.x;
		}

		private static float project_l2_mneg1(Vector3 u)
		{
			return SHEncoding.sEquationConstants[5] * u.y * u.z;
		}

		private static float project_l2_m0(Vector3 u)
		{
			return SHEncoding.sEquationConstants[6] * (3f * u.z * u.z - 1f);
		}

		private static float project_l2_m1(Vector3 u)
		{
			return SHEncoding.sEquationConstants[7] * u.z * u.x;
		}

		private static float project_l2_m2(Vector3 u)
		{
			return SHEncoding.sEquationConstants[8] * (u.x * u.x - u.y * u.y);
		}

		private static void scale(ref SHEncoding sh, float s)
		{
			for (int i = 0; i < 27; i++)
			{
				sh.c[i] *= s;
			}
		}

		public static void projectCubeBuffer(ref SHEncoding sh, CubeBuffer cube)
		{
			sh.clearToBlack();
			float num = 0f;
			ulong num2 = (ulong)cube.faceSize;
			float[] array = new float[9];
			Vector3 dst = Vector3.zero;
			for (ulong num3 = 0uL; num3 < 6; num3++)
			{
				for (ulong num4 = 0uL; num4 < num2; num4++)
				{
					for (ulong num5 = 0uL; num5 < num2; num5++)
					{
						float weight = 1f;
						Util.invCubeLookup(ref dst, ref weight, num3, num5, num4, num2);
						float num6 = 1.33333337f;
						ulong num7 = num3 * num2 * num2 + num4 * num2 + num5;
						Color color = cube.pixels[num7];
						array[0] = project_l0_m0(dst);
						array[1] = project_l1_mneg1(dst);
						array[2] = project_l1_m0(dst);
						array[3] = project_l1_m1(dst);
						array[4] = project_l2_mneg2(dst);
						array[5] = project_l2_mneg1(dst);
						array[6] = project_l2_m0(dst);
						array[7] = project_l2_m1(dst);
						array[8] = project_l2_m2(dst);
						for (int i = 0; i < 9; i++)
						{
							sh.c[3 * i] += num6 * weight * color[0] * array[i];
							sh.c[3 * i + 1] += num6 * weight * color[1] * array[i];
							sh.c[3 * i + 2] += num6 * weight * color[2] * array[i];
						}
						num += weight;
					}
				}
			}
			scale(ref sh, 16f / num);
		}

		public static void projectCube(ref SHEncoding sh, Cubemap cube, int mip, bool hdr)
		{
			sh.clearToBlack();
			float num = 0f;
			ulong num2 = (ulong)cube.width;
			mip = Mathf.Min(QPow.Log2i(num2) + 1, mip);
			num2 >>= mip;
			float[] array = new float[9];
			Vector3 dst = Vector3.zero;
			for (ulong num3 = 0uL; num3 < 6; num3++)
			{
				Color color = Color.black;
				Color[] pixels = cube.GetPixels((CubemapFace)num3, mip);
				for (ulong num4 = 0uL; num4 < num2; num4++)
				{
					for (ulong num5 = 0uL; num5 < num2; num5++)
					{
						float weight = 1f;
						Util.invCubeLookup(ref dst, ref weight, num3, num5, num4, num2);
						float num6 = 1.33333337f;
						ulong num7 = num4 * num2 + num5;
						if (hdr)
						{
							RGB.fromRGBM(ref color, pixels[num7], useGamma: true);
						}
						else
						{
							color = pixels[num7];
						}
						array[0] = project_l0_m0(dst);
						array[1] = project_l1_mneg1(dst);
						array[2] = project_l1_m0(dst);
						array[3] = project_l1_m1(dst);
						array[4] = project_l2_mneg2(dst);
						array[5] = project_l2_mneg1(dst);
						array[6] = project_l2_m0(dst);
						array[7] = project_l2_m1(dst);
						array[8] = project_l2_m2(dst);
						for (int i = 0; i < 9; i++)
						{
							sh.c[3 * i] += num6 * weight * color[0] * array[i];
							sh.c[3 * i + 1] += num6 * weight * color[1] * array[i];
							sh.c[3 * i + 2] += num6 * weight * color[2] * array[i];
						}
						num += weight;
					}
				}
			}
			scale(ref sh, 16f / num);
		}

		public static void convolve(ref SHEncoding sh)
		{
			convolve(ref sh, 1f, 2f / 3f, 0.25f);
		}

		public static void convolve(ref SHEncoding sh, float conv0, float conv1, float conv2)
		{
			for (int i = 0; i < 27; i++)
			{
				if (i < 3)
				{
					sh.c[i] *= conv0;
				}
				else if (i < 12)
				{
					sh.c[i] *= conv1;
				}
				else
				{
					sh.c[i] *= conv2;
				}
			}
		}
	}
}
