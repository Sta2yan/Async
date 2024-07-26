using System.Threading;
using UnityEngine;

public class NewThread : MonoBehaviour
{
    private Thread _thread;
    private int _randomNumber;
    
    public void Execute()
    {
        _thread = new Thread(() => _randomNumber = SquareNumber(5));
        _thread.Start();
        
        _thread.Join();
        
        print(_randomNumber);
    }

    private int SquareNumber(int number)
    {
        return number * number;
    }
}