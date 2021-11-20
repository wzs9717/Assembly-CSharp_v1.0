using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Mp3Importer
{
	public IntPtr handle_mpg;

	public IntPtr errPtr;

	public IntPtr rate;

	public IntPtr channels;

	public IntPtr encoding;

	public IntPtr id3v1;

	public IntPtr id3v2;

	public IntPtr done;

	public int status;

	public int intRate;

	public int intChannels;

	public int intEncoding;

	public int FrameSize;

	public int lengthSamples;

	public AudioClip myClip;

	private const float const_1_div_128_ = 0.0078125f;

	private const float const_1_div_32768_ = 3.05175781E-05f;

	private const double const_1_div_2147483648_ = 4.6566128730773926E-10;

	private AudioClip ImportFile(string filePath)
	{
		MPGImport.mpg123_init();
		handle_mpg = MPGImport.mpg123_new(null, errPtr);
		status = MPGImport.mpg123_open(handle_mpg, filePath);
		MPGImport.mpg123_getformat(handle_mpg, out rate, out channels, out encoding);
		intRate = rate.ToInt32();
		intChannels = channels.ToInt32();
		intEncoding = encoding.ToInt32();
		MPGImport.mpg123_id3(handle_mpg, out id3v1, out id3v2);
		MPGImport.mpg123_format_none(handle_mpg);
		MPGImport.mpg123_format(handle_mpg, intRate, intChannels, 208);
		FrameSize = MPGImport.mpg123_outblock(handle_mpg);
		byte[] array = new byte[FrameSize];
		lengthSamples = MPGImport.mpg123_length(handle_mpg);
		if (lengthSamples / intRate > 3000)
		{
			Debug.LogError("Audio file too big");
			return null;
		}
		if (lengthSamples / intRate > 2000)
		{
			Debug.LogWarning("Large audio file");
		}
		myClip = AudioClip.Create("myClip", lengthSamples, intChannels, intRate, stream: false);
		int num = 0;
		while (MPGImport.mpg123_read(handle_mpg, array, FrameSize, out done) == 0)
		{
			float[] array2 = ByteToFloat(array);
			myClip.SetData(array2, num * array2.Length / 2);
			num++;
		}
		MPGImport.mpg123_close(handle_mpg);
		return myClip;
	}

	public static AudioClip Import(string filePath)
	{
		Mp3Importer mp3Importer = new Mp3Importer();
		return mp3Importer.ImportFile(filePath);
	}

	public float[] IntToFloat(short[] from)
	{
		float[] array = new float[from.Length];
		for (int i = 0; i < from.Length; i++)
		{
			array[i] = (float)from[i] * 3.05175781E-05f;
		}
		return array;
	}

	public short[] ByteToInt16(byte[] buffer)
	{
		short[] result = new short[1];
		int num = buffer.Length;
		if (num % 2 != 0)
		{
			Console.WriteLine("error");
			return result;
		}
		result = new short[num / 2];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(buffer, 0, intPtr, num);
		Marshal.Copy(intPtr, result, 0, result.Length);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public float[] ByteToFloat(byte[] bArray)
	{
		short[] from = ByteToInt16(bArray);
		return IntToFloat(from);
	}
}
