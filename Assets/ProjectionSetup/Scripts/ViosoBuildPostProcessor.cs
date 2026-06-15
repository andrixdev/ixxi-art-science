#if UNITY_EDITOR
using System.Linq;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ViosoBuildPostProcessor : IPostprocessBuildWithReport
{
	public int callbackOrder { get { return 0; } }

	public void OnPostprocessBuild(BuildReport report) {

		string viosoPluginPath = Application.dataPath + "/Plugins/Vioso/";
		string outputDest = Path.GetDirectoryName(report.summary.outputPath) + "/" + Application.productName + "_Data/Plugins/x86_64/";

		if (Directory.Exists(viosoPluginPath) && Directory.Exists(outputDest)) {
			var filePaths = Directory.GetFiles(viosoPluginPath);

			foreach (var p in filePaths)
			{
				string extension = p.Split('.').Last();

				if (extension is "vwf" or "ini")
				{
					File.Copy(p, outputDest + Path.GetFileName(p));

					//Remove StreamingAssets path from logged string
					int index = p.IndexOf(viosoPluginPath, StringComparison.Ordinal);
					string cleanPath = (index < 0)
						? p
						: p.Remove(index, viosoPluginPath.Length);

					Debug.Log("Copied " + cleanPath + " to " + outputDest);
				}
			}
		}
	}
}
#endif
