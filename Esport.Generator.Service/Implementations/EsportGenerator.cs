using System.Text.RegularExpressions;

namespace Esport.GeneratorService.Implementations;

using Core.Interfaces;
using Esport.GeneratorService.Core.Models;
using Models;

public class EsportGenerator : IEsportGenerator
{
    private readonly Random _random = new();

    private readonly List<string> _esports = ["Dota 2", "Counter Strike 2", "League of Legends", "PUBG", "Fortnite"];
    private readonly List<string> _participantNames = ["Team Spirit", "NaVi", "LGD", "EG Sport", "OG", "EHOME"];
    private readonly List<string> _marketName = ["Match Outcome", "First Blood", "The highest number of frags"];

    private readonly Dictionary<string, List<string>> _leagues = new()
    {
        { "Dota 2", ["Blast Slam", "The International", "ESL Dota 2"] },
        { "Counter Strike 2", ["Blast Premier", "ESL CS 2"] },
        { "League of Legends", ["LoL Pro League", "Master Series", "European Championship Series"] },
        { "PUBG", ["Americas Series", "EMEA Championship", "PUBG Nations Cup", "PUBG Champions League"] },
        { "Fortnite", ["Fortnite World Cup"] }
    };

    private readonly Dictionary<string, List<string>> _championships = new()
    {
        { "Blast Slam", ["Blast Slam 1 2024", "Blast Slam 2 2024", "Blast Slam 3 2024"] },
        { "Blast Premier", ["Blast Premier 1 2024", "Blast Premier 2 2024", "Blast Premier 3 2024"] },
        { "The International",
            [
                "The International 2019", "The International 2021", "The International 2022", "The International 2023",
                "The International 2024"
            ]
        },
        { "ESL Dota 2", ["ESL One Birmingham 2024", "ESL One Bangkok 2024", "ESL One Kuala Lumpur 2023"] },
        { "ESL CS 2", ["ESL Pro League Season 20", "ESL Pro League Season 19"] },
        { "LoL Pro League",
            ["Pro League 2024 Spring", "Pro League 2024 Summer", "Pro League 2023 Spring", "Pro League 2023 Summer"]
        },
        { "Master Series", ["Pro League 2024 Spring", "Pro League 2024 Summer"] },
        { "European Championship Series", ["Pro League 2024 Spring", "Pro League 2024 Summer"] },
        { "Americas Series", ["PUBG Nations Cup 2022", "PUBG Nations Cup 2023", "PUBG Nations Cup 2024"] },
        { "EMEA Championship", ["PUBG Nations Cup 2022", "PUBG Nations Cup 2023", "PUBG Nations Cup 2024"] },
        { "PUBG Nations Cup", ["PUBG Nations Cup 2022", "PUBG Nations Cup 2023", "PUBG Nations Cup 2024"] },
        { "PUBG Champions League", ["PUBG Champions League 2022", "PUBG Champions League 2023"] },
        { "Fortnite World Cup", ["Fortnite World Cup 2022", "Fortnite World Cup 2023", "Fortnite World Cup 2024"] }
    };

    public EsportGeneratorModel GenerateEsportData()
    {
        var championshipData = GenerateChampionshipDataAsync();
        
        var teamOne = _participantNames[_random.Next(_participantNames.Count)];
        var teamTwo = _participantNames
            .Where(name => name != teamOne)
            .OrderBy(_ => _random.Next())
            .First();
        
        var marketName = _marketName[_random.Next(_marketName.Count)];
        
        var result = new EsportGeneratorModel()
        {
            Esport = championshipData.Esport,
            League = championshipData.League,
            Championship = championshipData.Championship,
            Event = new EsportEvent() 
            {
                Id = _random.Next(),
                Name = teamOne + " vs " + teamTwo,
                CurrentScore = teamOne + ":" + _random.NextInt64(0, 11) + " - " + teamTwo + ":" + _random.NextInt64(0, 11),
                Participants =
                [
                    new EsportParticipant
                    {
                        Id = _random.Next(),
                        Name = teamOne,
                    },

                    new EsportParticipant
                    {
                        Id = _random.Next(),
                        Name = teamTwo,
                    }
                ],
                Market = new EsportMarket()
                {
                    Id = _random.Next(),
                    Name = marketName,
                    Selections =
                    [
                        new EsportSelection
                        {
                            Id = _random.Next(),
                            Name = "Win " + teamOne,
                            Odds = _random.NextDouble()
                        },

                        new EsportSelection
                        {
                            Id = _random.Next(),
                            Name = "Win " + teamTwo,
                            Odds = _random.NextDouble()
                        }
                    ]
                }
            }
        };
        
        return result;
    }
    
    private EsportData GenerateChampionshipDataAsync()
    {
        var selectedEsport = _esports[_random.Next(_esports.Count)];

        var availableLeagues = _leagues[selectedEsport];
        var selectedLeague = availableLeagues[_random.Next(availableLeagues.Count)];

        var availableChampionships = _championships[selectedLeague];
        var selectedChampionship = availableChampionships[_random.Next(availableChampionships.Count)];

        var championshipData = new EsportData
        {
            Esport = selectedEsport,
            League = selectedLeague,
            Championship = selectedChampionship
        };
        
        return championshipData;
    }

    public EsportGeneratorModel UpdateEsportData(EsportGeneratorModel esportGeneratorModel)
    {
        var newScore = GetNewScore(esportGeneratorModel.Event.CurrentScore);
        esportGeneratorModel.Event.CurrentScore = newScore;

        return esportGeneratorModel;
    }

    private string GetNewScore(string currentScore)
    {
        var match = Regex.Match(currentScore, @"(.+?):(\d+) - (.+?):(\d+)");

        if (!match.Success)
            throw new ArgumentException("Неверный формат строки.");

        var teamOne = match.Groups[1].Value;
        var teamOneScore = int.Parse(match.Groups[2].Value);
        var teamTwo = match.Groups[3].Value;
        var teamTwoScore = int.Parse(match.Groups[4].Value);

        if (_random.Next(2) == 0)
            teamOneScore++;
        else
            teamTwoScore++;

        return $"{teamOne}:{teamOneScore} - {teamTwo}:{teamTwoScore}";
    }
}