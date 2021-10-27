using System;

using ManagedShell.WindowsTasks;

namespace Explorip.TaskBar.Utilities
{
    public class TaskCategoryProvider : ITaskCategoryProvider
    {
        public void Dispose()
        {
            
        }

        public string GetCategory(ApplicationWindow window)
        {
            return "";
        }

        public void SetCategoryChangeDelegate(TaskCategoryChangeDelegate changeDelegate)
        {
            Console.WriteLine("SetCategoryChangeDelegate");
        }
    }
}
