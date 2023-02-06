using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day16
{
    public int Part2(string input)
    {
        var valves = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        var toVisit = valves.Where(x => x.Value.FlowRate > 0).Select(x => x.Key).ToHashSet();
        var toVisitArray = toVisit.ToArray();
        
        
        var distances = CalculateDistances(valves);
        var startPosition = "AA";
        var startTime = 26;

            var firstSets = Enumerable.Range(0, 1 << (toVisitArray.Length))
                .Select(index => toVisitArray.Where((str, i) => (index & (1 << i)) != 0)
                    .ToArray());

            var best = new (string pos, int time, int score, HashSet<string> visited, List<string> log)[2]; 
            foreach (var fs in firstSets)
            {
                if(fs.Length > toVisitArray.Length/2)
                    continue;

                var a = Move(fs);
                var b = Move(toVisit.Except(fs).ToArray());

                if (best[0].score + best[1].score < a.score + b.score)
                {
                    best[0] = a;
                    best[1] = b;
                }
            }
            
            Console.WriteLine(string.Join(" -> ", best[0].log));
            Console.WriteLine(string.Join(" -> ", best[1].log));

            return best[0].score + best[1].score;
            
            (string pos, int time, int score, HashSet<string> visited, List<string> log) Move(string[] toVisit)
            {
                var queue =
                    new PriorityQueue<(string pos, int time, int score, HashSet<string> visited, List<string> log), int>();
        
                var start = (
                    pos: startPosition, 
                    time: startTime, 
                    score: 0, 
                    visited: new HashSet<string>(),
                    log: new List<string>() {startPosition}
                );
                
                queue.Enqueue(start, 0);
                (string pos, int time, int score, HashSet<string> visited, List<string> log) best = default;
                while (queue.TryDequeue(out var current, out var priority))
                {
                    foreach (var node in toVisit.Except(current.visited))
                    {
                        var dist = distances[valves[current.pos]][valves[node]];
                        var remainingTime = current.time - dist - 1;
                        if (remainingTime <= 0)
                            continue;
                        var score = current.score + remainingTime * valves[node].FlowRate;
                        var visited = current.visited.ToHashSet();
                        visited.Add(node);
                        var log = current.log.ToList();
                        log.Add($"{startTime - remainingTime + 1}:{node}");
                        var newElement = (node, remainingTime, score, visited, log);
                        queue.Enqueue(newElement, -score);
                        if (best.score < score)
                        {
                            best = newElement;
                        }
                    }
                }

                return best;
            }
    }

    public int Part2_(string input, HashSet<string> toVisit)
    {
        var valves = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        toVisit = valves.Where(x => x.Value.FlowRate > 0).Select(x => x.Key).ToHashSet();
        
        var distances = CalculateDistances(valves);
        var startPosition = "AA";
        var startTime = 26;
        
        var start = (
            pos1: startPosition, 
            pos2: startPosition, 
            time1: startTime, 
            time2: startTime, 
            score: 0, 
            visited: new HashSet<string>(),
            log1: new List<string>() {startPosition},
            log2: new List<string>() {startPosition}
        );

        var queue =
            new PriorityQueue<(string pos1, string pos2, int time1, int time2, int score, HashSet<string> visited, List<string> log1, List<string> log2), int>();
        
        queue.Enqueue(start, 0);
        var best = start;

        while (queue.TryDequeue(out var current, out var priority))
        {
            foreach (var node in toVisit.Except(current.visited))
            {
                {
                    MoveFirst(current, node);
                    MoveSecond(current, node);
                }

            }
        }
        
        Console.WriteLine(string.Join(" -> ", best.log1));
        Console.WriteLine(string.Join(" -> ", best.log2));
        return best.score;

        void MoveFirst((string pos1, string pos2, int time1, int time2, int score, HashSet<string> visited, List<string> log1, List<string> log2) current, string node)
        {
            // first moves
            var dist1 = distances[valves[current.pos1]][valves[node]];
            var remainingTime1 = current.time1 - dist1 - 1;
            if (remainingTime1 > 0)
            {
                var score1 = current.score + remainingTime1 * valves[node].FlowRate;
                var visited1 = current.visited.ToHashSet();
                visited1.Add(node);
                var log1 = current.log1.ToList();
                var log2 = current.log2.ToList();
                log1.Add($"{startTime - remainingTime1 + 1}:{node}");
                var newElement = (node, current.pos2, remainingTime1, current.time2, score1, visited1, log1, log2);
                queue.Enqueue(newElement, -score1);
                if (best.score < score1)
                    best = newElement;
            }
        }

        void MoveSecond(
            (string pos1, string pos2, int time1, int time2, int score, HashSet<string> visited, List<string> log1, List<string> log2) current,
            string node)
        {
            // second moves
            var dist2 = distances[valves[current.pos2]][valves[node]];
            var remainingTime2 = current.time2 - dist2 - 1;
            if (remainingTime2 > 0)
            {
                var score2 = current.score + remainingTime2 * valves[node].FlowRate;
                var visited2 = current.visited.ToHashSet();
                visited2.Add(node);
                var log1 = current.log1.ToList();
                var log2 = current.log2.ToList();
                log2.Add($"{startTime - remainingTime2 + 1}:{node}");
                var newElement = (current.pos1, node, current.time1, remainingTime2, score2, visited2, log1, log2);
                queue.Enqueue(newElement, -score2);
                if (best.score < score2)
                    best = newElement;
            }
        }
    }
    
    public int Part1(string input)
    {
        var valves = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        var distances = CalculateDistances(valves);

        var valvesWithFlow = valves.Where(x => x.Value.FlowRate > 0).Select(x => x.Key).ToHashSet();
        var startPosition = "AA";
        var startTime = 30;

        var start = (
            pos: startPosition, 
            time: startTime, 
            score: 0, 
            visited: new HashSet<string>(),
            log: new List<string>() {startPosition}
        );

        var queue =
            new PriorityQueue<(string pos, int time, int score, HashSet<string> visited, List<string> log), int>();
        
        queue.Enqueue(start, 0);
        var best = new List<(string pos, int time, int score, HashSet<string> visited, List<string> log)> { start };
        while (queue.TryDequeue(out var current, out var priority))
        {
            foreach (var node in valvesWithFlow.Except(current.visited))
            {
                var dist = distances[valves[current.pos]][valves[node]];
                var remainingTime = current.time - dist - 1;
                if (remainingTime <= 0)
                    continue;
                var score = current.score + remainingTime * valves[node].FlowRate;
                var visited = current.visited.ToHashSet();
                visited.Add(node);
                var log = current.log.ToList();
                log.Add($"{startTime - remainingTime + 1}:{node}");
                var newElement = (node, remainingTime, score, visited, log);
                queue.Enqueue(newElement, -score);
                if (best.First().score < score)
                {
                    best = new List<(string pos, int time, int score, HashSet<string> visited, List<string> log)>
                        { newElement };
                }
                else if (best.First().score < score)
                {
                    best.Add(newElement);
                }
            }
        }

        foreach (var b in best)
        {
            Console.WriteLine(string.Join(" -> ", b.log));
        }
        return best.First().score;
    }

    // https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm#Pseudocode_[11]
    private Dictionary<Valve, Dictionary<Valve, int>> CalculateDistances(Dictionary<string, Valve> valves)
    {
        var distances = new Dictionary<Valve, Dictionary<Valve, int>>();
        foreach (var v in valves.Values)
        {
            distances[v] = new Dictionary<Valve, int>();
            foreach (var o in v.Others)
                distances[v][valves[o]] = 1; // set neighbour length

            distances[v][v] = 0; // distance to self
        }

        foreach (var k in valves.Values)
        {
            foreach (var i in valves.Values)
            {
                foreach (var j in valves.Values)
                {
                    if (distances[i].ContainsKey(k)
                        && distances[k].ContainsKey(j)
                        && ((distances[i].ContainsKey(j) && distances[i][j] > distances[i][k] + distances[k][j])
                            || !distances[i].ContainsKey(j))
                       )
                    {
                        distances[i][j] = distances[i][k] + distances[k][j];
                    }
                }
            }
        }

        PrintDistances(distances);
        return distances;
    }

    public void PrintDistances(Dictionary<Valve, Dictionary<Valve, int>> dist)
    {
        var table = new string[dist.Keys.Count+1, dist.Keys.Count+1];
        var keysOrdered = dist.Keys.OrderBy(x => x.Name).ToArray();
        int i = 1, j = 1;

        foreach (var key in keysOrdered)
        {
            table[0,i] = key.Name;
            table[i,0] = key.Name;
            foreach (var key2 in keysOrdered)
            {
                table[i, j] = dist[key].ContainsKey(key2) ? $"{dist[key][key2]}" : $"∞";
                j++;
            }

            j = 1;
            i++;
        }

        var sb = new StringBuilder();
        for (var index = 0; index < table.GetLength(0); index++)
        {
            for (var index2 = 0; index2 < table.GetLength(0); index2++)
            {
                sb.Append(table[index, index2]);
                sb.Append("\t");
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
    
    public int Part1_(string input)
    {
        var valves = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        var startPosition = "AA";
        var startTime = 30;

        var start = (
            current: valves[startPosition],
            state: valves.ToDictionary(x => x.Key, x => x.Value),
            score: 0,
            time: startTime,
            maxPossibleScore: int.MinValue,
            log: new List<string>() {startPosition}
        );
        var queue = new PriorityQueue<(Valve current, Dictionary<string, Valve> state, int score, int time, int maxPossibleScore, List<string> log), int>();
        queue.Enqueue(start, int.MinValue);

        var highestResult = 0;
        var result = start;
        while (queue.TryDequeue(out var element, out _))
        {
            var (current, state, score, time, maxPossible, log) = element;
            if (time == 0 || state.All(x => x.Value.IsOpen || x.Value.FlowRate == 0))
            {
                if (score > highestResult)
                {
                    highestResult = score;
                    result = element;
                }

                else if (highestResult > -maxPossible)
                    break;
                
                continue;
            }
            
            if (current.IsOpen == false && current.FlowRate > 0)
            {
                var newCurrent = current with { IsOpen = true };
                var newState = state.ToDictionary(x => x.Key, x => x.Value);
                newState[newCurrent.Name] = newCurrent;
                var newScore = score + (time - 1) * current.FlowRate;
                var newTime = time - 1;
                var timeShift = 1;
                var maxPossibleScore = newState.Values
                    .Where(x => x.IsOpen == false && x.FlowRate > 0)
                    .OrderBy(x => x.FlowRate)
                    .Aggregate(-newScore, (acc, x) =>
                    {
                        if (time - timeShift < 0)
                            return acc;
                        var res = acc - x.FlowRate * (time - timeShift);
                        timeShift += 1;
                        return res;
                    })
                    ;
                    
                    // var newLog = log.ToList();
                    // newLog.Add("^");
                queue.Enqueue((newCurrent, newState, newScore, newTime, maxPossibleScore, log), maxPossibleScore);
            }

            var timeShift2 = 1;
            var maxPossibleScore2 = state.Values
                    .Where(x => x.IsOpen == false && x.FlowRate > 0)
                    .OrderBy(x => x.FlowRate)
                    .Aggregate(-score, (acc, x) =>
                    {
                        if (time - timeShift2 < 0)
                            return acc;
                        var res = acc - x.FlowRate * (time - timeShift2);
                        timeShift2 += 1;
                        return res;
                    })
                ;
            foreach (var other in current.Others)
            {
                var newCurrent = state[other];
                var newState = state.ToDictionary(x => x.Key, x => x.Value);
                var newScore = score;
                var newTime = time - 1;
                // var newLog = log.ToList();
                // newLog.Add($"->{other}");
                queue.Enqueue((newCurrent, newState, newScore, newTime, maxPossibleScore2, log), maxPossibleScore2);
            }
        }

        Console.WriteLine(string.Join("", result.log));
        return highestResult;
    }
    
    public Valve Parse(string line)
    {
        var match = Regex.Match(line, @"^Valve ([A-Z]+) has flow rate=(\d+); tunnel[s]? lead[s]? to valve[s]? (.*)$", RegexOptions.Compiled);
        return new Valve(match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value.Split(", ", StringSplitOptions.RemoveEmptyEntries));
    }
}

public record Valve(string Name, int FlowRate, string[] Others, bool IsOpen = false);



[TestFixture]
public class Day16Tests
{
    public const string Example = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";

    [Test]
    public void ParseTest()
    {
        var valve = new Day16().Parse("Valve AA has flow rate=0; tunnels lead to valves DD, II, BB");
        valve.Name.Should().Be("AA");
        valve.FlowRate.Should().Be(0);
        valve.Others.Should().BeEquivalentTo(new[] { "DD", "II", "BB" });
    }

    [TestCase("Valve JJ has flow rate=21; tunnel leads to valve II")]
    [TestCase("Valve AA has flow rate=0; tunnels lead to valves DD, II, BB")]
    public void ParseOk(string input)
    {
        var x = "II".Split(", ");

        var valve = new Day16().Parse(input);
        valve.Name.Should().NotBeEmpty();
        valve.FlowRate.Should().BeGreaterOrEqualTo(0);
        valve.Others.Should().HaveCountGreaterOrEqualTo(1);
    }

    [Test]
    public void Part1Example()
    {
        new Day16().Part1(Example).Should().Be(1651);
    }
    
    [Test]
    public void Part1Input()
    {
        new Day16().Part1(Helper.ReadDay(16)).Should().Be(1647);
    }
    
    [Test]
    public void Part2Example()
    {
        //new HashSet<string> { "AA", "DD", "BB", "JJ", "HH", "EE", "CC" }
        new Day16().Part2(Example).Should().Be(1707);
    }

    [Test]
    public void Part2Input()
    {
        //new HashSet<string> { "AA", "HS", "MV", "LI", "AU", "KR", "NG" }
        new Day16().Part2(
        Helper.ReadDay(16))
            .Should().Be(2169);
    }
}