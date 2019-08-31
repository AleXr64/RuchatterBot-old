using System;
using System.Security.Cryptography;
using System.Text;

namespace BotWithAPI.Util
{
	public static class StringHelper
	{
		public static string Base64Encode(this string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(this string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static string GetMd5Hash(this string input)
		{
			var md5Hash = MD5.Create();
			// Convert the input string to a byte array and compute the hash.
			var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			var sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for(var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
	}
}
