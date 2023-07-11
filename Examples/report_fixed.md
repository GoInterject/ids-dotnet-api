# Example for `ReportFixed`

## 1) Setting Up A Data Connection

- Ensure a data connection exists for a running data api on the interject portal - https://portal.gointerject.com/DataPortalConnections.html

For directions on how to setup a Data Connection for an API, see: https://docs.gointerject.com/wPortal/L-Api-Connections.html

## 2) Setting Up A Data Portal

- Create a new dataportal on the portal site - https://portal.gointerject.com/DataPortals.html
- Add the `Interject_RequestContext` system parameter
- (Optional) Add other Formula Parameters

For directions on how to set up a Data Portal for an API, see: https://docs.gointerject.com/wPortal/Data-Portals.html

## 3) Configure the `ReportFixed` Function in the Excel Report
- Configure the data range (RowDefRange). This is a single column that defines what column is used as a key to match incoming data with and what range of rows data should be populated on.
- Configure the columns definitions (ColDefRange). This range defines what columns are sent to the data api.
- (Optional) If multiple column attributes are needed per column (i.e. financials may need Source, Period, Year, etc.), configure `jColumnDef()` and replace a column name as a string with `jColumnDef()`. For an example of this, see: https://docs.gointerject.com/wGetStarted/L-Create-FinancialFixed.html
- (Optional) Configure the dataportal parameters.

For a full working example report, see the `ReportFixed` Tab in the `example.xlsx` file.

For directions on how to set up a ReportFixed report, see: https://docs.gointerject.com/wGetStarted/L-Create-InventoryFixed.html

For documentation on the ReportFixed function, see: https://docs.gointerject.com/wIndex/ReportFixed.html

## 4) Create A Post Endpoint on Data API To Process the Data Request

For a working example of a ReportFixed endpoint, see the method `TestReportFixed` in the `ExampleController.cs`.
