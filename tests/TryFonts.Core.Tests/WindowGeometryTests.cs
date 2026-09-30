using TryFonts.Core.Models;

namespace TryFonts.Core.Tests;

public sealed class WindowGeometryTests
{
    [Fact]
    public void ConstrainTo_VisibleGeometry_PreservesSizeAndPosition()
    {
        var geometry = new WindowGeometry(900, 600, 200, 150);
        Assert.Equal(geometry, geometry.ConstrainTo(0, 48, 2560, 1518, 1.5, 16, 40));
    }

    [Fact]
    public void ConstrainTo_OversizedSavedWindow_AccountsForScalingAndTitleBar()
    {
        var geometry = new WindowGeometry(1706.6666666666667, 1012, -11, 133)
            .ConstrainTo(0, 48, 2560, 1518, 1.5, 16, 40);

        Assert.Equal(0, geometry.X);
        Assert.Equal(48, geometry.Y);
        Assert.True((geometry.Width + 16) * 1.5 <= 2560);
        Assert.True((geometry.Height + 40) * 1.5 <= 1518);
    }

    [Fact]
    public void ConstrainTo_NegativeMonitorCoordinates_RemainOnThatMonitor()
    {
        var geometry = new WindowGeometry(900, 600, -1800, 100);
        Assert.Equal(geometry, geometry.ConstrainTo(-1920, 0, 1920, 1040, 1, 16, 40));
    }

    [Theory]
    [InlineData(double.NaN, double.NaN)]
    [InlineData(double.PositiveInfinity, double.NegativeInfinity)]
    [InlineData(-32000, -32000)]
    public void ConstrainTo_InvalidOrOffscreenPosition_KeepsEntireFrameVisible(double x, double y)
    {
        var geometry = new WindowGeometry(900, 600, x, y).ConstrainTo(0, 48, 1920, 1032, 1, 16, 40);
        Assert.InRange(geometry.X, 0, 1920 - 916);
        Assert.InRange(geometry.Y, 48, 1080 - 640);
    }

    [Theory]
    [InlineData(double.NaN, double.PositiveInfinity)]
    [InlineData(-1, 0)]
    public void ConstrainTo_InvalidSize_UsesVisibleDefaults(double width, double height)
    {
        var geometry = new WindowGeometry(width, height, double.NaN, double.NaN)
            .ConstrainTo(0, 0, 1920, 1040, 1, 16, 40);
        Assert.Equal(1200, geometry.Width);
        Assert.Equal(800, geometry.Height);
    }

    [Fact]
    public void ConstrainTo_SmallWorkingArea_DoesNotExceedScreenToMeetMinimumSize()
    {
        var geometry = new WindowGeometry(900, 600, 100, 100).ConstrainTo(0, 0, 600, 350, 1, 16, 40);
        Assert.Equal(584, geometry.Width);
        Assert.Equal(310, geometry.Height);
        Assert.Equal(0, geometry.X);
        Assert.Equal(0, geometry.Y);
    }
}
