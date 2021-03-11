/****************************************************
	文件：PETools.cs
	作者：Harmonie 	
	功能：工具类
*****************************************************/

using System.Collections;
using UnityEngine;

public class PETools
{
   //功能1:获取随机数
   public static int RDInt(int min,int max,System.Random rd = null)
	{
		if (rd == null)
		{
			rd = new System.Random();
		}
		int val = rd.Next(min, max + 1);
		return val;
	}


	
}
