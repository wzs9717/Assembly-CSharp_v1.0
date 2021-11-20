using UnityEngine;

namespace mset
{
	public class CubeBuffer
	{
		public enum FilterMode
		{
			NEAREST,
			BILINEAR,
			BICUBIC
		}

		public delegate void SampleFunc(ref Color dst, float u, float v, int face);

		private class CubeEdge
		{
			public int other;

			public bool flipped;

			public bool swizzled;

			public bool mirrored;

			public bool minEdge;

			public CubeEdge(int Other, bool flip, bool swizzle)
			{
				other = Other;
				flipped = flip;
				swizzled = swizzle;
				mirrored = false;
				minEdge = false;
			}

			public CubeEdge(int Other, bool flip, bool swizzle, bool mirror)
			{
				other = Other;
				flipped = flip;
				swizzled = swizzle;
				mirrored = mirror;
				minEdge = false;
			}

			public void transmogrify(ref int primary, ref int secondary, ref int face, int faceSize)
			{
				bool flag = false;
				if (minEdge && primary < 0)
				{
					primary = faceSize + primary;
					flag = true;
				}
				else if (!minEdge && primary >= faceSize)
				{
					primary %= faceSize;
					flag = true;
				}
				if (flag)
				{
					if (mirrored)
					{
						primary = faceSize - primary - 1;
					}
					if (flipped)
					{
						secondary = faceSize - secondary - 1;
					}
					if (swizzled)
					{
						int num = secondary;
						secondary = primary;
						primary = num;
					}
					face = other;
				}
			}

			public void transmogrify(ref int primary_i, ref int primary_j, ref int secondary_i, ref int secondary_j, ref int face_i, ref int face_j, int faceSize)
			{
				if (primary_i < 0)
				{
					primary_i = (primary_j = faceSize - 1);
				}
				else
				{
					primary_i = (primary_j = 0);
				}
				if (mirrored)
				{
					primary_i = faceSize - primary_i - 1;
					primary_j = faceSize - primary_j - 1;
				}
				if (flipped)
				{
					secondary_i = faceSize - secondary_i - 1;
					secondary_j = faceSize - secondary_j - 1;
				}
				if (swizzled)
				{
					int num = secondary_i;
					secondary_i = primary_i;
					primary_i = num;
					num = secondary_j;
					secondary_j = primary_j;
					primary_j = num;
				}
				face_i = (face_j = other);
			}
		}

		public SampleFunc sample;

		private FilterMode _filterMode;

		public int faceSize;

		public Color[] pixels;

		private static CubeEdge[] _leftEdges = null;

		private static CubeEdge[] _rightEdges = null;

		private static CubeEdge[] _upEdges = null;

		private static CubeEdge[] _downEdges = null;

		private static Color[,] cubicKernel = new Color[4, 4];

		public FilterMode filterMode
		{
			get
			{
				return _filterMode;
			}
			set
			{
				_filterMode = value;
				switch (_filterMode)
				{
				case FilterMode.NEAREST:
					sample = sampleNearest;
					break;
				case FilterMode.BILINEAR:
					sample = sampleBilinear;
					break;
				case FilterMode.BICUBIC:
					sample = sampleBicubic;
					break;
				}
			}
		}

		public int width => faceSize;

		public int height => faceSize * 6;

		public CubeBuffer()
		{
			filterMode = FilterMode.BILINEAR;
			clear();
		}

		~CubeBuffer()
		{
		}

		public void clear()
		{
			pixels = null;
			faceSize = 0;
		}

		public bool empty()
		{
			if (pixels == null)
			{
				return true;
			}
			if (pixels.Length == 0)
			{
				return true;
			}
			return false;
		}

		public static void pixelCopy(ref Color[] dst, int dst_offset, Color[] src, int src_offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				ref Color reference = ref dst[dst_offset + i];
				reference = src[src_offset + i];
			}
		}

