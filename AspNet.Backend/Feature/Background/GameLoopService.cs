namespace Gen.Backend.Feature.Background;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The <see cref="GameLoopService"/> class
/// is a <see cref="BackgroundService"/> running at a given <see cref="TickRateMs"/> and acts as the game-server. 
/// </summary>
/// <param name="logger"></param>
public class GameLoopService(ILogger<GameLoopService> logger) : BackgroundService
{
    /// <summary>
    /// The tickrate at which the game-server is running at.
    /// </summary>
    private const int TickRateMs = 1000 / 60; // 60Hz = 16.67ms per tick

    /// <summary>
    /// The gameloop itself, running in the background. 
    /// </summary>
    /// <param name="stoppingToken">The <see cref="CancellationToken"/> to stop the gameloop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Server starting...");
        
        var stopwatch = new Stopwatch();
        while (!stoppingToken.IsCancellationRequested)
        {
            stopwatch.Restart();
            
            try
            {
                UpdateGameLogic();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var delay = Math.Max(0, TickRateMs - elapsedMs);

            await Task.Delay((int)delay, stoppingToken);
        }

        logger.LogInformation("Server stopped...");
    }

    /// <summary>
    /// Runs in the gameloop to update the server gamestate.
    /// </summary>
    private void UpdateGameLogic()
    {
        //logger.LogInformation("Tick: {Time}", DateTimeOffset.Now);
    }
}
