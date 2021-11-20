using System;
using UnityEngine;

namespace mset
{
	public class RGB
	{
		public static void toRGBM(ref Color32 rgbm, Color color, bool useGamma)
		{
			if (useGamma)
			{
				color.r = Mathf.Pow(color.r, Gamma.toSRGB);
				color.g = Mathf.Pow(color.g, Gamma.toSRGB);
				color.b = Mathf.Pow(color.b, Gamma.toSRGB);
			}
			color *= 355f / (678f * (float)Math.PI);
			float value = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
			value = Mathf.Clamp01(value);
			value = Mathf.Ceil(value * 255f) / 255f;
			if (value > 0f)
			{
				float num = 1f / value;
				color.r = Mathf.Clamp01(color.r * num);
				color.g = Mathf.Clamp01(color.g * num);
				color.b = Mathf.Clamp01(color.b * num);
				rgbm.r = (byte)(color.r * 255f);
				rgbm.g = (byte)(color.g * 255f);
				rgbm.b = (byte)(color.b * 255f);
				rgbm.a = (byte)(value * 255f);
			}
			else
			{
				rgbm.r = (rgbm.g = (rgbm.b = (rgbm.a = 0)));
			}
		}

		public static void toRGBM(ref Color rgbm, Color color, bool useGamma)
		{
			if (useGamma)
			{
				color.r = Mathf.Pow(color.r, Gamma.toSRGB);
				color.g = Mathf.Pow(color.g, Gamma.toSRGB);
				color.b = Mathf.Pow(color.b, Gamma.toSRGB);
			}
			color *= 355f / (678f * (float)Math.PI);
			float value = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
			value = Mathf.Clamp01(value);
			value = Mathf.Ceil(value * 255f) / 255f;
			if (value > 0f)
			{
				float num = 1f / value;
				rgbm.r = Mathf.Clamp01(color.r * num);
				rgbm.g = Mathf.Clamp01(color.g * num);
				rgbm.b = Mathf.Clamp01(color.b * num);
				rgbm.a = value;
			}
			else
			{
				rgbm.r = (rgbm.g = (rgbm.b = (rgbm.a = 0f)));
			}
		}

		public static void fromRGBM(ref Color color, Color32 rgbm, bool useGamma)
		{
			float num = 0.003921569f;
			float num2 = (float)(int)rgbm.a * num;
			color.r = (float)(int)rgbm.r * num;
			color.g = (float)(int)rgbm.g * num;
			color.b = (float)(int)rgbm.b * num;
			color *= num2;
			color *= 6f;
			if (useGamma)
			{
				color.r = Mathf.Pow(color.r, Gamma.toLinear);
				color.g = Mathf.Pow(color.g, Gamma.toLinear);
				color.b = Mathf.Pow(color.b, Gamma.toLinear);
			}
			color.a = 1f;
		}

		public static void fromRGBM(ref Color color, Color rgbm, bool useGamma)
		{
			float a = rgbm.a;
			color = rgbm;
			color *= a;
			color *= 6f;
			if (useGamma)
			{
				color.r = Mathf.Pow(color.r, Gamma.toLinear);
				color.g = Mathf.Pow(color.g, Gamma.toLinear);
				color.b = Mathf.Pow(color.b, Gamma.toLinear);
			}
			color.a = 1f;
		}

		public static void fromXYZ(ref Color rgb, Color xyz)
		{
			rgb.r = 3.2404542f * xyz.r - 1.53713846f * xyz.g - 0.4985314f * xyz.b;
			rgb.g = -0.969266f * xyz.r + 1.87601078f * xyz.g + 0.041556f * xyz.b;
			rgb.b = 0.0556434f * xyz.r - 0.2040259f * xyz.g + 1.05722523f * xyz.b;
		}

		public static void toXYZ(ref Color xyz, Color rgb)
		{
			xyz.r = 0.4124564f * rgb.r + 0.3575761f * rgb.g + 0.1804375f * rgb.b;
			xyz.g = 0.2126729f * rgb.r + 0.7151522f * rgb.g + 0.072175f * rgb.b;
			xyz.b = 0.0193339f * rgb.r + 0.119192f * rgb.g + 0.9503041f * rgb.b;
		}

		public static void toRGBE(ref Color32 rgbe, Color color)
		{
			float f = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
			int value = Mathf.CeilToInt(Mathf.Log(f, 2f));
			value = Mathf.Clamp(value, -128, 127);
			f = Mathf.Pow(2f, value);
			float num = 255f / f;
			rgbe.r = (byte)Mathf.Clamp(color.r * num, 0f, 255f);
			rgbe.g = (byte)Mathf.Clamp(color.g * num, 0f, 255f);
			rgbe.b = (byte)Mathf.Clamp(color.b * num, 0f, 255f);
			rgbe.a = (byte)(value + 128);
		}
	}
}