		public static void pixelCopy(ref Color[] dst, int dst_offset, Color32[] src, int src_offset, int count)
		{
			float num = 0.003921569f;
			for (int i = 0; i < count; i++)
			{
				dst[dst_offset + i].r = (float)(int)src[src_offset + i].r * num;
				dst[dst_offset + i].g = (float)(int)src[src_offset + i].g * num;
				dst[dst_offset + i].b = (float)(int)src[src_offset + i].b * num;
				dst[dst_offset + i].a = (float)(int)src[src_offset + i].a * num;
			}
		}

		public static void pixelCopy(ref Color32[] dst, int dst_offset, Color[] src, int src_offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[dst_offset + i].r = (byte)Mathf.Clamp(src[src_offset + i].r * 255f, 0f, 255f);
				dst[dst_offset + i].g = (byte)Mathf.Clamp(src[src_offset + i].g * 255f, 0f, 255f);
				dst[dst_offset + i].b = (byte)Mathf.Clamp(src[src_offset + i].b * 255f, 0f, 255f);
				dst[dst_offset + i].a = (byte)Mathf.Clamp(src[src_offset + i].a * 255f, 0f, 255f);
			}
		}

		public static void pixelCopyBlock<T>(ref T[] dst, int dst_x, int dst_y, int dst_w, T[] src, int src_x, int src_y, int src_w, int block_w, int block_h, bool flip)
		{
			if (flip)
			{
				for (int i = 0; i < block_w; i++)
				{
					for (int j = 0; j < block_h; j++)
					{
						int num = (dst_y + j) * dst_w + dst_x + i;
						int num2 = (src_y + (block_h - j - 1)) * src_w + src_x + i;
						dst[num] = src[num2];
					}
				}
				return;
			}
			for (int k = 0; k < block_w; k++)
			{
				for (int l = 0; l < block_h; l++)
				{
					int num3 = (dst_y + l) * dst_w + dst_x + k;
					int num4 = (src_y + l) * src_w + src_x + k;
					dst[num3] = src[num4];
				}
			}
		}

		public static void encode(ref Color[] dst, Color[] src, ColorMode outMode, bool useGamma)
		{
			if (outMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < src.Length; i++)
				{
					RGB.toRGBM(ref dst[i], src[i], useGamma);
				}
			}
			else if (useGamma)
			{
				Util.applyGamma(ref dst, src, Gamma.toSRGB);
			}
			else
			{
				pixelCopy(ref dst, 0, src, 0, src.Length);
			}
		}

		public static void encode(ref Color32[] dst, Color[] src, ColorMode outMode, bool useGamma)
		{
			if (outMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < src.Length; i++)
				{
					RGB.toRGBM(ref dst[i], src[i], useGamma);
				}
				return;
			}
			if (useGamma)
			{
				Util.applyGamma(ref src, src, Gamma.toSRGB);
			}
			pixelCopy(ref dst, 0, src, 0, src.Length);
		}

		public static void decode(ref Color[] dst, Color[] src, ColorMode inMode, bool useGamma)
		{
			if (inMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < src.Length; i++)
				{
					RGB.fromRGBM(ref dst[i], src[i], useGamma);
				}
				return;
			}
			if (useGamma)
			{
				Util.applyGamma(ref dst, src, Gamma.toLinear);
			}
			else
			{
				pixelCopy(ref dst, 0, src, 0, src.Length);
			}
			clearAlpha(ref dst);
		}

		public static void decode(ref Color[] dst, Color32[] src, ColorMode inMode, bool useGamma)
		{
			if (inMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < src.Length; i++)
				{
					RGB.fromRGBM(ref dst[i], src[i], useGamma);
				}
				return;
			}
			pixelCopy(ref dst, 0, src, 0, src.Length);
			if (useGamma)
			{
				Util.applyGamma(ref dst, Gamma.toLinear);
			}
			clearAlpha(ref dst);
		}

		public static void decode(ref Color[] dst, int dst_offset, Color[] src, int src_offset, int count, ColorMode inMode, bool useGamma)
		{
			if (inMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < count; i++)
				{
					RGB.fromRGBM(ref dst[i + dst_offset], src[i + src_offset], useGamma);
				}
				return;
			}
			if (useGamma)
			{
				Util.applyGamma(ref dst, dst_offset, src, src_offset, count, Gamma.toLinear);
			}
			else
			{
				pixelCopy(ref dst, dst_offset, src, src_offset, count);
			}
			clearAlpha(ref dst, dst_offset, count);
		}

		public static void decode(ref Color[] dst, int dst_offset, Color32[] src, int src_offset, int count, ColorMode inMode, bool useGamma)
		{
			if (inMode == ColorMode.RGBM8)
			{
				for (int i = 0; i < count; i++)
				{
					RGB.fromRGBM(ref dst[i + dst_offset], src[i + src_offset], useGamma);
				}
				return;
			}
			pixelCopy(ref dst, dst_offset, src, src_offset, count);
			if (useGamma)
			{
				Util.applyGamma(ref dst, dst_offset, dst, dst_offset, count, Gamma.toLinear);
			}
			clearAlpha(ref dst, dst_offset, count);
		}

		public static void clearAlpha(ref Color[] dst)
		{
			clearAlpha(ref dst, 0, dst.Length);
		}

		public static void clearAlpha(ref Color[] dst, int offset, int count)
		{
			for (int i = offset; i < offset + count; i++)
			{
				dst[i].a = 1f;
			}
		}

		public static void clearAlpha(ref Color32[] dst)
		{
			clearAlpha(ref dst, 0, dst.Length);
		}

		public static void clearAlpha(ref Color32[] dst, int offset, int count)
		{
			for (int i = offset; i < offset + count; i++)
			{
				dst[i].a = byte.MaxValue;
			}
		}

		public static void applyExposure(ref Color[] pixels, float mult)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].r *= mult;
				pixels[i].g *= mult;
				pixels[i].b *= mult;
			}
		}

		public void applyExposure(float mult)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].r *= mult;
				pixels[i].g *= mult;
				pixels[i].b *= mult;
			}
		}

		public int toIndex(int face, int x, int y)
		{
			x = Mathf.Clamp(x, 0, faceSize - 1);
			y = Mathf.Clamp(y, 0, faceSize - 1);
			return faceSize * faceSize * face + faceSize * y + x;
		}

		public int toIndex(CubemapFace face, int x, int y)
		{
			x = Mathf.Clamp(x, 0, faceSize - 1);
			y = Mathf.Clamp(y, 0, faceSize - 1);
			return faceSize * faceSize * (int)face + faceSize * y + x;
		}

		private static void linkEdges()
		{
			if (_leftEdges == null)
			{
				_leftEdges = new CubeEdge[6];
				_leftEdges[1] = new CubeEdge(5, flip: false, swizzle: false);
				_leftEdges[0] = new CubeEdge(4, flip: false, swizzle: false);
				_leftEdges[3] = new CubeEdge(1, flip: true, swizzle: true);
				_leftEdges[2] = new CubeEdge(1, flip: false, swizzle: true, mirror: true);
				_leftEdges[5] = new CubeEdge(0, flip: false, swizzle: false);
				_leftEdges[4] = new CubeEdge(1, flip: false, swizzle: false);
				_rightEdges = new CubeEdge[6];
				_rightEdges[1] = new CubeEdge(4, flip: false, swizzle: false);
				_rightEdges[0] = new CubeEdge(5, flip: false, swizzle: false);
				_rightEdges[3] = new CubeEdge(0, flip: false, swizzle: true, mirror: true);
				_rightEdges[2] = new CubeEdge(0, flip: true, swizzle: true);
				_rightEdges[5] = new CubeEdge(1, flip: false, swizzle: false);
				_rightEdges[4] = new CubeEdge(0, flip: false, swizzle: false);
				_upEdges = new CubeEdge[6];
				_upEdges[1] = new CubeEdge(2, flip: false, swizzle: true, mirror: true);
				_upEdges[0] = new CubeEdge(2, flip: true, swizzle: true);
				_upEdges[3] = new CubeEdge(4, flip: false, swizzle: false);
				_upEdges[2] = new CubeEdge(5, flip: true, swizzle: false, mirror: true);
				_upEdges[5] = new CubeEdge(2, flip: true, swizzle: false, mirror: true);
				_upEdges[4] = new CubeEdge(2, flip: false, swizzle: false);
				_downEdges = new CubeEdge[6];
				_downEdges[1] = new CubeEdge(3, flip: true, swizzle: true);
				_downEdges[0] = new CubeEdge(3, flip: false, swizzle: true, mirror: true);
				_downEdges[3] = new CubeEdge(5, flip: true, swizzle: false, mirror: true);
				_downEdges[2] = new CubeEdge(4, flip: false, swizzle: false);
				_downEdges[5] = new CubeEdge(3, flip: true, swizzle: false, mirror: true);
				_downEdges[4] = new CubeEdge(3, flip: false, swizzle: false);
				for (int i = 0; i < 6; i++)
				{
					_leftEdges[i].minEdge = (_upEdges[i].minEdge = true);
					_rightEdges[i].minEdge = (_downEdges[i].minEdge = false);
				}
			}
		}

		public int toIndexLinked(int face, int u, int v)
		{
			linkEdges();
			int face2 = face;
			_leftEdges[face2].transmogrify(ref u, ref v, ref face2, faceSize);
			_upEdges[face2].transmogrify(ref v, ref u, ref face2, faceSize);
			_rightEdges[face2].transmogrify(ref u, ref v, ref face2, faceSize);
			_downEdges[face2].transmogrify(ref v, ref u, ref face2, faceSize);
			u = Mathf.Clamp(u, 0, faceSize - 1);
			v = Mathf.Clamp(v, 0, faceSize - 1);
			return toIndex(face2, u, v);
		}

		public void sampleNearest(ref Color dst, float u, float v, int face)
		{
			int num = Mathf.FloorToInt((float)faceSize * u);
			int num2 = Mathf.FloorToInt((float)faceSize * v);
			dst = pixels[faceSize * faceSize * face + faceSize * num2 + num];
		}

		public void sampleBilinear(ref Color dst, float u, float v, int face)
		{
			u = (float)faceSize * u + 0.5f;
			int num = Mathf.FloorToInt(u) - 1;
			u = Mathf.Repeat(u, 1f);
			v = (float)faceSize * v + 0.5f;
			int num2 = Mathf.FloorToInt(v) - 1;
			v = Mathf.Repeat(v, 1f);
			int num3 = toIndexLinked(face, num, num2);
			int num4 = toIndexLinked(face, num + 1, num2);
			int num5 = toIndexLinked(face, num + 1, num2 + 1);
			int num6 = toIndexLinked(face, num, num2 + 1);
			Color a = Color.Lerp(pixels[num3], pixels[num4], u);
			Color b = Color.Lerp(pixels[num6], pixels[num5], u);
			dst = Color.Lerp(a, b, v);
		}

		public void sampleBicubic(ref Color dst, float u, float v, int face)
		{
			u = (float)faceSize * u + 0.5f;
			int num = Mathf.FloorToInt(u) - 1;
			u = Mathf.Repeat(u, 1f);
			v = (float)faceSize * v + 0.5f;
			int num2 = Mathf.FloorToInt(v) - 1;
			v = Mathf.Repeat(v, 1f);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int num3 = toIndexLinked(face, num - 1 + i, num2 - 1 + j);
					ref Color reference = ref cubicKernel[i, j];
					reference = pixels[num3];
				}
			}
			float t = 0.85f;
			float num4 = 0.333f;
			Color color;
			Color color2;
			Color b;
			Color b2;
			Color b3;
			Color a;
			Color color3;
			for (int k = 0; k < 4; k++)
			{
				b = cubicKernel[0, k];
				color = cubicKernel[1, k];
				color2 = cubicKernel[2, k];
				b2 = cubicKernel[3, k];
				b = Color.Lerp(color, b, t);
				b2 = Color.Lerp(color2, b2, t);
				b = color + num4 * (color - b);
				b2 = color2 + num4 * (color2 - b2);
				a = Color.Lerp(color, b, u);
				color3 = Color.Lerp(b, b2, u);
				b3 = Color.Lerp(b2, color2, u);
				a = Color.Lerp(a, color3, u);
				color3 = Color.Lerp(color3, b3, u);
				ref Color reference2 = ref cubicKernel[0, k];
				reference2 = Color.Lerp(a, color3, u);
			}
			b = cubicKernel[0, 0];
			color = cubicKernel[0, 1];
			color2 = cubicKernel[0, 2];
			b2 = cubicKernel[0, 3];
			b = Color.Lerp(color, b, t);
			b2 = Color.Lerp(color2, b2, t);
			b = color + num4 * (color - b);
			b2 = color2 + num4 * (color2 - b2);
			a = Color.Lerp(color, b, v);
			color3 = Color.Lerp(b, b2, v);
			b3 = Color.Lerp(b2, color2, v);
			a = Color.Lerp(a, color3, v);
			color3 = Color.Lerp(color3, b3, v);
			dst = Color.Lerp(a, color3, v);
		}

		public void resize(int newFaceSize)
		{
			if (newFaceSize != faceSize)
			{
				faceSize = newFaceSize;
				pixels = null;
				pixels = new Color[faceSize * faceSize * 6];
				Util.clearTo(ref pixels, Color.black);
			}
		}

		public void resize(int newFaceSize, Color clearColor)
		{
			resize(newFaceSize);
			Util.clearTo(ref pixels, clearColor);
		}

		public void resample(int newSize)
		{
			if (newSize != faceSize)
			{
				Color[] dst = new Color[newSize * newSize * 6];
				resample(ref dst, newSize);
				pixels = dst;
				faceSize = newSize;
			}
		}

		public void resample(ref Color[] dst, int newSize)
		{
			int num = newSize * newSize;
			float num2 = 1f / (float)newSize;
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < newSize; j++)
				{
					float v = ((float)j + 0.5f) * num2;
					for (int k = 0; k < newSize; k++)
					{
						float u = ((float)k + 0.5f) * num2;
						int num3 = num * i + j * newSize + k;
						sample(ref dst[num3], u, v, i);
					}
				}
			}
		}

		public void resampleFace(ref Color[] dst, int face, int newSize, bool flipY)
		{
			resampleFace(ref dst, 0, face, newSize, flipY);
		}

		public void resampleFace(ref Color[] dst, int dstOffset, int face, int newSize, bool flipY)
		{
			if (newSize == faceSize)
			{
				pixelCopy(ref dst, dstOffset, pixels, face * faceSize * faceSize, faceSize * faceSize);
				return;
			}
			float num = 1f / (float)newSize;
			if (flipY)
			{
				for (int i = 0; i < newSize; i++)
				{
					float v = 1f - ((float)i + 0.5f) * num;
					for (int j = 0; j < newSize; j++)
					{
						float u = ((float)j + 0.5f) * num;
						int num2 = i * newSize + j + dstOffset;
						sample(ref dst[num2], u, v, face);
					}
				}
				return;
			}
			for (int k = 0; k < newSize; k++)
			{
				float v2 = ((float)k + 0.5f) * num;
				for (int l = 0; l < newSize; l++)
				{
					float u2 = ((float)l + 0.5f) * num;
					int num3 = k * newSize + l + dstOffset;
					sample(ref dst[num3], u2, v2, face);
				}
			}
		}

		public void fromCube(Cubemap cube, int mip, ColorMode cubeColorMode, bool useGamma)
		{
			int num = cube.width >> mip;
			if (pixels == null || faceSize != num)
			{
				resize(num);
			}
			for (int i = 0; i < 6; i++)
			{
				Color[] array = cube.GetPixels((CubemapFace)i, mip);
				pixelCopy(ref pixels, i * faceSize * faceSize, array, 0, array.Length);
			}
			decode(ref pixels, pixels, cubeColorMode, useGamma);
		}

		public void toCube(ref Cubemap cube, int mip, ColorMode cubeColorMode, bool useGamma)
		{
			int num = faceSize * faceSize;
			Color[] dst = new Color[num];
			for (int i = 0; i < 6; i++)
			{
				pixelCopy(ref dst, 0, pixels, i * num, num);
				encode(ref dst, dst, cubeColorMode, useGamma);
				cube.SetPixels(dst, (CubemapFace)i, mip);
			}
			cube.Apply(updateMipmaps: false);
		}

		public void resampleToCube(ref Cubemap cube, int mip, ColorMode cubeColorMode, bool useGamma, float exposureMult)
		{
			int num = cube.width >> mip;
			int num2 = num * num * 6;
			Color[] dst = new Color[num2];
			for (int i = 0; i < 6; i++)
			{
				resampleFace(ref dst, i, num, flipY: false);
				if (exposureMult != 1f)
				{
					applyExposure(ref dst, exposureMult);
				}
				encode(ref dst, dst, cubeColorMode, useGamma);
				cube.SetPixels(dst, (CubemapFace)i, mip);
			}
			cube.Apply(updateMipmaps: false);
		}

		public void resampleToBuffer(ref CubeBuffer dst, float exposureMult)
		{
			int num = dst.faceSize * dst.faceSize;
			for (int i = 0; i < 6; i++)
			{
				resampleFace(ref dst.pixels, i * num, i, dst.faceSize, flipY: false);
				dst.applyExposure(exposureMult);
			}
		}

		public void fromBuffer(CubeBuffer src)
		{
			clear();
			faceSize = src.faceSize;
			pixels = new Color[src.pixels.Length];
			pixelCopy(ref pixels, 0, src.pixels, 0, pixels.Length);
		}

		public void fromPanoTexture(Texture2D tex, int _faceSize, ColorMode texColorMode, bool useGamma)
		{
			resize(_faceSize);
			ulong num = (ulong)faceSize;
			for (ulong num2 = 0uL; num2 < 6; num2++)
			{
				for (ulong num3 = 0uL; num3 < num; num3++)
				{
					for (ulong num4 = 0uL; num4 < num; num4++)
					{
						float pano_u = 0f;
						float pano_v = 0f;
						Util.cubeToLatLongLookup(ref pano_u, ref pano_v, num2, num4, num3, num);
						float num5 = 1f / (float)faceSize;
						pano_v = Mathf.Clamp(pano_v, num5, 1f - num5);
						ref Color reference = ref pixels[num2 * num * num + num3 * num + num4];
						reference = tex.GetPixelBilinear(pano_u, pano_v);
					}
				}
			}
			decode(ref pixels, pixels, texColorMode, useGamma);
		}

		public void fromColTexture(Texture2D tex, ColorMode texColorMode, bool useGamma)
		{
			fromColTexture(tex, 0, texColorMode, useGamma);
		}

		public void fromColTexture(Texture2D tex, int mip, ColorMode texColorMode, bool useGamma)
		{
			if (tex.width * 6 != tex.height)
			{
				Debug.LogError("CubeBuffer.fromColTexture takes textures of a 1x6 aspect ratio");
				return;
			}
			int num = tex.width >> mip;
			if (pixels == null || faceSize != num)
			{
				resize(num);
			}
			Color32[] dst = tex.GetPixels32(mip);
			if ((float)(int)dst[0].a != 1f)
			{
				clearAlpha(ref dst);
			}
			decode(ref pixels, dst, texColorMode, useGamma);
		}

		public void fromHorizCrossTexture(Texture2D tex, ColorMode texColorMode, bool useGamma)
		{
			fromHorizCrossTexture(tex, 0, texColorMode, useGamma);
		}

		public void fromHorizCrossTexture(Texture2D tex, int mip, ColorMode texColorMode, bool useGamma)
		{
			if (tex.width * 3 != tex.height * 4)
			{
				Debug.LogError("CubeBuffer.fromHorizCrossTexture takes textures of a 4x3 aspect ratio");
				return;
			}
			int num = tex.width / 4 >> mip;
			if (pixels == null || faceSize != num)
			{
				resize(num);
			}
			Color32[] dst = tex.GetPixels32(mip);
			if ((float)(int)dst[0].a != 1f)
			{
				clearAlpha(ref dst);
			}
			Color32[] dst2 = new Color32[faceSize * faceSize];
			for (int i = 0; i < 6; i++)
			{
				CubemapFace cubemapFace = (CubemapFace)i;
				int src_x = 0;
				int src_y = 0;
				int dst_offset = i * faceSize * faceSize;
				switch (cubemapFace)
				{
				case CubemapFace.NegativeX:
					src_x = 0;
					src_y = faceSize;
					break;
				case CubemapFace.NegativeY:
					src_x = faceSize;
					src_y = 0;
					break;
				case CubemapFace.NegativeZ:
					src_x = faceSize * 3;
					src_y = faceSize;
					break;
				case CubemapFace.PositiveX:
					src_x = faceSize * 2;
					src_y = faceSize;
					break;
				case CubemapFace.PositiveY:
					src_x = faceSize;
					src_y = faceSize * 2;
					break;
				case CubemapFace.PositiveZ:
					src_x = faceSize;
					src_y = faceSize;
					break;
				}
				pixelCopyBlock(ref dst2, 0, 0, faceSize, dst, src_x, src_y, faceSize * 4, faceSize, faceSize, flip: true);
				decode(ref pixels, dst_offset, dst2, 0, faceSize * faceSize, texColorMode, useGamma);
			}
		}

		public void toColTexture(ref Texture2D tex, ColorMode texColorMode, bool useGamma)
		{
			if (tex.width != faceSize || tex.height != faceSize * 6)
			{
				tex.Resize(faceSize, 6 * faceSize);
			}
			Color32[] dst = tex.GetPixels32();
			encode(ref dst, pixels, texColorMode, useGamma);
			tex.SetPixels32(dst);
			tex.Apply(updateMipmaps: false);
		}

		public void toPanoTexture(ref Texture2D tex, ColorMode texColorMode, bool useGamma)
		{
			ulong num = (ulong)tex.width;
			ulong num2 = (ulong)tex.height;
			Color[] dst = tex.GetPixels();
			for (ulong num3 = 0uL; num3 < num; num3++)
			{
				for (ulong num4 = 0uL; num4 < num2; num4++)
				{
					float cube_u = 0f;
					float cube_v = 0f;
					ulong face = 0uL;
					Util.latLongToCubeLookup(ref cube_u, ref cube_v, ref face, num3, num4, num, num2);
					sample(ref dst[num4 * num + num3], cube_u, cube_v, (int)face);
				}
			}
			encode(ref dst, dst, texColorMode, useGamma);
			tex.SetPixels(dst);
			tex.Apply(tex.mipmapCount > 1);
		}

		public void toPanoBuffer(ref Color[] buffer, int width, int height)
		{
			ulong num = (ulong)width;
			ulong num2 = (ulong)height;
			for (ulong num3 = 0uL; num3 < num; num3++)
			{
				for (ulong num4 = 0uL; num4 < num2; num4++)
				{
					float cube_u = 0f;
					float cube_v = 0f;
					ulong face = 0uL;
					Util.latLongToCubeLookup(ref cube_u, ref cube_v, ref face, num3, num4, num, num2);
					sample(ref buffer[num4 * num + num3], cube_u, cube_v, (int)face);
				}
			}
		}
	}
}
