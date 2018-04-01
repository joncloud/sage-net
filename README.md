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
Query1  0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C
Query2  0x54CB67D1746CD42CA947F6CE705060D0FB5540E55D588F5726CDAD0B73F41618
```

Use the `--hash` flag to change the hashing algorithm. By default Sage uses `SHA256`. Sage supports
 * MD5
 * SHA1
 * SHA256
 * SHA384
 * SHA512
```powershell
$Json | ./sage.cmd tab $ConnectionString --hash=MD5
Query1  0xF851F5BA5DEB579BBFE5D98E9CD268F6
Query2  0x048F9E6DEF2045421BC057264C16A042
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

Use the `json` command to export the information as JSON instead of Tab-Delimited. Note that the hashes produced by JSON output vary from the Tab-Delimited output.
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
