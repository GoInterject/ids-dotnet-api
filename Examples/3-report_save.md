# Example for `ReportSave`

## PreReqs
- ensure a data connection exists for a running data api in the interject portal - https://portal.gointerject.com/DataPortalConnections.html


## 1) Setup Dataportal
- create a new dataportal on the portal site - https://portal.gointerject.com/DataPortals.html
- add the `Interject_XMLDataToSave` system parameter
- (optional) add Formula Parameters


## 2) configure the `ReportSave` formula in the excel report
- configure data range (RowDefRange). these define what rows are sent to the data api
- configure columns definitions (ColDefRange). these define what columns are sent to the data api
- (optional) configure results range colums (ResultsRange). these define what columns are populated with data returned
- (optional) configure dataportal parameters
- for full example see the `ReportSave` Tab in the `example.xlsx` file


## 3) create Post endpoint on data api to process save data
- the example below uses the spring framework for java used by this repo
- find the following api endpoint in the module: `interject-webapp\src\main\java\com\rest\interject\controller\TestController.java : testReportSave`

<br>

### TestReportSave Endpoint Controller

<br>

```java
@PostMapping(TEST_SAVE_URL)
@Operation( 
    summary = "Test Interject API",
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
public ResponseEntity<InterjectResponseDto> testReportSave(
    HttpServletRequest request, 
    @RequestBody @Valid InterjectRequestDto interjectRequest
) {    
    // initialize the Interject Response object
    InterjectResponseDto resp = new InterjectResponseDto();

    try {
        // get the data from the spreadsheet via the dataportal system parameter: 'XmlDataToSave'
        InterjectTable table = interjectRequest.getXmlDataToSave();

        // (optional) get dataportal formula param values
        String reportParam1 = interjectRequest.getParamValue("Param1");
        Integer reportParam2 =  interjectRequest.getParamValueInt("Param2");
        LOG.info("P1: " + reportParam1 + "   P2: " + reportParam2);

        // 
        // .... (optional) use data from the spreadsheet for your stuff here ....
        //

        // (optional) add columns and row data for ReportSave's 'ResultsRange'
        table.addColumn(new InterjectColumn("notes"));
        table.update("notes", 1, "my new note");
        table.addRowAtEnd(Arrays.asList("234", "aber", "my new note"));

        // (optional) show contents of Interject Table object
        table.print();

        // set response variables
        resp.addReturnedData(interjectRequest.getXmlDataToSave());
        resp.setRequestParameterList(interjectRequest.getRequestParameterList());

        // postprocess response object 
        // (handles stuff like serializing Returned Data Objects)
        resp.prepare();
    }

    catch (Exception ex) {
        // (optional) return an error message to interject 
        //            that a user will be able to see in Excel
        resp.setErrorMessage(ex.toString());

        // (optional) log error to Log4j
        LOG.error(ex.toString());
    }

    return ok(resp);
}
```