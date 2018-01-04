using System.Collections.Generic;

namespace FortressCraft.ModFoundation
{
/// Provides average of measurements over last time window
public class WindowAverage
{
    public WindowAverage(float averagingInterval, float startingValue = 0f)
    {
        this.AveragingInterval = averagingInterval;
        this.AddMeasurement(startingValue, averagingInterval);
    }

    public float Value
    {
        get
        {
            var oldestInterval = this.Intervals.Peek();
            var oldestMeasurement = this.Measurements.Peek();
            var oldestIntegral = oldestInterval * oldestMeasurement;
            var relevantInterval = oldestInterval - this.SumInterval + this.AveragingInterval;
            var relevantIntegral = relevantInterval * oldestMeasurement;
            return (this.IntegralMeasurement - oldestIntegral + relevantIntegral) / this.AveragingInterval;
        }
    }

    /// "during this interval value was observed to be equal our measurement"
    public void AddMeasurement(float measurement, float interval)
    {
        this.SumInterval += interval;
        this.Intervals.Enqueue(interval);
        this.IntegralMeasurement += measurement * interval;
        this.Measurements.Enqueue(measurement);

        while (this.SumInterval - this.Intervals.Peek() >= this.AveragingInterval)
        {
            var lateInterval = this.Intervals.Dequeue();
            this.SumInterval -= lateInterval;
            this.IntegralMeasurement -= this.Measurements.Dequeue() * lateInterval;
        }
    }

    readonly float AveragingInterval;
    float SumInterval;
    float IntegralMeasurement;
    Queue<float> Intervals = new Queue<float>();
    Queue<float> Measurements = new Queue<float>();
}
}
