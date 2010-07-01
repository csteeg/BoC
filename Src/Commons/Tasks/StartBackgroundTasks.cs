using BoC.Helpers;

namespace BoC.Tasks
{
    public class StartBackgroundTasks : IBootstrapperTask
    {
        private readonly IBackgroundTask[] _tasks;

        public StartBackgroundTasks(IBackgroundTask[] tasks)
        {
            _tasks = tasks;
        }

        public void Execute()
        {
            if (_tasks != null)
            {
                foreach (var task in _tasks)
                {
                    task.Start();
                }
            }
        }
    }
}