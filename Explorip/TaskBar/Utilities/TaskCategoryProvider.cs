using System;
using System.Diagnostics;

using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.Utilities;

public class TaskCategoryProvider : ITaskCategoryProvider
{
    private bool disposedValue;

    public string GetCategory(ApplicationWindow window)
    {
        return "";
    }

    public void SetCategoryChangeDelegate(TaskCategoryChangeDelegate changeDelegate)
    {
        Debug.WriteLine("SetCategoryChangeDelegate");
    }

    public bool IsDisposed
    {
        get { return disposedValue; }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Nothing to do here ?!
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
