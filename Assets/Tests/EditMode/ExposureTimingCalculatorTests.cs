using NUnit.Framework;

public class ExposureTimingCalculatorTests
{
    [Test]
    public void TryCalculate_UsesFirstShotAfterVisibility()
    {
        ShotSample[] shots =
        {
            CreateShot(time: 1.3f, movementSpeed: 2.5f)
        };

        bool found = ExposureTimingCalculator.TryCalculate(
            hasVisibilityResult: true,
            visibleAtSeconds: 1f,
            shots,
            out ExposureTimingResult result
        );

        Assert.That(found, Is.True);
        Assert.That(
            result.ExposureMilliseconds,
            Is.EqualTo(300f).Within(0.01f)
        );
    }

    [Test]
    public void TryCalculate_IgnoresShotsBeforeVisibility()
    {
        ShotSample[] shots =
        {
            CreateShot(time: 0.8f),
            CreateShot(time: 1.2f)
        };

        bool found = ExposureTimingCalculator.TryCalculate(
            true,
            1f,
            shots,
            out ExposureTimingResult result
        );

        Assert.That(found, Is.True);
        Assert.That(
            result.ExposureMilliseconds,
            Is.EqualTo(200f).Within(0.01f)
        );
    }

    [Test]
    public void TryCalculate_ShotAtVisibilityHasZeroExposure()
    {
        ShotSample[] shots = { CreateShot(time: 1f) };

        bool found = ExposureTimingCalculator.TryCalculate(
            true,
            1f,
            shots,
            out ExposureTimingResult result
        );

        Assert.That(found, Is.True);
        Assert.That(
            result.ExposureMilliseconds,
            Is.EqualTo(0f).Within(0.01f)
        );
    }

    [Test]
    public void TryCalculate_NoShotsReturnsNoResult()
    {
        bool found = ExposureTimingCalculator.TryCalculate(
            true,
            1f,
            new ShotSample[0],
            out _
        );

        Assert.That(found, Is.False);
    }

    [Test]
    public void TryCalculate_OnlyEarlierShotsReturnsNoResult()
    {
        ShotSample[] shots =
        {
            CreateShot(time: 0.5f),
            CreateShot(time: 0.9f)
        };

        bool found = ExposureTimingCalculator.TryCalculate(
            true,
            1f,
            shots,
            out _
        );

        Assert.That(found, Is.False);
    }

    [Test]
    public void TryCalculate_SelectsEarliestEligibleShotAndItsData()
    {
        ShotSample[] shots =
        {
            CreateShot(
                time: 1.6f,
                movementSpeed: 3f,
                wasStable: false,
                stoppedForMilliseconds: 0f
            ),
            CreateShot(
                time: 1.2f,
                movementSpeed: 0f,
                wasStable: true,
                stoppedForMilliseconds: 150f
            ),
            CreateShot(time: 0.7f)
        };

        bool found = ExposureTimingCalculator.TryCalculate(
            true,
            1f,
            shots,
            out ExposureTimingResult result
        );

        Assert.That(found, Is.True);
        Assert.That(result.Shot.Time, Is.EqualTo(1.2f));
        Assert.That(result.Shot.MovementSpeed, Is.EqualTo(0f));
        Assert.That(result.Shot.WasStable, Is.True);
        Assert.That(
            result.Shot.StoppedForMilliseconds,
            Is.EqualTo(150f)
        );
    }

    [Test]
    public void TryCalculate_MissingVisibilityIsNotTimeZero()
    {
        ShotSample[] shots = { CreateShot(time: 0f) };

        bool found = ExposureTimingCalculator.TryCalculate(
            hasVisibilityResult: false,
            visibleAtSeconds: 0f,
            shots,
            out _
        );

        Assert.That(found, Is.False);
    }

    private static ShotSample CreateShot(
        float time,
        float movementSpeed = 0f,
        bool wasStable = true,
        float stoppedForMilliseconds = 0f
    )
    {
        return new ShotSample(
            time,
            movementSpeed,
            wasStable,
            stoppedForMilliseconds
        );
    }
}
