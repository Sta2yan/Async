using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NewThread : MonoBehaviour
{
    private Thread _thread;
    private int _randomNumber;
    
    public async void Execute()
    {
        /*#region ДоЛекции

        _thread = new Thread(() => _randomNumber = SquareNumber(5));
        _thread.Start();
        
        _thread.Join();
        
        print(_randomNumber);

        #endregion*/

        #region После лекции

        await Task.Run(() => _randomNumber = SquareNumber(5));
        print(_randomNumber);

        #endregion
    }

    private int SquareNumber(int number)
    {
        return number * number;
    }
}