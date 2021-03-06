using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static IEnumerator YieldUntilCompleted<T>(this Task<T> task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            ExceptionDispatchInfo.Capture(task.Exception).Throw();
        }

        yield return null;
    }
}