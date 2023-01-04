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
                if (!withMerged && IsLower(with, i))
                    continue;

                if (!withMerged && IsHigher(with, i))
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

        bool IsEmpty()
        {
            return !_ranges.Any();
        }

        bool IsLowestNonOverlapping(SingleRange singleRange)
        {
            return singleRange.To < _ranges[0].From;
        }

        bool IsHighestNonOverlapping(SingleRange with1)
        {
            return with1.From > _ranges[^1].To;
        }

        bool IsLower(SingleRange singleRange1, int i)
        {
            return _ranges[i].From > singleRange1.To;
        }

        bool IsHigher(SingleRange with2, int i)
        {
            return _ranges[i].To < with2.From;
        }

        SingleRange MergeOverlapping(SingleRange a, SingleRange b)
        {
            return new SingleRange(Math.Min(a.From, b.From), Math.Max(a.To, b.To));
        }
    }

    public Ranges Intersect(SingleRange singleRange)
    {
        return this;
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

    public class IntersectOneToOne
    {
        [Test]
        public void IntersectEmptyRangesToAny()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void IntersectNonOverlapping()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(15, 20));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,6,7,8,9,10 });
        }
        
        [Test]
        public void IntersectInner()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(6,7));
            ranges.Iterate().Should().BeEquivalentTo(new[] { 5,8,9,10 });
        }
        
        [Test]
        public void IntersectOuter()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(2,17));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void IntersectEqual()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(5,10));
            ranges.Iterate().Should().BeEquivalentTo(Array.Empty<int>());
        }
        
        [Test]
        public void IntersectLeft()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(3,7));
            ranges.Iterate().Should().BeEquivalentTo(new [] {8,9,10});
        }
        
        [Test]
        public void IntersectRight()
        {
            var ranges = new Ranges((5, 10));
            ranges.Intersect(new SingleRange(7, 11));
            ranges.Iterate().Should().BeEquivalentTo(new [] {5,6});
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