using System;
using UnityEngine;

namespace mset
{
	public class Util
	{
		public static void cubeLookup(ref float s, ref float t, ref ulong face, Vector3 dir)
		{
			float num = Mathf.Abs(dir.x);
			float num2 = Mathf.Abs(dir.y);
			float num3 = Mathf.Abs(dir.z);
			if (num >= num2 && num >= num3)
			{
				if (dir.x >= 0f)
				{
					face = 0uL;
				}
				else
				{
					face = 1uL;
				}
			}
			else if (num2 >= num && num2 >= num3)
			{
				if (dir.y >= 0f)
				{
					face = 2uL;
				}
				else
				{
					face = 3uL;
				}
			}
			else if (dir.z >= 0f)
			{
				face = 4uL;
			}
			else
			{
				face = 5uL;
			}
			if ((long)face >= 0L && (long)face <= 5L)
			{
				switch (face)
				{
				case 0uL:
					s = 0.5f * ((0f - dir.z) / num + 1f);
					t = 0.5f * ((0f - dir.y) / num + 1f);
					break;
				case 1uL:
					s = 0.5f * (dir.z / num + 1f);
					t = 0.5f * ((0f - dir.y) / num + 1f);
					break;
				case 2uL:
					s = 0.5f * (dir.x / num2 + 1f);
					t = 0.5f * (dir.z / num2 + 1f);
					break;
				case 3uL:
					s = 0.5f * (dir.x / num2 + 1f);
					t = 0.5f * ((0f - dir.z) / num2 + 1f);
					break;
				case 4uL:
					s = 0.5f * (dir.x / num3 + 1f);
					t = 0.5f * ((0f - dir.y) / num3 + 1f);
					break;
				case 5uL:
					s = 0.5f * ((0f - dir.x) / num3 + 1f);
					t = 0.5f * ((0f - dir.y) / num3 + 1f);
					break;
				}
			}
		}

		public static void invCubeLookup(ref Vector3 dst, ref float weight, ulong face, ulong col, ulong row, ulong faceSize)
		{
			float num = 2f / (float)faceSize;
			float num2 = ((float)col + 0.5f) * num - 1f;
			float num3 = ((float)row + 0.5f) * num - 1f;
			if ((long)face >= 0L && (long)face <= 5L)
			{
				switch (face)
				{
				case 0uL:
					dst[0] = 1f;
					dst[1] = 0f - num3;
					dst[2] = 0f - num2;
					break;
				case 1uL:
					dst[0] = -1f;
					dst[1] = 0f - num3;
					dst[2] = num2;
					break;
				case 2uL:
					dst[0] = num2;
					dst[1] = 1f;
					dst[2] = num3;
					break;
				case 3uL:
					dst[0] = num2;
					dst[1] = -1f;
					dst[2] = 0f - num3;
					break;
				case 4uL:
					dst[0] = num2;
					dst[1] = 0f - num3;
					dst[2] = 1f;
					break;
				case 5uL:
					dst[0] = 0f - num2;
					dst[1] = 0f - num3;
					dst[2] = -1f;
					break;
				}
			}
			float magnitude = dst.magnitude;
			weight = 4f / (magnitude * magnitude * magnitude);
			dst /= magnitude;
		}

		public static void invLatLongLookup(ref Vector3 dst, ref float cosPhi, ulong col, ulong row, ulong width, ulong height)
		{
			float num = 0.5f;
			float num2 = ((float)col + num) / (float)width;
			float num3 = ((float)row + num) / (float)height;
			float f = (float)Math.PI * -2f * num2 - (float)Math.PI / 2f;
			float f2 = (float)Math.PI / 2f * (2f * num3 - 1f);
			cosPhi = Mathf.Cos(f2);
			dst.x = Mathf.Cos(f) * cosPhi;
			dst.y = Mathf.Sin(f2);
			dst.z = Mathf.Sin(f) * cosPhi;
		}

		public static void cubeToLatLongLookup(ref float pano_u, ref float pano_v, ulong face, ulong col, ulong row, ulong faceSize)
		{
			Vector3 dst = default(Vector3);
			float weight = -1f;
			invCubeLookup(ref dst, ref weight, face, col, row, faceSize);
			pano_v = Mathf.Asin(dst.y) / (float)Math.PI + 0.5f;
			pano_u = 0.5f * Mathf.Atan2(0f - dst.x, 0f - dst.z) / (float)Math.PI;
			pano_u = Mathf.Repeat(pano_u, 1f);
		}

		public static void latLongToCubeLookup(ref float cube_u, ref float cube_v, ref ulong face, ulong col, ulong row, ulong width, ulong height)
		{
			Vector3 dst = default(Vector3);
			float cosPhi = -1f;
			invLatLongLookup(ref dst, ref cosPhi, col, row, width, height);
			cubeLookup(ref cube_u, ref cube_v, ref face, dst);
		}

