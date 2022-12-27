using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace AdventOfCode2022;

public class Day13
{
    public long Part1(string input)
    {
        var index = 1;
        var sum = 0L;
        foreach (var lines in input.Split($"{Environment.NewLine}{Environment.NewLine}",
                     StringSplitOptions.RemoveEmptyEntries))
        {
            var lr = lines.Split(Environment.NewLine);
            sum += Compare(lr[0], lr[1]) ? index : 0L;
            index++;
        }

        return sum;
    }

    public long Part2(string input)
    {
        var list = new List<IElement>();
        var first = Parse("[[2]]");
        var second = Parse("[[6]]");
        list.Add(first);
        list.Add(second);
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            list.Add(Parse(line));

        list.Sort(Compare);

        return (list.IndexOf(first) + 1) * (list.IndexOf(second) + 1);
    }
    
    public bool Compare(string left, string right)
    {
        return Compare(Parse(left), Parse(right)) == -1;
    }

    public int Compare(IElement left, IElement right)
    {
        if (left is Element l && right is Element r)
        {
            return l.Value.CompareTo(r.Value);
        }

        if (left is ElementList lList && right is ElementList rList)
        {
            for (var i = 0; i < lList.List.Count && i < rList.List.Count; i++)
            {
                var comp = Compare(lList.List[i], rList.List[i]);
                if (comp != 0)
                    return comp;
            }

            if (lList.List.Count < rList.List.Count)
                return -1;
            if (lList.List.Count > rList.List.Count)
                return 1;
            return 0;
        }

        if (left is Element el && right is ElementList li)
        {
            var newList = new ElementList();
            newList.Add(el);
            return Compare(newList, li);
        }
        
        if (left is ElementList li2 && right is Element el2)
        {
            var newList = new ElementList();
            newList.Add(el2);
            return Compare(li2, newList);
        }

        throw new InvalidOperationException();
    }

    public IElementList Parse(string line)
    {
        var stack = new Stack<IElementList>();
        int? valueStart = null;
        IElementList current = null;
        IElementList toReturn = null;

        foreach(var element in line.Split(","))
        {
            if (int.TryParse(element, out var value))
                current.Add(value);
            else
                for (var index = 0; index < element.Length; index++)
                {
                    var ch = element[index];
                    switch (ch)
                    {
                        case '[':
                            var newList = new ElementList();
                            toReturn = toReturn ?? newList;
                            if (current is not null)
                            {
                                current.Add(newList);
                                stack.Push(current);
                            }
                            current = newList;
                            break;
                        case ']':
                            if (valueStart.HasValue)
                            {
                                current.Add(int.Parse(element[valueStart.Value..(index)]));
                                valueStart = null;
                            }

                            stack.TryPop(out current);
                            break;
                        default:
                            if(!valueStart.HasValue)
                                valueStart = index;
                            break;
                    }
                }

            if (valueStart.HasValue)
            {
                current.Add(int.Parse(element[valueStart.Value..]));
                valueStart = null;
            }
        }

        return toReturn;
    }
}

public class ElementList : IElementList
{
    private List<IElement> _list = new List<IElement>();
    public IReadOnlyList<IElement> List => _list;
    public void Add(int value)
    {
        _list.Add(new Element(value));
    }

    public void Add(IElement element)
    {
        _list.Add(element);
    }

    public override string ToString()
    {
        return $"[{string.Join(",", _list.Select(x => x.ToString()))}]";
    }
}

public class Element : IElement
{
    public Element(int value)
    {
        Value = value;
    }
    
    public int Value { get; }

    public override string ToString() => $"{Value}";
}

public interface IElementList : IElement
{
    void Add(int value);
    void Add(IElement element);
}

public interface IElement
{
    
}

[TestFixture]
public class Day13Tests
{
    [Test]
    public void ParseTest()
    {
        string input = "[10,1,3,1,12]"; 
        var parsed = new Day13().Parse(input) as ElementList;
        parsed.List.Cast<Element>().Select(x => x.Value).Should().ContainInOrder(10, 1, 3, 1, 12);
    }
    
    [TestCase("[1,1,3,1,1]")]
    [TestCase("[1,1,5,1,1]")]
    [TestCase("[[1],[2,3,4]]")]
    [TestCase("[[1],4]")]
    [TestCase("[9]")]
    [TestCase("[[8,7,6]]")]
    [TestCase("[[4,4],4,4]")]
    [TestCase("[[4,4],4,4,4]")]
    [TestCase("[7,7,7,7]")]
    [TestCase("[7,7,7]")]
    [TestCase("[]")]
    [TestCase("[3]")]
    [TestCase("[[[]]]")]
    [TestCase("[[]]")]
    [TestCase("[1,[2,[3,[4,[5,6,7]]]],8,9]")]
    [TestCase("[1,[2,[3,[4,[5,6,0]]]],8,9]")]
    public void ParseCaseTest(string input)
    {
        var parsed = new Day13().Parse(input) as ElementList;
        parsed.ToString().Should().Be(input);
    }

    [TestCase(3, 4, -1)]
    [TestCase(4, 3, 1)]
    [TestCase(4, 4, 0)]
    public void CompareElementTest(int left, int right, int result)
    {
        new Day13().Compare(new Element(left), new Element(right)).Should().Be(result);
    }
    
    [TestCase(new [] {1,1,3}, new [] {1,1,3}, 0)]
    [TestCase(new [] {1,1,3}, new [] {1,2,3}, -1)]
    [TestCase(new [] {1,2,3}, new [] {1,1,3}, 1)]
    [TestCase(new [] {1,2,3}, new [] {1,2,3,4}, -1)]
    [TestCase(new [] {1,2,3,4,5}, new [] {1,2,3,4}, 1)]
    [TestCase(new [] {1,2,3,4,5}, new [] {1,2,3,4,5}, 0)]
    public void CompareListsTest(int[] left, int[] right, int result)
    {
        var leftList = new ElementList();
        foreach (var el in left.Select(x => new Element(x)))
        {
            leftList.Add(el);
        }
        var rightList = new ElementList();
        foreach (var el in right.Select(x => new Element(x)))
        {
            rightList.Add(el);
        }
        
        new Day13().Compare(leftList, rightList).Should().Be(result);
    }
    
    [TestCase("[1,1,3,1,1]","[1,1,5,1,1]", true)]
    [TestCase("[[1],[2,3,4]]","[[1],4]", true)]
    [TestCase("[9]","[[8,7,6]]", false)]
    [TestCase("[[4,4],4,4]","[[4,4],4,4,4]", true)]
    [TestCase("[7,7,7,7]","[7,7,7]", false)]
    [TestCase("[]","[3]", true)]
    [TestCase("[[[]]]","[[]]", false)]
    [TestCase("[1,[2,[3,[4,[5,6,7]]]],8,9]","[1,[2,[3,[4,[5,6,0]]]],8,9]", false)]
    public void CompareSingleExamplesTest(string left, string right, bool rightOrder)
    {
        new Day13().Compare(left, right).Should().Be(rightOrder);
    }

    [Test]
    public void Part1Example()
    {
        new Day13().Part1(Example).Should().Be(13);
    }

    [Test]
    public void Part1Input()
    {
        new Day13().Part1(Helper.ReadDay(13)).Should().Be(6086);
    }

    [Test]
    public void Part2Example()
    {
        new Day13().Part2(Example).Should().Be(140);
    }

    [Test]
    public void Part2Input()
    {
        new Day13().Part2(Helper.ReadDay(13)).Should().Be(27930);
    }


    private const string Example = @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]";

}