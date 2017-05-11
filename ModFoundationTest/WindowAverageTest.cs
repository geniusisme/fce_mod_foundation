using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortressCraft.ModFoundation;
using static ModFoundationTest.TestUtil;

namespace ModFoundationTest
{
[TestClass]
public class WindowAverageTest
{
    [TestMethod]
    public void AverageOnCreationIsStartingValue()
    {
        var ave = new WindowAverage(1f, 42f);
        AreClose(ave.Value, 42);
    }

    [TestMethod]
    public void AverageOverTwoIntervals()
    {
        var ave = new WindowAverage(1f, 20f);
        ave.AddMeasurement(8f, 0.5f);
        AreClose(14f, ave.Value);
    }

    [TestMethod]
    public void AverageAfterLongTime()
    {
        var ave = new WindowAverage(1f);
        for (var i = 0; i < 100; ++i)
        {
            ave.AddMeasurement(123f, 0.3f);
        }

        ave.AddMeasurement(20f, 1f);
        ave.AddMeasurement(8f, 0.5f);
        AreClose(14f, ave.Value);
    }
}
}
