﻿using System.Collections.Generic;

namespace AbdusCo.CronJobs
{
    public class JobDescription
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public List<string> CronExpressions { get; set; }
        public string? Description { get; set; }
    }
}