using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntonioBatinicWebApp.Models
{
    public class MeterReading
    {
        // Gets or sets AccountId.
        public int AccountId { get; set; }

        // Gets or sets DateTime of the Reading.
        public DateTime MeterReadingDateTime { get; set; }

        // Gets or sets the Value Read
        public string MeterReadValue { get; set; }
    }
}