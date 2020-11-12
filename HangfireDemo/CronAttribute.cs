﻿using System;
using System.Linq;

namespace HangfireDemo
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CronAttribute : Attribute
    {
        public string[] CronExpressions { get; }

        public CronAttribute(params string[] cronExpressions)
        {
            if (cronExpressions.Any(e => !IsValid(e)))
            {
                throw new ArgumentException("Invalid cron expression", nameof(cronExpressions));
            }

            CronExpressions = cronExpressions;
        }

        private bool IsValid(string cron) => cron.Split(" ").Length == 5;
    }
}