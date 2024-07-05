# Working with the `jColumnDef` Formula

The `jColumnDef` formula can be used to get extra details about a column such as source, year or custom defined segments for accounting purposes. The example below shows how to configue the spreadsheet report with the formula and how to access the data the formula provides from the data api side.

<br>

# The Report

![](./static/formula-1-jcolumndef_json.png)
<br>

# The Data Portal

With the report configured, now we need to make sure the data portal on the [Interject customer portal site](https://portal.gointerject.com/DataPortals.html) includes either the system parameter, `Interject_ColDefItems` or `Interject_RequestContext`.

![](static/formula-2-coldefitems.png)
<br>

# The Data API

From the data API side, Column Definition Items can be accessed from the request object with the _GetColDefItems_ method of the `InterjectRequest` object. This will return a list of `InterjectColDefItems` that represent each column of the report.

To access the column attributes from the `jColumnDef` formula, access variables in the `InterjectColDefItem.Json` hashmap as shown below with `item.Json["<KEY>"]`. All available keys are show below in the json object.

```csharp
// how to iterate over column def items and get jColumnDef attributes
List<InterjectColDefItem> colDefItems = interjectRequest.GetColDefItems();
foreach (InterjectColDefItem item in colDefItems) {
    Console.Write(item.ToString());
    String period = item.Json["P"];
}
```

```jsonc
// sample json data stored in InterjectRowColItem.Json hashmap
{
  "S": "actual",
  "P": "1",
  "Y": "2022",
  "V": "A",
  "S1": "asdh",
  "S2": "23",
  "S3": "234",
  "S4": "fff",
  "S5": "85kk-sdf",
  "S6": "ss",
  "S7": "ffgh",
  "S8": "f78",
  "B": "2347889"
}
```
