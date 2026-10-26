namespace VotePreferentiel;

public class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        return voteModel.Votes.SelectMany(vote => vote.Choices).ToHashSet();
    }

    public string GetWinner(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        if (voteModel.Votes.Count == 0)
        {
            throw new ArgumentException("Aucun bulletin n'a été fourni.", nameof(voteModel));
        }

        if (voteModel.Votes.Any(vote => vote.Choices.Count == 0))
        {
            throw new ArgumentException("Un bulletin ne contient aucun choix.", nameof(voteModel));
        }

        var remainingCandidates = GetCandidates(voteModel);

        while (remainingCandidates.Count > 1)
        {
            var firstChoices = GetFirstChoices(voteModel, remainingCandidates);
            var counts = CountFirstChoices(firstChoices);

            foreach (var candidate in remainingCandidates)
            {
                counts.TryAdd(candidate, 0);
            }

            var losers = GetCandidatesWithLeastVotes(counts);

            if (losers.Length == remainingCandidates.Count)
            {
                throw new InvalidOperationException(
                    $"Égalité parfaite entre les derniers candidats : {string.Join(", ", losers)}.");
            }

            remainingCandidates.ExceptWith(losers);
        }

        return remainingCandidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        return voteModel.Votes
            .Where(vote => vote.Choices.Count > 0)
            .Select(vote => vote.Choices[0])
            .ToList();
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        return firstChoices
            .GroupBy(choice => choice)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        if (firstChoiceCounts.Count == 0)
        {
            return [];
        }

        var minimum = firstChoiceCounts.Values.Min();

        return firstChoiceCounts
            .Where(count => count.Value == minimum)
            .Select(count => count.Key)
            .ToArray();
    }

    private static List<string> GetFirstChoices(VoteModel voteModel, HashSet<string> remainingCandidates)
    {
        var firstChoices = new List<string>(voteModel.Votes.Count);

        foreach (var vote in voteModel.Votes)
        {
            var choice = vote.Choices.FirstOrDefault(remainingCandidates.Contains);

            if (choice is not null)
            {
                firstChoices.Add(choice);
            }
        }

        return firstChoices;
    }
}
