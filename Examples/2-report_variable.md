# Example for `ReportVariable`

## PreReqs
- ensure a data connection exists for a running data api in the interject portal - https://portal.gointerject.com/DataPortalConnections.html


## 1) Setup Dataportal
- create a new dataportal on the portal site - https://portal.gointerject.com/DataPortals.html
- add the `Interject_RequestContext` system parameter
- (optional) add Formula Parameters


## 2) configure the `ReportVariable` formula in the excel report
- configure data range (RowDefRange). this is a single column that defines what column is used as a key to match incoming data with and what range of rows data should be populated on.
- configure columns definitions (ColDefRange). these define what columns are sent to the data api
- (optional) if multiple column attributes are needed per column (ie financials may need Source, Period, Year, etc...), configure `jColumnDef()` and replace a column name as a string with `jColumnDef()`.
- (optional) configure results range colums (ResultsRange). these define what columns are populated with data returned
- (optional) configure dataportal parameters
- for full example see the `ReportVariable` Tab in the `example.xlsx` file


## 3) create Post endpoint on data api to process save data
- the example below uses the spring framework for java used by this repo
- find the following api endpoint in the module: `interject-webapp\src\main\java\com\rest\interject\controller\TestController.java : testReportVariable`

<br>

### TestReportVariable Endpoint Controller

<br>

```java
@PostMapping(TEST_VARIABLE_URL)
@Operation( summary = "Test Interject API",
            description = "",
            responses = {
                    @ApiResponse(
                        responseCode = "200", 
                        description = "Successfully tested",
                        content = @Content(
                            mediaType = "application/json", 
                            schema = @Schema(implementation = InterjectResponseDto.class)
                        )
                    )
            },
            parameters = {/** none */}
)
public ResponseEntity<InterjectResponseDto> testReportVariable(HttpServletRequest request, @RequestBody @Valid InterjectRequestDto interjectRequest) {
    InterjectResponseDto resp = new InterjectResponseDto();

    try {
        // InterjectRequestContext requestContext = interjectRequest.getRequestContext();
        List<InterjectRowColItem> colDefItems = interjectRequest.getColDefItems();
        List<InterjectRowColItem> rowDefItems = interjectRequest.getRowDefItems();

        // (optional) get dataportal formula param values
        String reportParam1 = interjectRequest.getParamValue("Param1");
        Integer reportParam2 =  interjectRequest.getParamValueInt("Param2");
        Integer reportParam3 =  interjectRequest.getParamValueInt("Param3");
        LOG.info("P1: " + reportParam1 + "   P2: " + reportParam2 + "   P2: " + reportParam2);

        // return data
        InterjectTable idsTable = new InterjectTable();
        idsTable.addColumn(new InterjectColumn("AccountType"));
        idsTable.addColumn(new InterjectColumn("Segment1"));
        idsTable.addColumn(new InterjectColumn("jColumnDef_1"));
        idsTable.addColumn(new InterjectColumn("jColumnDef_2"));
        idsTable.addColumn(new InterjectColumn("jColumnDef_3"));
        idsTable.addRow(Arrays.asList("Revenue - Sales", "123098", "904.34", "904.34", "904.34"));
        idsTable.addRow(Arrays.asList("Revenue - Sales", "123099", "1004.234", "443.14", "90.40"));
        idsTable.addRow(Arrays.asList("Revenue - Other", "123080", "1004.234", "443.14", "90.40"));
        idsTable.addRow(Arrays.asList("Revenue - Other", "123081", "1004.234", "443.14", "90.40"));
        idsTable.print();
        
        // set response variables
        resp.addReturnedData(idsTable);
        resp.setRequestParameterList(interjectRequest.getRequestParameterList());
        resp.prepare();
    }

    catch (Exception ex) {
        resp.setErrorMessage(ex.toString());
        LOG.error(ex.toString());
    }

    return ok(resp);
}
```