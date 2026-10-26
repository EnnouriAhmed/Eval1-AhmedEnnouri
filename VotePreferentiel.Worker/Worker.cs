namespace VotePreferentiel.Worker;

public class Worker(
    ILogger<Worker> logger,
    IConfiguration configuration,
    VoteProvider voteProvider,
    IVoteCounter voteCounter,
    IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var url = configuration["Votes:Url"]
                ?? throw new InvalidOperationException("L'URL des votes (Votes:Url) n'est pas configurée.");

            logger.LogInformation("Téléchargement des votes depuis {Url}", url);
            var voteModel = await voteProvider.GetVotesAsync(url, stoppingToken);

            var candidates = voteCounter.GetCandidates(voteModel);
            logger.LogInformation("{VoteCount} bulletins récupérés pour {CandidateCount} candidats",
                voteModel.Votes.Count, candidates.Count);

            var winner = voteCounter.GetWinner(voteModel);
            Console.WriteLine($"Gagnant : {winner}");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !stoppingToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Le calcul du résultat de l'élection a échoué");
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
}
