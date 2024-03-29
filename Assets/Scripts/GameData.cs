﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	internal class GameData
	{
		public enum GameManagerState
		{
			StartMenu,
			WaitingOthers,
			PlayerJoined,
			Ready,
			Playing,
			GameOver
		}

		public enum Colors
		{
			Orange,
			Blue,
			Purple,
			Green,
			Pink,
			Yellow,
			White,
			LightGray,
			DarkGray,
			DarkestGray
		}

		public static Dictionary<Colors, Color> ColorDict { get; } = new Dictionary<Colors, Color>()
		{
			{ Colors.Orange, new Color32(242, 115, 46, 255) },
			{ Colors.Blue, new Color32(46, 186, 242, 255) },
			{ Colors.Purple, new Color32(160, 46, 242, 255) },
			{ Colors.Green, new Color32(102, 201, 83, 255) },
			{ Colors.Pink, new Color32(245, 78, 148, 255) },
			{ Colors.Yellow, new Color32(250, 231, 85, 255) },
			{ Colors.White, new Color32(255, 255, 255, 255) },
			{ Colors.LightGray, new Color32(230, 230, 230, 255) },
			{ Colors.DarkGray, new Color32(100, 100, 100, 255) },
			{ Colors.DarkestGray, new Color32(50, 50, 50, 255) }
		};

		public static List<Color> BoxColors { get; } = new List<Color>()
		{
			ColorDict[Colors.White],
			ColorDict[Colors.Orange],
			ColorDict[Colors.Blue],
			ColorDict[Colors.Purple],
			ColorDict[Colors.Green],
			ColorDict[Colors.Pink],
			ColorDict[Colors.Yellow]
		};

	}
}
