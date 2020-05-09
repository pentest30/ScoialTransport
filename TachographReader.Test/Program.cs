using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tacchograaph_reader.Core.Linq;
using TachographReader.Application.Dtos.Activities;
using LogLevel = DataFileReader.LogLevel;

namespace TachographReader.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            

            //configure console logging
           
            var listOfRules = new List<IndemnityRule>()
            {
                new IndemnityRule()
                {
                    Label = "Indemnité de repas",
                    Price = 13.6f,
                    StarTime = new TimeSpan(0,11,45),
                    EndTime =  new TimeSpan(0,14,15),
                    SecondStarTime = new TimeSpan(0,18,45),
                    SecondEndTime = new TimeSpan(0,21,15)
                }
            };
           var tree=  GetExpressionTree(listOfRules);
            
        }
        //
        private static Expression GetExpressionTree(List<IndemnityRule> listOfRules)
        {
            var p1 = PredicateBuilder.False<DriverService>();
            foreach (var indemnityRule in listOfRules)
            {
                 p1 = p1.And( x =>
                    x.EndingBServiceTime.Subtract(x.BeginningServiceTime) >= indemnityRule.StarTime.Add(indemnityRule.EndTime)
                    || x.EndingBServiceTime.Subtract(x.BeginningServiceTime) >=
                    indemnityRule.SecondStarTime.Value.Add(indemnityRule.SecondEndTime.Value));
            }

            return p1;
        }
    }
}
