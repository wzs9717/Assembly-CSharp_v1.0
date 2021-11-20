using System;

public class LomontFFT
{
	private double[] cosTable;

	private double[] sinTable;

	public int A { get; set; }

	public int B { get; set; }

	public LomontFFT()
	{
		A = 0;
		B = 1;
	}

	public void FFT(double[] data, bool forward)
	{
		int num = data.Length;
		if ((num & (num - 1)) != 0)
		{
			throw new ArgumentException("data length " + num + " in FFT is not a power of 2");
		}
		num /= 2;
		Reverse(data, num);
		double num2 = ((!forward) ? (-B) : B);
		int num3 = 1;
		while (num > num3)
		{
			int num4 = 2 * num3;
			double num5 = num2 * Math.PI / (double)num3;
			double num6 = 1.0;
			double num7 = 0.0;
			double num8 = Math.Cos(num5);
			double num9 = Math.Sin(num5);
			for (int i = 0; i < num4; i += 2)
			{
				for (int j = i; j < 2 * num; j += 2 * num4)
				{
					int num10 = j + num4;
					double num11 = num6 * data[num10] - num7 * data[num10 + 1];
					double num12 = num7 * data[num10] + num6 * data[num10 + 1];
					data[num10] = data[j] - num11;
					data[num10 + 1] = data[j + 1] - num12;
					data[j] += num11;
					data[j + 1] += num12;
				}
				double num13 = num6;
				num6 = num6 * num8 - num7 * num9;
				num7 = num7 * num8 + num13 * num9;
			}
			num3 = num4;
		}
		Scale(data, num, forward);
	}

	public void TableFFT(double[] data, bool forward)
	{
		int num = data.Length;
		if ((num & (num - 1)) != 0)
		{
			throw new ArgumentException("data length " + num + " in FFT is not a power of 2");
		}
		num /= 2;
		Reverse(data, num);
		if (cosTable == null || cosTable.Length != num)
		{
			Initialize(num);
		}
		double num2 = ((!forward) ? (-B) : B);
		int num3 = 1;
		int num4 = 0;
		while (num > num3)
		{
			int num5 = 2 * num3;
			for (int i = 0; i < num5; i += 2)
			{
				double num6 = cosTable[num4];
				double num7 = num2 * sinTable[num4++];
				for (int j = i; j < 2 * num; j += 2 * num5)
				{
					int num8 = j + num5;
					double num9 = num6 * data[num8] - num7 * data[num8 + 1];
					double num10 = num7 * data[num8] + num6 * data[num8 + 1];
					data[num8] = data[j] - num9;
					data[num8 + 1] = data[j + 1] - num10;
					data[j] += num9;
					data[j + 1] += num10;
				}
			}
			num3 = num5;
		}
		Scale(data, num, forward);
	}

	public void RealFFT(double[] data, bool forward)
	{
		int num = data.Length;
		if ((num & (num - 1)) != 0)
		{
			throw new ArgumentException("data length " + num + " in FFT is not a power of 2");
		}
		double num2 = -1.0;
		if (forward)
		{
			TableFFT(data, forward: true);
			num2 = 1.0;
			if (A != 1)
			{
				double num3 = Math.Pow(2.0, (double)(A - 1) / 2.0);
				for (int i = 0; i < data.Length; i++)
				{
					data[i] *= num3;
				}
			}
		}
		double num4 = (double)B * num2 * 2.0 * Math.PI / (double)num;
		double num5 = Math.Cos(num4);
		double num6 = Math.Sin(num4);
		double num7 = num5;
		double num8 = num6;
		for (int j = 1; j <= num / 4; j++)
		{
			int num9 = num / 2 - j;
			double num10 = data[2 * num9];
			double num11 = data[2 * num9 + 1];
			double num12 = data[2 * j];
			double num13 = data[2 * j + 1];
			double num14 = (num12 - num10) * num8;
			double num15 = (num13 + num11) * num7;
			double num16 = (num12 - num10) * num7;
			double num17 = (num13 + num11) * num8;
			double num18 = num12 + num10;
			double num19 = num13 - num11;
			data[2 * j] = 0.5 * (num18 + num2 * (num14 + num15));
			data[2 * j + 1] = 0.5 * (num19 + num2 * (num17 - num16));
			data[2 * num9] = 0.5 * (num18 - num2 * (num15 + num14));
			data[2 * num9 + 1] = 0.5 * (num2 * (num17 - num16) - num19);
			double num20 = num7;
			num7 = num7 * num5 - num8 * num6;
			num8 = num20 * num6 + num8 * num5;
		}
		if (forward)
		{
			double num21 = data[0];
			data[0] += data[1];
			data[1] = num21 - data[1];
			return;
		}
		double num22 = data[0];
		data[0] = 0.5 * (num22 + data[1]);
		data[1] = 0.5 * (num22 - data[1]);
		TableFFT(data, forward: false);
		double num23 = Math.Pow(2.0, (double)(-(A + 1)) / 2.0) * 2.0;
		for (int k = 0; k < data.Length; k++)
		{
			data[k] *= num23;
		}
	}

	private void Scale(double[] data, int n, bool forward)
	{
		if (forward && A != 1)
		{
			double num = Math.Pow(n, (double)(A - 1) / 2.0);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= num;
			}
		}
		if (!forward && A != -1)
		{
			double num2 = Math.Pow(n, (double)(-(A + 1)) / 2.0);
			for (int j = 0; j < data.Length; j++)
			{
				data[j] *= num2;
			}
		}
	}

	private void Initialize(int size)
	{
		cosTable = new double[size];
		sinTable = new double[size];
		int num = 1;
		int num2 = 0;
		while (size > num)
		{
			int num3 = 2 * num;
			double num4 = Math.PI / (double)num;
			double num5 = 1.0;
			double num6 = 0.0;
			double num7 = Math.Sin(num4);
			double num8 = Math.Sin(num4 / 2.0);
			num8 = -2.0 * num8 * num8;
			for (int i = 0; i < num3; i += 2)
			{
				cosTable[num2] = num5;
				sinTable[num2++] = num6;
				double num9 = num5;
				num5 = num5 * num8 - num6 * num7 + num5;
				num6 = num6 * num8 + num9 * num7 + num6;
			}
			num = num3;
		}
	}

	private static void Reverse(double[] data, int n)
	{
		int num = 0;
		int num2 = 0;
		int num3 = n / 2;
		while (true)
		{
			double num4 = data[num + 2];
			data[num + 2] = data[num2 + n];
			data[num2 + n] = num4;
			num4 = data[num + 3];
			data[num + 3] = data[num2 + n + 1];
			data[num2 + n + 1] = num4;
			if (num > num2)
			{
				num4 = data[num];
				data[num] = data[num2];
				data[num2] = num4;
				num4 = data[num + 1];
				data[num + 1] = data[num2 + 1];
				data[num2 + 1] = num4;
				num4 = data[num + n + 2];
				data[num + n + 2] = data[num2 + n + 2];
				data[num2 + n + 2] = num4;
				num4 = data[num + n + 3];
				data[num + n + 3] = data[num2 + n + 3];
				data[num2 + n + 3] = num4;
			}
			num2 += 4;
			if (num2 >= n)
			{
				break;
			}
			int num5 = num3;
			while (num >= num5)
			{
				num -= num5;
				num5 /= 2;
			}
			num += num5;
		}
	}
}
