using Microsoft.Extensions.Logging;
using Quartz;

namespace CaravelTemplate.Adapter.Quartz.Jobs;

[DisallowConcurrentExecution]
public class HelloWorldJob : IJob
{
    private readonly ILogger<HelloWorldJob> _logger;
    
    public HelloWorldJob(ILogger<HelloWorldJob> logger)
    {
        _logger = logger;
    }
    
    public Task Execute(IJobExecutionContext context)
    {
        // Code that sends a periodic email to the user (for example)
        // Note: This method must always return a value 
        // This is especially important for trigger listeners watching job execution 
        
        _logger.LogInformation("Hello World Job started");
        
        return Task.CompletedTask;
    }
}  