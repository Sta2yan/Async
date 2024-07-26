using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    private const string LoadingImageUrl = "https://gas-kvas.com/grafic/uploads/posts/2024-01/gas-kvas-com-p-znachok-perezagruzki-na-prozrachnom-fone-10.png";
    private const string LoadingImageResourcePath = "Loading";
    private const int GameSceneIndex = 1;

    [SerializeField] private Image _load;
    [SerializeField] private Image _loadResource;
    [SerializeField] private Scrollbar _scrollbar;

    private float _pastWebImageProgress;
    private float _pastResourceImageProgress;
    
    public async void Start()
    {
        #region GovnoCod

        Progress<float> webImageProgress = new Progress<float>();
        webImageProgress.ProgressChanged += OnWebImageProgressChanged;
        
        Progress<float> resourceImageProgress = new Progress<float>();
        resourceImageProgress.ProgressChanged += OnResourceImageProgressChanged;

        #endregion
        
        Task<Texture2D> webImageTask = LoadWebImageAsync(LoadingImageUrl, webImageProgress);
        Task<Texture2D> resourceImageTask = LoadResourceImageAsync(LoadingImageResourcePath, resourceImageProgress);

        await Task.WhenAll(webImageTask, resourceImageTask);
        
        _load.sprite = CreateSpriteBy(webImageTask.Result);
        _loadResource.sprite = CreateSpriteBy(resourceImageTask.Result);
        
        //
        await Task.Delay(1000);
        //
        
        await LoadSceneAsync(GameSceneIndex);

        Debug.Log("Complete");
    }

    #region GovnoCod

    private void OnWebImageProgressChanged(object sender, float progress)
    {
        float plusProgress = progress - _pastWebImageProgress;
        _scrollbar.value += plusProgress / 2;
        _pastWebImageProgress = progress;
    }
    
    private void OnResourceImageProgressChanged(object sender, float progress)
    {
        float plusProgress = progress - _pastResourceImageProgress;
        _scrollbar.value += plusProgress / 2;
        _pastResourceImageProgress = progress;
    }

    #endregion
    
    private async Task<Texture2D> LoadWebImageAsync(string url, IProgress<float> progress)
    {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        
        UnityWebRequestAsyncOperation task = www.SendWebRequest();
        
        while (task.isDone == false)
        {
            progress.Report(task.progress);
            await Task.Yield();
        }
         
        progress.Report(task.progress);
        return DownloadHandlerTexture.GetContent(www);
    }

    private async Task<Texture2D> LoadResourceImageAsync(string path, IProgress<float> progress)
    {
        ResourceRequest resourceRequest = Resources.LoadAsync<Texture2D>(path);

        while (resourceRequest.isDone == false)
        {
            progress.Report(resourceRequest.progress);
            await Task.Yield();
        }
        
        progress.Report(resourceRequest.progress);
        return (Texture2D) resourceRequest.asset;
    }

    private async Task LoadSceneAsync(int index)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);

        while (asyncOperation.isDone == false)
        {
            await Task.Yield();
        }
    }
    
    private Sprite CreateSpriteBy(Texture2D texture2D)
    {
        return Sprite.Create(texture2D,
            new Rect(0, 0, texture2D.width, texture2D.height),
            new Vector2(0.5f, 0.5f));
    }
}
