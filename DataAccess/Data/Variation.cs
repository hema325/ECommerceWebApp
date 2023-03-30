﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Variation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }

        //ignored
        public Category Category { get; set; }
        public List<Option> Options { get; set; }
    }
}
