$sqlConn = New-Object System.Data.SqlClient.SqlConnection
$sqlConn.ConnectionString = “Data Source=CVWCYD3\SQLEXPRESS;Initial Catalog=ProductTest;Integrated Security=True;”
$sqlConn.Open()
$sqlcmd = $sqlConn.CreateCommand()
$sqlcmd.Connection = $sqlConn
$query = “EXEC GetProduct”
$sqlcmd.CommandText = $query
$reader = $sqlcmd.ExecuteReader()
$data = 
while($reader.Read())
{
   
     [PSCustomObject] @{
        ID = $reader["ID"]
        Name = $reader["Name"]
       Description= $reader["Description"]
  
  }
}
$sqlConn.Close()
return $data