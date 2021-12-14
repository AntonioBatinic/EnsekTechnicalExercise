Tested with Postman ; MeterReadingUpload returns { "SuccesfulReadings": 27, "FailedReadings": 8 }

DB Design: 
CREATE TABLE Meter_Readings( ID INT IDENTITY(1,1), AccountID INT NOT NULL, MeterReadingDateTime DATETIME2(7), MeterReadValue INT, PRIMARY KEY (ID))

CREATE TABLE Accounts( AccountId INT NOT NULL, FirstName VARCHAR(50), LastName VARCHAR(50))

**Relevant Folders:**
Controllers
Models
