using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;


public class LoadDDS : MonoBehaviour {

    public static string PrintBytes(byte[] byteArray)
	{
		var sb = new StringBuilder("new byte[] { ");
		for (var i = 0; i < byteArray.Length; i++)
		{
			var b = byteArray[i];
			sb.Append(b);
			if (i < byteArray.Length - 1)
			{
				sb.Append(", ");
			}
		}
		sb.Append(" }");
		return sb.ToString();
	}

    public static bool IsTextureDDS(byte[] ddsBytes)
    {

        if (ddsBytes[4] == 124)
            return true;

        return false;
    }

    public static bool IsTextureCRN(byte[] crnBytes)
    {
        if (System.BitConverter.ToUInt16(new byte[2] { crnBytes[1], crnBytes[0] }, 0) == (('H' << 8) | 'x'))
            return true;

        return false;
    }

    public static TextureFormat GetDDSTextureFormat(byte[] ddsBytes)
    {
        string ddsFormatCheck = Encoding.Default.GetString(new byte[] { ddsBytes[84], ddsBytes[85], ddsBytes[86], ddsBytes[87] });

        switch (ddsFormatCheck)
        {
            case "DXT1":
                return TextureFormat.DXT1;
            case "DXT5":
                return TextureFormat.DXT5;
            default:
                throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");
        }
    }

    public static TextureFormat GetCRNTextureFormat(byte[] crnBytes)
    {
        switch (crnBytes[18])
        {
            case 0:
                return TextureFormat.DXT1Crunched;
            case 2:
                return TextureFormat.DXT5Crunched;
            default:
                throw new Exception("Invalid TextureFormat. Only DXT1Crunched and DXT5Crunched formats are supported by this method.");
        }
    }

    public static Texture2D LoadTextureDXT(byte[] ddsBytes)
    {
        if (!IsTextureDDS(ddsBytes))
            throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

        int height = ddsBytes[13] * 256 + ddsBytes[12];
        int width = ddsBytes[17] * 256 + ddsBytes[16];

        //Subtract the Header
        int DDS_HEADER_SIZE = 128;
        byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
        Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

        Texture2D texture = new Texture2D(width, height, GetDDSTextureFormat(ddsBytes), true);
        texture.LoadRawTextureData(dxtBytes);
        texture.Apply();

        return texture;
    }

    public static Texture2D LoadTextureCRN(byte[] crnBytes)
    {
        if (!IsTextureCRN(new byte[] { crnBytes[0], crnBytes[1] }))
            throw new Exception("Invalid CRN DXTn texture. Unable to read");

        int width = BitConverter.ToUInt16(new byte[2] { crnBytes[13], crnBytes[12] }, 0);
        int height = BitConverter.ToUInt16(new byte[2] { crnBytes[15], crnBytes[14] }, 0);

        Texture2D texture = new Texture2D(width, height, GetCRNTextureFormat(crnBytes), true);
        texture.LoadRawTextureData(crnBytes);
        texture.Apply();

        return texture;
    }

    public static Texture2D LoadTexturePNGorJPG(byte[] bytes)
    {
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(bytes);
        texture.Apply();
        return texture;
    }
}
