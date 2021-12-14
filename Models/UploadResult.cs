using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntonioBatinicWebApp.Models
{
    // Return Object that shows a number of successful and failed reading
    public class UploadResult
    {
        // Total number of Successfully uploaded Readings
        public int SuccesfulReadings{ get; set; }
        // Total number of Unsuccessfully uploaded Readings
        public int FailedReadings{ get; set; }
    }
}