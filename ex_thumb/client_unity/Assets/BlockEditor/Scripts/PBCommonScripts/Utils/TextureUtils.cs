﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureUtils  {

	public static Texture2D LoadTexture(string filePath) {
 
		Texture2D tex = null;
		byte[] fileData;
 
		if (File.Exists(filePath))     {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.anisoLevel = 8;
			tex.LoadImage(fileData);
		}
		return tex;
	}
}
