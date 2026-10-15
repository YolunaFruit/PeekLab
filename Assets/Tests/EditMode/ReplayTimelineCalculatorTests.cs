using System.Collections.Generic;
using NUnit.Framework;

public class ReplayTimelineCalculatorTests
{
    [Test]
    public void EmptyTimelineReturnsNoSegment()
    {
        bool found = ReplayTimelineCalculator.TryCalculate(
            new List<float>(),
            0f,
            out ReplaySegment _
        );

        Assert.That(found, Is.False);
    }

    [Test]
    public void SingleFrameFinishesAtThatFrame()
    {
        bool found = ReplayTimelineCalculator.TryCalculate(
            new[] { 0.2f },
            0f,
            out ReplaySegment segment
        );

        Assert.That(found, Is.True);
        Assert.That(segment.IsFinished, Is.True);
        Assert.That(segment.FromIndex, Is.EqualTo(0));
        Assert.That(segment.ToIndex, Is.EqualTo(0));
    }

    [Test]
    public void TimeBeforeFirstFrameClampsToStart()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0.1f, 0.3f },
            -0.5f,
            out ReplaySegment segment
        );

        Assert.That(segment.IsFinished, Is.False);
        Assert.That(segment.FromIndex, Is.EqualTo(0));
        Assert.That(segment.ToIndex, Is.EqualTo(1));
        Assert.That(segment.InterpolationAmount, Is.EqualTo(0f));
    }

    [Test]
    public void TimeBetweenFramesReturnsInterpolationAmount()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0f, 0.4f },
            0.1f,
            out ReplaySegment segment
        );

        Assert.That(segment.FromIndex, Is.EqualTo(0));
        Assert.That(segment.ToIndex, Is.EqualTo(1));
        Assert.That(segment.InterpolationAmount, Is.EqualTo(0.25f));
    }

    [Test]
    public void IrregularTimelineSelectsContainingSegment()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0f, 0.02f, 0.10f, 0.35f },
            0.20f,
            out ReplaySegment segment
        );

        Assert.That(segment.FromIndex, Is.EqualTo(2));
        Assert.That(segment.ToIndex, Is.EqualTo(3));
        Assert.That(
            segment.InterpolationAmount,
            Is.EqualTo(0.4f).Within(0.0001f)
        );
    }

    [Test]
    public void ExactFrameTimeStartsFollowingSegment()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0f, 0.1f, 0.3f },
            0.1f,
            out ReplaySegment segment
        );

        Assert.That(segment.FromIndex, Is.EqualTo(1));
        Assert.That(segment.ToIndex, Is.EqualTo(2));
        Assert.That(segment.InterpolationAmount, Is.EqualTo(0f));
    }

    [Test]
    public void DuplicateTimesSkipZeroDurationSegment()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0f, 0.1f, 0.1f, 0.3f },
            0.1f,
            out ReplaySegment segment
        );

        Assert.That(segment.FromIndex, Is.EqualTo(2));
        Assert.That(segment.ToIndex, Is.EqualTo(3));
        Assert.That(segment.InterpolationAmount, Is.EqualTo(0f));
    }

    [Test]
    public void TimeAtEndFinishesOnLastFrame()
    {
        ReplayTimelineCalculator.TryCalculate(
            new[] { 0f, 0.2f, 0.5f },
            0.5f,
            out ReplaySegment segment
        );

        Assert.That(segment.IsFinished, Is.True);
        Assert.That(segment.FromIndex, Is.EqualTo(2));
        Assert.That(segment.ToIndex, Is.EqualTo(2));
    }
}