		public static void rotationToInvLatLong(out float u, out float v, Quaternion rot)
		{
			u = rot.eulerAngles.y;
			v = rot.eulerAngles.x;
			u = Mathf.Repeat(u, 360f) / 360f;
			v = 1f - Mathf.Repeat(v + 90f, 360f) / 180f;
		}

		public static void dirToLatLong(out float u, out float v, Vector3 dir)
		{
			dir = dir.normalized;
			u = 0.5f * Mathf.Atan2(0f - dir.x, 0f - dir.z) / (float)Math.PI;
			u = Mathf.Repeat(u, 1f);
			v = Mathf.Asin(dir.y) / (float)Math.PI + 0.5f;
			v = 1f - Mathf.Repeat(v, 1f);
		}

		public static void applyGamma(ref Color c, float gamma)
		{
			c.r = Mathf.Pow(c.r, gamma);
			c.g = Mathf.Pow(c.g, gamma);
			c.b = Mathf.Pow(c.b, gamma);
		}

		public static void applyGamma(ref Color[] c, float gamma)
		{
			for (int i = 0; i < c.Length; i++)
			{
				c[i].r = Mathf.Pow(c[i].r, gamma);
				c[i].g = Mathf.Pow(c[i].g, gamma);
				c[i].b = Mathf.Pow(c[i].b, gamma);
			}
		}

		public static void applyGamma(ref Color[] dst, Color[] src, float gamma)
		{
			for (int i = 0; i < src.Length; i++)
			{
				dst[i].r = Mathf.Pow(src[i].r, gamma);
				dst[i].g = Mathf.Pow(src[i].g, gamma);
				dst[i].b = Mathf.Pow(src[i].b, gamma);
				dst[i].a = src[i].a;
			}
		}

		public static void applyGamma(ref Color[] dst, int dst_offset, Color[] src, int src_offset, int count, float gamma)
		{
			for (int i = 0; i < count && i < src.Length; i++)
			{
				dst[i + dst_offset].r = Mathf.Pow(src[i + src_offset].r, gamma);
				dst[i + dst_offset].g = Mathf.Pow(src[i + src_offset].g, gamma);
				dst[i + dst_offset].b = Mathf.Pow(src[i + src_offset].b, gamma);
				dst[i + dst_offset].a = src[i + src_offset].a;
			}
		}

		public static void applyGamma2D(ref Texture2D tex, float gamma)
		{
			for (int i = 0; i < tex.mipmapCount; i++)
			{
				Color[] c = tex.GetPixels(i);
				applyGamma(ref c, gamma);
				tex.SetPixels(c);
			}
			tex.Apply(updateMipmaps: false);
		}

		public static void clearTo(ref Color[] c, Color color)
		{
			for (int i = 0; i < c.Length; i++)
			{
				c[i] = color;
			}
		}

		public static void clearTo2D(ref Texture2D tex, Color color)
		{
			for (int i = 0; i < tex.mipmapCount; i++)
			{
				Color[] c = tex.GetPixels(i);
				clearTo(ref c, color);
				tex.SetPixels(c, i);
			}
			tex.Apply(updateMipmaps: false);
		}

		public static void clearChecker2D(ref Texture2D tex)
		{
			Color color = new Color(0.25f, 0.25f, 0.25f, 0.25f);
			Color color2 = new Color(0.5f, 0.5f, 0.5f, 0.25f);
			Color[] pixels = tex.GetPixels();
			int width = tex.width;
			int height = tex.height;
			int num = height / 4;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (i / num % 2 == j / num % 2)
					{
						pixels[j * width + i] = color;
					}
					else
					{
						pixels[j * width + i] = color2;
					}
				}
			}
			tex.SetPixels(pixels);
			tex.Apply(updateMipmaps: false);
		}

		public static void clearCheckerCube(ref Cubemap cube)
		{
			Color color = new Color(0.25f, 0.25f, 0.25f, 0.25f);
			Color color2 = new Color(0.5f, 0.5f, 0.5f, 0.25f);
			Color[] pixels = cube.GetPixels(CubemapFace.NegativeX);
			int width = cube.width;
			int num = Mathf.Max(1, width / 4);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < width; k++)
					{
						if (j / num % 2 == k / num % 2)
						{
							pixels[k * width + j] = color;
						}
						else
						{
							pixels[k * width + j] = color2;
						}
					}
				}
				cube.SetPixels(pixels, (CubemapFace)i);
			}
			cube.Apply(updateMipmaps: true);
		}
	}
}
