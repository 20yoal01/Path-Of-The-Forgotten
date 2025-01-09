using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO[] _sceneDataSOArray;
    private Dictionary<string, int> _sceneIDToIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        GameManager.Instance.SceneLoader = this;

        PopulateSceneMappings();
    }

    private void PopulateSceneMappings()
    {
        foreach (var sceneDataSO in _sceneDataSOArray)
        {
            _sceneIDToIndexMap[sceneDataSO.UniqueName] = sceneDataSO.SceneIndex;
        }
    }

    public void LoadSceneByIndex(string savedSceneID)
    {
        if (_sceneIDToIndexMap.TryGetValue(savedSceneID, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"No Scene found for ID: {savedSceneID}");
        }
    }

    public async Task LoadSceneByIndexAsync(string savedSceneID)
    {
        if (_sceneIDToIndexMap.TryGetValue(savedSceneID, out int sceneIndex))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                    break;
                }
                await Task.Yield();
            }
        }
        else
        {
            Debug.LogError($"No Scene found for ID: {savedSceneID}");
        }
    }
}
