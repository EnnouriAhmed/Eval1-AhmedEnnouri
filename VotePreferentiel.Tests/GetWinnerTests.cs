namespace VotePreferentiel.Tests;

public class GetWinnerTests
{
    private readonly VoteCounter _voteCounter = new();

    private static VoteModel CreateModel(params (int Count, string[] Choices)[] ballots)
    {
        return new VoteModel
        {
            Votes = ballots
                .SelectMany(ballot => Enumerable.Range(0, ballot.Count)
                    .Select(_ => new Vote { Choices = [.. ballot.Choices] }))
                .ToList()
        };
    }

    [Test]
    public async Task CasNominal_ChloeGagne()
    {
        var voteModel = CreateModel(
            (1, ["Alice", "Bilal", "Chloé"]),
            (1, ["Alice", "Chloé", "Bilal"]),
            (1, ["Bilal", "Chloé", "Alice"]),
            (1, ["Chloé", "Bilal", "Alice"]),
            (1, ["Chloé", "Alice", "Bilal"]));

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task UnSeulCandidat_AliceGagne()
    {
        var voteModel = CreateModel((1, ["Alice"]));

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task ReportDesPreferences_ChloeGagne()
    {
        var voteModel = CreateModel(
            (4, ["Alice", "Bilal", "Chloé", "Diane"]),
            (3, ["Bilal", "Chloé", "Alice", "Diane"]),
            (3, ["Chloé", "Alice", "Bilal", "Diane"]),
            (1, ["Diane", "Chloé", "Bilal", "Alice"]));

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task EgaliteEnDernierePosition_ChloeGagne()
    {
        var voteModel = CreateModel(
            (1, ["Alice", "Chloé", "Bilal"]),
            (1, ["Bilal", "Chloé", "Alice"]),
            (1, ["Chloé", "Alice", "Bilal"]),
            (1, ["Chloé", "Bilal", "Alice"]));

        var winner = _voteCounter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task BulletinVide_LeveUneException()
    {
        var voteModel = CreateModel((1, []));

        await Assert.That(() => _voteCounter.GetWinner(voteModel)).Throws<ArgumentException>();
    }
}
