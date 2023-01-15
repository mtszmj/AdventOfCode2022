using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day16
{
    public int Part1(string input)
    {
        var valves = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        var distances = new Dictionary<Valve, Dictionary<Valve, int>>();
        foreach (var v in valves.Values)
        {
            distances[v] = new Dictionary<Valve, int>();
            foreach (var o in v.Others)
                distances[v][valves[o]] = 1; // set neighbour length

            distances[v][v] = 0; // distance to self
        }

        PrintDistances(distances);

        var max = valves.Count;
        foreach(var k in valves.Values)
        {
            foreach (var i in valves.Values)
            {
                foreach(var j in valves.Values)
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
            PrintDistances(distances);
        }

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
        var best = start;
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
                log.Add(node);
                var newElement = (node, remainingTime, score, visited, log);
                queue.Enqueue(newElement, -score);
                if (best.score < score)
                {
                    best = newElement;
                }
            }
        }

        Console.WriteLine(string.Join(" -> ", best.log));
        return best.score;
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

        // Console.WriteLine(sb.ToString());
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
}