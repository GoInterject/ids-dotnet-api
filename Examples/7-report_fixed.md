# Example for `ReportFixed`

## PreReqs

- ensure a data connection exists for a running data api in the interject portal - https://portal.gointerject.com/DataPortalConnections.html

## 1) Setup Dataportal

- create a new dataportal on the portal site - https://portal.gointerject.com/DataPortals.html
- add the `Interject_RequestContext` system parameter
- (optional) add Formula Parameters

## 2) configure the `ReportFixed` formula in the excel report

- configure data range (RowDefRange). this is a single or multiple columns that defines what columns are used as a key to match incoming data with and what range of rows data should be populated on.
- configure columns definitions (ColDefRange). these define what columns are sent to the data api
- (optional) if multiple column attributes are needed per column (ie financials may need Source, Period, Year, etc...), configure `jColumnDef()` and replace a column name as a string with `jColumnDef()`.
- (optional) configure results range colums (ResultsRange). these define what columns are populated with data returned
- (optional) configure dataportal parameters
- for full example see the `ReportFixed` Tab in the `example.xlsx` file

## 3) create Post endpoint on data api to process save data

- the example below uses the spring framework for java used by this repo
- find the following api endpoint in the module: `interject-webapp\src\main\java\com\rest\interject\controller\TestController.java : testFixedReport`

<br>

### TestReportFixed Endpoint Controller

<br>

```java
@PostMapping(TEST_FIXED_REPORT)
    @Operation( summary = "Test Interject API(Fixed)",
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
public ResponseEntity<InterjectResponseDto> testFixedReport(HttpServletRequest request, @RequestBody @Valid InterjectRequestDto interjectRequest) {
        InterjectResponseDto resp = new InterjectResponseDto();

        try {
            // (optional) get dataportal formula param values
            // String reportParam1 = interjectRequest.getParamValue("Param1");
            // Integer reportParam2 =  interjectRequest.getParamValueInt("Param2");
            // Integer reportParam3 =  interjectRequest.getParamValueInt("Param3");
            // LOG.info("P1: " + reportParam1 + "   P2: " + reportParam2 + "   P3: " + reportParam3);

            InterjectTable idsTable = new InterjectTable();


            // (optional) process Column Definition Items
            List<InterjectColDefItem> colDefItems = interjectRequest.getColDefItems();
            for (InterjectColDefItem item : colDefItems){
                if(item.getRowDef() == null) {
                    idsTable.addColumn(new InterjectColumn(item.getValue()));
                }
            }

            // (optional) process Row Definition Items
            List<InterjectRowDefItem> rowDefItems = interjectRequest.getRowDefItems();
            for (InterjectRowDefItem item : rowDefItems){
                // int div = item.getValueInt("Div");       //this function is here if there is a need
                String div = item.getValueString("Div");
                String acct = item.getValueString("Acct");

                //result from database simulated
                //result = database.query(div,acct);
                //idstable.addRow(item.getRow(),div,acct,result[0][0], result[0][1], result[0][2]);

                // simulated values comming from database
                List<String> newRow = Arrays.asList(
                    div,
                    acct,
                    "Account_1",
                    "904.34",
                    "904.34",
                    "1000"
                );
                idsTable.addRow(newRow);
            }

            //
            // .... (optional) use data from the Interject for your stuff here ....
            // (optional) show contents of Interject Table object
            idsTable.print();

            // set response variables
            resp.addReturnedData(idsTable);

            // (optional)
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
