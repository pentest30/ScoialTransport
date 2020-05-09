using System;
using System.Collections.Generic;
using System.Linq;
using TachographReader.Application.Dtos.Activities;
using TachographReader.Application.Dtos.Driver;

namespace TachographReader.Application.Services
{
    public class DetectionOfIndemnities 
    {
        public DriverPeriodActivitiesDto DriverPeriodActivities { get ; set ; }

        public void DetectIndemnities()
        {
           
            var listOfRules = new List<IndemnityRule>
            {
                new IndemnityRule
                {
                    IndemnityIntervals = new List<IndemnityInterval>
                    {
                        new IndemnityInterval
                        {
                            StarTime = new TimeSpan(11,45,0),
                            EndTime = new TimeSpan(14,15,0)
                        },
                        new IndemnityInterval
                        {
                            StarTime = new TimeSpan(18,45,0),
                            EndTime = new TimeSpan(21,15,0)
                        }
                    },
                    Label = "Indemnité de repas",
                    Price = 13.6f,
                   
                    TotalDistanceRequired = true,
                    MaxTotalDistance = 10
                },
                new IndemnityRule()
                {
                    Label = "Repas sur le lieu de travail ",
                    Price = 13.6f,
                    IndemnityIntervals = new List<IndemnityInterval>
                    {
                        new IndemnityInterval
                        {
                            StarTime = new TimeSpan(11,0,0),
                            EndTime = new TimeSpan(14,30,0)
                        },
                        new IndemnityInterval
                        {
                            StarTime = new TimeSpan(18,30,0),
                            EndTime = new TimeSpan(22,00,0)
                        }
                    },
                   
                    TotalDistanceRequired = false,
                    MaxTotalDistance = 1
                },
                new IndemnityRule()
                {
                    Label = "Service de nuit",
                    Price = 13.6f,
                    IndemnityIntervals = new List<IndemnityInterval>
                    {
                        new IndemnityInterval
                        {
                            StarTime = new TimeSpan(22,0,0),
                            EndTime = new TimeSpan(7,0,0)
                        },

                    }
                 
                }
            };
            //var tree = GetExpressionTree(listOfRules);
            //var r = DriverPeriodActivities.DriverServices.AsQueryable().Where(tree);
            foreach (var indemnityRule in listOfRules)
            {
                var predicateFilter = PredicateFilter(indemnityRule);
                var r = DriverPeriodActivities.DriverServices.Where(predicateFilter);
                foreach (var driverService in r.ToList())
                {
                    Console.WriteLine(driverService.BeginningServiceTime + " end: " + driverService.EndingBServiceTime);
                    Console.WriteLine(indemnityRule.Label);
                }
            }
           
        }

        private static Func<DriverService, bool> PredicateFilter(IndemnityRule indemnityRule)
        {
            bool Filter(DriverService x)
            {
                bool b = false;
                if (indemnityRule.TotalDistanceRequired != null && (indemnityRule.TotalDistanceRequired.Value && x.TotalDistance==indemnityRule.MaxTotalDistance) )
                    return false;
                if (indemnityRule.TotalDistanceRequired != null && (!indemnityRule.TotalDistanceRequired.Value && x.TotalDistance > indemnityRule.MaxTotalDistance))
                    return false;

                foreach (var indemnityInterval in indemnityRule.IndemnityIntervals)
                {
                    if (indemnityInterval.StarTime < new TimeSpan(12, 0, 0))
                        b = x.BeginningServiceTime.ToLocalTime().TimeOfDay <= indemnityInterval.StarTime && x.EndingBServiceTime.ToLocalTime().TimeOfDay >= indemnityInterval.EndTime;
                    else
                        b = x.BeginningServiceTime.ToLocalTime().TimeOfDay >= indemnityInterval.StarTime && x.EndingBServiceTime.ToLocalTime().TimeOfDay >= indemnityInterval.StarTime;
                    return b;
                }

                return b;
            }

            return Filter;
        }

        
        
    }
}
