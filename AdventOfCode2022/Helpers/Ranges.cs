namespace AdventOfCode2022.Helpers;

public class Ranges
{
    private readonly List<SingleRange> _ranges = new();

    public Ranges(params (int from, int to)[] args)
    {
        if (args.Any())
        {
            _ranges.Add(new SingleRange(args[0].from, args[0].to));
            foreach (var a in args.Skip(1))
                Union(new SingleRange(a.from, a.to));
        }
    }

    public IEnumerable<int> Iterate()
    {
        foreach (var r in _ranges)
            for (var i = r.From; i <= r.To; i++)
                yield return i;
    }

    public Ranges Union(Ranges with)
    {
        foreach (var single in with._ranges)
        {
            Union(single);
        }

        return this;
    }
    
    public Ranges Union(SingleRange with)
    {
        if (IsEmpty())
        {
            _ranges.Add(with);
        }
        else if (IsLowestNonOverlapping(with))
        {
            _ranges.Insert(0, with);
        }
        else if (IsHighestNonOverlapping(with))
        {
            _ranges.Add(with);
        }
        else
        {
            var withMerged = false;
            for (var i = _ranges.Count - 1; i >= 0; i--)
            {
                if (!withMerged && IsALowerThanB(with, _ranges[i]))
                    continue;

                if (!withMerged && IsAHigherThanB(with, _ranges[i]))
                {
                    _ranges.Insert(i+1, with);
                    break;
                }
                
                if (!withMerged)
                {
                    _ranges[i] = MergeOverlapping(_ranges[i], with);
                    withMerged = true;
                    continue;
                }

                if (_ranges[i].To < _ranges[i + 1].From)
                    break;

                _ranges[i] = MergeOverlapping(_ranges[i], _ranges[i+1]);
                _ranges.RemoveAt(i + 1);
            }
        }

        return this;
        
        SingleRange MergeOverlapping(SingleRange a, SingleRange b)
        {
            return new SingleRange(Math.Min(a.From, b.From), Math.Max(a.To, b.To));
        }
    }
    
    public Ranges Except(Ranges with)
    {
        foreach (var single in with._ranges)
        {
            Except(single);
        }

        return this;
    }

