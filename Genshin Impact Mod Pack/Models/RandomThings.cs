﻿using System.Collections.Generic;

namespace Genshin_Impact_Mod.Models
{
	public class SkiffyBotApi
	{
		public string Success { get; set; }
		public string Status { get; set; }
		public string Category { get; set; }
		public string Endpoint { get; set; }
		public string Message { get; set; }
	}

	public class Data
	{
		public string Anime_name { get; set; }
		public string Source_url { get; set; }
		public string Url { get; set; }
	}

	public class NekosBest
	{
		public List<Data> Results { get; set; }
	}

	public class AlexflipnoteApi
	{
		public string File { get; set; }
	}

	public class PurrBot
	{
		public string Link { get; set; }
	}

	public class NekoBot
	{
		public string Message { get; set; }
		public string Color { get; set; }
	}
}