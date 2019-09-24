using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SFB;

public class UIManager : MonoBehaviour
{
	public GameObject dataTableLeftPanel = null;
	[SerializeField] GameObject dataTableRightPanel = null;
	[SerializeField] GameObject dataTable = null;
	[SerializeField] GameObject graph = null;
	[SerializeField] GameObject dataTableButton = null;
	[SerializeField] GameObject graphButton = null;

	private Main main = null;

	public void SwitchToGraphTab()
	{
		dataTable.SetActive(false);
		graphButton.SetActive(false);
		graph.SetActive(true);
		dataTableButton.SetActive(true);
	}

	public void SwitchToDataTableTab()
	{
		graph.SetActive(false);
		dataTableButton.SetActive(false);
		dataTable.SetActive(true);
		graphButton.SetActive(true);
	}

	public void ChangeDataTab()
	{
		if (dataTableLeftPanel.activeInHierarchy)
		{
			dataTableLeftPanel.SetActive(false);
			dataTableRightPanel.SetActive(true);
		}
		else
		{
			dataTableRightPanel.SetActive(false);
			dataTableLeftPanel.SetActive(true);
		}
	}

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
			main.Export(path);
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
