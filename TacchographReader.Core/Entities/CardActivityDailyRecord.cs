using System;
using System.Collections.Generic;

namespace tacchograaph_reader.Core.Entities
{
    public class CardActivityDailyRecord : BaseEntity
    {
        public CardActivityDailyRecord()
        {
            Activities = new List<CardDriverActivity>();
        }
        public string CardNumber { get; set; }
        public DateTime Date { get; set; }
        public double TotalDistance { get; set; }
        public ICollection<CardDriverActivity> Activities { get; set; }

    }
}
