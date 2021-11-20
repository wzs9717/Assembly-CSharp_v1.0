using System;
using System.IO;

public static class FileChecker
{
	public static bool CheckSignature(string filepath, int signatureSize, string expectedSignature)
	{
		if (string.IsNullOrEmpty(filepath))
		{
			throw new ArgumentException("Must specify a filepath");
		}
		if (string.IsNullOrEmpty(expectedSignature))
		{
			throw new ArgumentException("Must specify a value for the expected file signature");
		}
		using FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		if (fileStream.Length < signatureSize)
		{
			return false;
		}
		byte[] array = new byte[signatureSize];
		int num = signatureSize;
		int num2 = 0;
		while (num > 0)
		{
			int num3 = fileStream.Read(array, num2, num);
			num -= num3;
			num2 += num3;
		}
		string text = BitConverter.ToString(array);
		if (text == expectedSignature)
		{
			return true;
		}
		return false;
	}

	public static bool IsGzipped(string filepath)
	{
		return CheckSignature(filepath, 3, "1F-8B-08");
	}
}
