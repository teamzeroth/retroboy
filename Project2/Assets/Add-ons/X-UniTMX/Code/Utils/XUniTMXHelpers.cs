using UnityEngine;
using System.IO;

namespace X_UniTMX.Utils
{
	public static class XUniTMXHelpers
	{
		/// <summary>
		/// Parses a given pathToParse in relation to the mapPath, to be used with Unity's Resource.Load()
		/// </summary>
		/// <param name="mapPath">Map's full path inside Resources folder</param>
		/// <param name="pathToParse">Path to be parsed, usually an image or tileset path</param>
		/// <returns></returns>
		public static string ParsePath(string mapPath, string pathToParse)
		{
			string path = pathToParse;
			string rootPath = mapPath;
			
			if (rootPath.Length > 0 && !rootPath.EndsWith("/"))
				rootPath += Path.AltDirectorySeparatorChar;

			while (path.StartsWith("../"))
			{
				if (rootPath.StartsWith("/"))
					rootPath = string.Concat("/..", rootPath);
				else
					rootPath = rootPath.Replace(Directory.GetParent(rootPath).Name + Path.AltDirectorySeparatorChar, "");
				path = path.Remove(0, 3);
			}
			
			path = Path.GetDirectoryName(path) + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(path);
			if (path.StartsWith("/"))
				path = path.Remove(0, 1);

			if (rootPath.StartsWith("/"))
				rootPath = rootPath.Remove(0, 1);
			
			path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			
			path = string.Concat(rootPath, path);
			
			return path;
		}
	}
}
