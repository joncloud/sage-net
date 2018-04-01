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

For single queries, Sage.NET accepts a single JSON object to execute:
```powershell
$Query = @{
  name = "Query";
  commandText = "SELECT 1 [Num]"
}
$Json = $Query | ConvertTo-Json
$ConnectionString = "Data Source=.;Initial Catalog=master;Integrated Security=true;"
$Json | ./sage.cmd tab $ConnectionString
Query  0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C
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

Sage.NET will also report exceptions from query execution:
```powershell
$Query = @{
  name = "Query";
  commandText = "SELECT x [Num]"
}
$Json = $Query | ConvertTo-Json
$ConnectionString = "Data Source=.;Initial Catalog=master;Integrated Security=true;"
$Json | ./sage.cmd tab $ConnectionString
Query  System.Data.SqlClient.SqlException (0x80131904): Invalid column name 'x'.
   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at System.Data.SqlClient.SqlDataReader.get_MetaData()
   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)
   at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, SqlDataReader ds)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean asyncWrite, String method)
   at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior)
   at System.Data.SqlClient.SqlCommand.ExecuteReader()
   at Sage.Formatters.ReadDataTabDelimited(SqlCommand command, StreamWriter writer) in ./src/Sage/Formatters.cs:line 16
   at Sage.Program.LoadData(String connectionString, String file, String commandText, Formatter fn) in ./src/Sage/Program.cs:line 210
   at Sage.Program.ComputeHash(String connectionString, String name, String commandText, OutDir outDir, Formatter fn, HashAlgorithm algorithm) in ./src/Sage/Program.cs:line 180
   at Rlx.Functions.<>c__DisplayClass35_0`7.<Try>b__0()
   at Rlx.Attempt`1.Catch[TException]()
ClientConnectionId:f6663cff-34f2-49ba-8cad-326f3be6af17
Error Number:207,State:1,Class:16
```