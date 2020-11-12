using System;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CronAttribute : Attribute
    {
        public string[] CronExpressions { get; }
        
        public CronAttribute(params string[] cronExpressions)
        {
            CronExpressions = cronExpressions;
        }
    }
}