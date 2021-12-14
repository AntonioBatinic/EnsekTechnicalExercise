using AntonioBatinicWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AntonioBatinicWebApp.Controllers
{
    public class ReadingsController : ApiController
    {
        [HttpPost]
        [Route("meter-reading-uploads")]
        public UploadResult MeterReadingUpload()
        {
            // Gets the first available posted file
            var postedFile = HttpContext.Current.Request.Files.Count > 0 ?
           HttpContext.Current.Request.Files[0] : null;

            // List of Meter Readings ; to be populated from the received CSV file
            List<MeterReading> meterReadings = new List<MeterReading>();

            // Initialize the Result object that will be returned
            UploadResult result = new UploadResult
            {
                SuccesfulReadings = 0,
                FailedReadings = 0
            };

            string filePath = string.Empty;
            // If the received file exists
            if (postedFile != null)
            {
                // Initiate datacontext
                EnsekProjDBDataContext dataContext = new EnsekProjDBDataContext();

                // Prepare a path on the Server to store received files
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Saves the file locally so that we can handle it later
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                // Read the contents of CSV file
                string csvData = System.IO.File.ReadAllText(filePath);

                // Trim the headers from the CSV file
                string trimmedCsvData = csvData.Substring(csvData.IndexOf('\n') + 1);

                // Execute a loop over the rows
                foreach (string row in trimmedCsvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {   
                        // Fill a list of Meter Reading objects
                        meterReadings.Add(new MeterReading
                        {
                            AccountId = Convert.ToInt32(row.Split(',')[0]),
                            MeterReadingDateTime = DateTime.Parse(row.Split(',')[1]),
                            MeterReadValue = row.Split(',')[2]
                        });
                    }
                }

                // Get a list of Authorized Account IDs
                var authorizedAccountIds = (from Account in dataContext.Accounts select Account.AccountId).Distinct();

                foreach (MeterReading singleReading in meterReadings)
                {
                    // If the reading is from an Authorized Account ID proceed
                    if(authorizedAccountIds.Contains(singleReading.AccountId))
                    {
                        // Ensure the Reading is in the correct format
                        if (int.TryParse(singleReading.MeterReadValue, out _))
                        {
                            // Check that the reading doesn't already exist in the Database
                            if (!CheckIfReadingExists(singleReading))
                            {
                                // Prepare the new row for Insertion
                                Meter_Reading newReading = new Meter_Reading();
                                newReading.AccountID = singleReading.AccountId;
                                newReading.MeterReadingDateTime = singleReading.MeterReadingDateTime;
                                newReading.MeterReadValue = Convert.ToInt32(singleReading.MeterReadValue);

                                // Insert the row and submit changes
                                dataContext.Meter_Readings.InsertOnSubmit(newReading);
                                dataContext.SubmitChanges();

                                // Mark the result as Successful
                                result.SuccesfulReadings++;
                            }
                            // Otherwise flag as a failed insert
                            else
                            {
                                result.FailedReadings++;
                            }
                        }
                        // Otherwise flag as a failed insert
                        else
                        {
                            result.FailedReadings++;
                        }
                    }
                    // Otherwise flag as a failed insert
                    else
                    {
                        result.FailedReadings++;
                    }
                }
            }

            return result;
        }

        // Checks if the reading already exists in the Database
        private bool CheckIfReadingExists(MeterReading singleReading)
        {

            EnsekProjDBDataContext dataContext = new EnsekProjDBDataContext();
            bool readingExists = false;

            // Get all readings from the database
            var allReadings = from Readings in dataContext.Meter_Readings
                        select Readings;
            // Iterate through every individual reading
            foreach(var reading in allReadings)
            {
                // Only if all three values match up (AccountID, DateTime, ReadValue) mark the Reading as already processed
                if(singleReading.AccountId == reading.AccountID &&
                   singleReading.MeterReadingDateTime == reading.MeterReadingDateTime &&
                   Convert.ToInt32(singleReading.MeterReadValue) == reading.MeterReadValue)
                {
                    readingExists = true;
                }
            }
            return readingExists;
        }
    }
}
