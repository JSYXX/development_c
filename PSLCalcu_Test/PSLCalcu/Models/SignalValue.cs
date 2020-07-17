using System;

public class TagValue
{
    protected int _status;
    protected DateTime _timestamp;
    protected double _value;

    public TagValue(DateTime timestamp, double value, int status);

    public int Status { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }

    public TagValue Clone();
}