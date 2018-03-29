# Sage.NET

## Description
Sage.NET queries SQL Server databases and converts results into a hash.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Usage
Sage.NET accepts a JSON array of queries to execute, and then hashes all of the results. Use the following example to connect to localhost.
```powershell
$Queries = @(
  @{
    name = "Query1";
    commandText = "SELECT 1 [Num]"
  },
  @{
    name = "Query2";
    commandText = "SELECT 2 [Num]"
  }
)
$Json = $Queries | ConvertTo-Json
$ConnectionString = "Data Source=.;Initial Catalog=master;Integrated Security=true;"
$Json | ./sage.cmd tab $ConnectionString
Query1  0x07846B783C23904A75834C1932BC2FAD41E388E18BC32FFD73ED87BFA3844D87
Query2  0x24C813CB6F9BADE379034E39A4665BF3BE056293A53543502481E4F3DB7E9291
```

Use the `--out-dir` flag in order to persist the information from the resulting query.
```powershell
$Json | ./sage.cmd tab $ConnectionString --out-dir=./
...
Get-Content ./Query1
1
Get-Content ./Query2
2
```

Use the `json` command to export the information as JSON instead of Tab-Delimited.
```powershell
$Json | ./sage.cmd json $ConnectionString --out-dir=./
...
Get-Content ./Query1
[
  {
    "Num":1

  }
]
Get-Content ./Query2
[
  {
    "Num":2

  }
]
```
