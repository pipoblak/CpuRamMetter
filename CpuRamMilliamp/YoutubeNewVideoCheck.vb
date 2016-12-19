Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Data.SQLite
Imports System.Data.Common
Imports System.Data

Public Class YoutubeNewVideoCheck

    Public Function isLatestVideoANewVideo()
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Dim json As JObject
            request = DirectCast(WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=UCm2CE2YfpmobBmF8ARLPzAw&order=date&key=AIzaSyAh1Bb31Y8kIjQcrZ_BzB27HvHB0JcGQBU"), HttpWebRequest)
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())

            Dim rawresp As String
            rawresp = reader.ReadToEnd()
            ' MsgBox(rawresp)
            json = JObject.Parse(rawresp)

            Dim dateOfVideo As Date = Date.Parse(json.SelectToken("").SelectToken("items[0]").SelectToken("snippet").SelectToken("publishedAt").ToString)
            Dim difference As Integer = DateDiff(DateInterval.Minute, dateOfVideo, getLastDateInfoFromSqlite)
            If difference < 0 Then
                setDateInfoToSqlite(Date.Parse(json.SelectToken("").SelectToken("items[0]").SelectToken("snippet").SelectToken("publishedAt").ToString))
                Return True
            Else
                Return False
            End If




        Catch ex As Exception

        End Try

    End Function
    Public Sub setDateInfoToSqlite(ByVal dt As String)
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=false; FailIfMissing=false;", arquivoDB)
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS VideoDateInfo (dtVideo text);"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "SELECT dtVideo FROM VideoDateInfo"

                    Using reader As DbDataReader = cmd.ExecuteReader()


                        If reader.HasRows Then
                            reader.Close()
                            cmd.CommandText = "UPDATE VideoDateInfo SET dtVideo=@dtVideo"
                        Else
                            reader.Close()
                            cmd.CommandText = "INSERT INTO VideoDateInfo (dtVideo) VALUES(@dtVideo)"
                        End If
                    End Using
                    Dim p2 = cmd.CreateParameter()
                    p2.ParameterName = "@dtVideo"
                    p2.Value = dt
                    cmd.Parameters.Add(p2)
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                End Using
                If dbConn.State <> ConnectionState.Closed Then
                    dbConn.Close()
                End If
                dbConn.Dispose()
                factory.Dispose()
            End Using
        End Using

    End Sub
    Public Function getLastDateInfoFromSqlite()
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=false; FailIfMissing=false;", arquivoDB)
        Dim dtVideo As String = ""
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "SELECT dtVideo FROM VideoDateInfo"

                    Using reader As DbDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            dtVideo = reader.GetString(0)
                        Else
                            dtVideo = DateTime.Now.ToString
                            setDateInfoToSqlite(dtVideo)
                        End If
                    End Using
                    cmd.Dispose()
                End Using
                If dbConn.State <> ConnectionState.Closed Then
                    dbConn.Close()
                End If
                dbConn.Dispose()
                factory.Dispose()
            End Using
        End Using

        Return dtVideo
    End Function



End Class
