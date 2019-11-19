using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SFB;

public class FileExplorer : MonoBehaviour
{
	private Main main = null;

	public void FileExplorerImport()
	{
		var paths = StandaloneFileBrowser.OpenFilePanel("Import", "", "csv", false);
		if (paths.Length > 0)
		{
			StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
		}
	}

	public void FileExplorerExport()
	{
		var path = StandaloneFileBrowser.SaveFilePanel("Export", "", "sample", "csv");
		if (!string.IsNullOrEmpty(path))
		{
			main.Export(path, false);
		}
	}

	private void Start ()
	{
		main = GetComponent<Main>();
	}

	private IEnumerator OutputRoutine(string url)
	{
		var loader = new UnityWebRequest(url);
		yield return loader;
		main.Import(loader.url.Replace("file:///", ""));
	}
}