    public Ranges Except(SingleRange with)
    {
        if (IsEmpty() || IsHighestNonOverlapping(with) || IsLowestNonOverlapping(with))
            return this;
        
        for (var i = _ranges.Count - 1; i >= 0; i--)
        {
            var current = _ranges[i];
            if (IsALowerThanB(with, current))
                continue;
            if (IsAHigherThanB(with, current))
                break;
            if (DoesAContainB(with, current))
            {
                _ranges.RemoveAt(i);
                continue;
            }
            if (DoesAContainB(current, with))
            {
                if (with.To < current.To && with.From > current.From)
                {
                    _ranges.Insert(i+1, new SingleRange(with.To + 1, current.To));
                    _ranges[i] = new SingleRange(current.From, with.From - 1);
                }
                else if (with.To == current.To)
                {
                    _ranges[i] = new SingleRange(current.From, with.From - 1);
                }
                else
                {
                    _ranges[i] = new SingleRange(with.To + 1, current.To);
                } 
            }
            else if (with.From > current.From)
            {
                _ranges[i] = new SingleRange(current.From, with.From - 1);
            }
            else if (with.To < current.To)
            {
                _ranges[i] = new SingleRange(current.To, with.To + 1);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        return this;
        
        
        SingleRange MergeOverlapping(SingleRange a, SingleRange b)
        {
            return new SingleRange(Math.Min(a.From, b.From), Math.Max(a.To, b.To));
        }
    }

    private bool IsEmpty()
    {
        return !_ranges.Any();
    }

    private bool IsLowestNonOverlapping(SingleRange check)
    {
        return check.To < _ranges[0].From;
    }

    private bool IsHighestNonOverlapping(SingleRange check)
    {
        return check.From > _ranges[^1].To;
    }

    private bool IsALowerThanB(SingleRange a, SingleRange b)
    {
        return a.To < b.From;
    }

    private bool IsAHigherThanB(SingleRange a, SingleRange b)
    {
        return a.From > b.To;
    }

    private bool DoesAContainB(SingleRange a, SingleRange b)
    {
        return a.From <= b.From && a.To >= b.To;
    }
}

public record SingleRange
{
    public SingleRange(int from, int to)
    {
        if (from <= to)
        {
            From = from;
            To = to;
        }
        else
        {
            From = to;
            To = from;
        }
    }

    public int From { get; }
    public int To { get; }
}

[TestFixture]
public class RangesTests
{
    public class UnionOneToOne
    {
        [Test]
        public void UnionTestEmpty()
        {
            var ranges = new Ranges();
            ranges.Union(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 15, 16, 17, 18, 19, 20 });
        }

        [Test]
        public void UnionTestBefore()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(0, 1));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 0, 1, 5, 6, 7, 8, 9, 10 });
        }

        [Test]
        public void UnionTestAfter()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(13, 14));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10, 13, 14 });
        }

        [Test]
        public void UnionTestInner()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(6, 7));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10 });
        }

        [Test]
        public void UnionTestInner2()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(4, 11));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 4, 5, 6, 7, 8, 9, 10, 11 });
        }

        [Test]
        public void UnionTestMerge()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(6, 15));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
        }

        [Test]
        public void UnionTestMerge2()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(10, 15));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
        }

        [Test]
        public void UnionTestOuter()
        {
            var ranges = new Ranges((5, 10));
            ranges.Union(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10, 15, 16, 17, 18, 19, 20 });
        }
    }

    public class UnionManyToOne
    {
        
        [Test]
        public void UnionMiddle()
        {
            var ranges = new Ranges((5, 10), (15, 16));
            ranges.Union(new SingleRange(12, 13));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10, 12, 13, 15, 16 });
        }
        
        [Test]
        public void UnionOuter()
        {
            var ranges = new Ranges((5, 7), (9, 10));
            ranges.Union(new SingleRange(3, 13));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13});
        }
        
        [Test]
        public void UnionSingleMergeFirstLeft()
        {
            var ranges = new Ranges((5, 7), (9, 10));
            ranges.Union(new SingleRange(3, 6));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 3, 4, 5, 6, 7, 9, 10});
        }
        
        [Test]
        public void UnionSingleMergeFirstRight()
        {
            var ranges = new Ranges((5, 7), (10, 11));
            ranges.Union(new SingleRange(6, 8));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 10, 11});
        }
        
        [Test]
        public void UnionSingleMergeSecondLeft()
        {
            var ranges = new Ranges((5, 6), (9, 11));
            ranges.Union(new SingleRange(8, 10));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 8, 9, 10, 11});
        }
        
        [Test]
        public void UnionSingleMergeSecondRight()
        {
            var ranges = new Ranges((5, 7), (10, 11));
            ranges.Union(new SingleRange(11, 13));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 10, 11, 12, 13});
        }
        
        [Test]
        public void UnionMultipleMerge()
        {
            var ranges = new Ranges((5, 7), (10, 11), (13, 14), (16, 17), (20, 22));
            ranges.Union(new SingleRange(9, 16));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 20, 21, 22});
        }
    }
    
    public class UnionRangeToRange
    {
        [Test]
        public void UnionTest1()
        {
            var r1 = new Ranges((5, 10), (15, 20));
            var r2 = new Ranges((12, 13), (22, 23));
            r1.Union(r2).Iterate().Should().BeEquivalentTo(new [] {5, 6, 7, 8, 9, 10, 12, 13, 15, 16, 17, 18, 19, 20, 22, 23});
        }
    }
    
    public class UnionInConstructor
    {
        [Test]
        public void UnionConstr()
        {
            var ranges = new Ranges((10, 5), (-2, 1), (6, 7), (15, 17), (10, 11), (14, 18), (21, 23), (22, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[]
                { -2, -1, 0, 1, 5, 6, 7, 8, 9, 10, 11, 14, 15, 16, 17, 18, 20, 21, 22, 23 });
        }
    }

    public class ExceptOneToOne
    {
        [Test]
        public void ExceptEmptyRangesToAny()
        {
            var ranges = new Ranges();
            ranges.Except(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void ExceptNonOverlapping()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,9,10 });
        }
        
        [Test]
        public void ExceptInner()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(6,7));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,8,9,10 });
        }
        
        [Test]
        public void ExceptOuter()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(2,17));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void ExceptEqual()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(5,10));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void ExceptLeft()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(3,7));
            ranges.Iterate().Should().BeEquivalentTo(new [] {8,9,10});
        }
        
        [Test]
        public void ExceptRight()
        {
            var ranges = new Ranges((5, 10));
            ranges.Except(new SingleRange(7, 11));
            ranges.Iterate().Should().BeEquivalentTo(new [] {5,6});
        }
    }

    public class ExceptManyToOne
    {
        [Test]
        public void ExceptNonOverlappingLeft()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(0, 3));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,15,16,17,18 });
        }
        
        [Test]
        public void ExceptNonOverlappingRight()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(20, 23));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,15,16,17,18 });
        }
        
        [Test]
        public void ExceptNonOverlappingMiddle()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(10, 13));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,15,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingFirstLeft()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(3, 6));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 7,8,15,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingFirstRight()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(7, 10));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,15,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingFirstInside()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(6, 7));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,8,15,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingFirstOutside()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(3, 9));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 15,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingSecondLeft()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(13, 16));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingSecondRight()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(17, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,15,16 });
        }
        
        [Test]
        public void ExceptOverlappingSecondInside()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(16, 17));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,15,18 });
        }
        
        [Test]
        public void ExceptOverlappingSecondOutside()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(13, 19));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8 });
        }
        
        [Test]
        public void ExceptOverlappingBoth()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(8, 15));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,16,17,18 });
        }
        
        [Test]
        public void ExceptOverlappingBothTotally()
        {
            var ranges = new Ranges((5, 8), (15, 18));
            ranges.Except(new SingleRange(0, 25));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
    }
    
    public class ExceptRangeToRange
    {
        
        [Test]
        public void ExceptTest1()
        {
            var r1 = new Ranges((5, 10), (15, 20), (25, 30));
            var r2 = new Ranges((5,7), (10, 16), (22, 23), (25,25), (28,29), (31,33));
            r1.Except(r2).Iterate().Should().BeEquivalentTo(new [] {8, 9, 17, 18, 19, 20, 26, 27, 30});
        }
    }

}

[TestFixture]
public class SingleRangeTests
{
    [TestCase(7, 9)]
    [TestCase(9, 7)]
    [TestCase(7, 7)]
    [TestCase(-1, 7)]
    [TestCase(7, -1)]
    public void RangeIsFromLowerToHigher(int a, int b)
    {
        new SingleRange(a, b).Should().Be(new SingleRange(b, a));
    }
}