using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationAsync : MonoBehaviour
{
    private const int CountClicksToSkipAnimation = 3;
    
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;
    
    private int _clicks;
    private Task _currentTask;
    private CancellationTokenSource _source;
    
    private async void Start()
    {
        _source = new CancellationTokenSource();

        _currentTask = StartAnimation(3f, _source.Token);

        try
        {
            await _currentTask;
        }
        catch
        {
            Debug.Log("Skip animation");
            _root.position = _endPosition.position;
            return;
        }

        //
        Debug.Log("Complete");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clicks++;

            if (_clicks == CountClicksToSkipAnimation)
            {
                _clicks = 0;
                SkipAnimation();
            }
        }
    }

    private async Task StartAnimation(float duration, CancellationToken ctn)
    {
        _root.position = _startPosition.position;
        
        while (_root.position != _endPosition.position)
        {
            _root.position = Vector3.MoveTowards(_root.position, _endPosition.position, duration * Time.deltaTime);
            ctn.ThrowIfCancellationRequested();
            await Task.Yield();
        }
    }
    
    private void SkipAnimation()
    {
        _source.Cancel();
    }
}
