using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SFB;

public class FileExplorer : MonoBehaviour
{
	private Main main = null;
	private COM com = null;

	public void FileExplorerImport()
	{
		com.DisableBenchmark();

		var paths = StandaloneFileBrowser.OpenFilePanel("Import", Application.dataPath, "csv", false);
		
		if (paths.Length > 0)
		{
			StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
		}

		com.EnableBenchmark();
	}

	public void FileExplorerExport()
	{
		com.DisableBenchmark();

		var path = StandaloneFileBrowser.SaveFilePanel("Export", Application.dataPath, "sample", "csv");
		
		if (!string.IsNullOrEmpty(path))
		{
			main.Export(path, false);
		}

		com.EnableBenchmark();
	}

	private void Start ()
	{
		main = GetComponent<Main>();
		com = GetComponent<COM>();
	}

	private IEnumerator OutputRoutine(string url)
	{
		var loader = new UnityWebRequest(url);
		yield return loader;
		main.Import(loader.url.Replace("file:///", ""));
	}
}
