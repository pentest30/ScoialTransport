using System;


namespace tacchograaph_reader.Core.Entities
{
    public class CardDriverActivity : BaseEntity
    {
        public CardActivityDailyRecord ActivityDailyRecord { get; set; }
        public Guid ActivityDailyRecordId { get; set; }
        public byte SlotOne { get; set; }
        public bool CardPresent { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DriverActivityType DriverActivityType { get; set; }
        public string Offset { get; set; }
        public DateTime ActivityUtc { get; set; }
    }

    public enum DriverActivityType
    {
        Break,
        Available,
        Work,
        Driving
    }
}
