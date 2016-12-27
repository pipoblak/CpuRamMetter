Imports System.Data.SQLite
Imports System.Data.Common
Imports System.Data
Public Class DbHelper
    Public Sub setUserInfo(ByVal username As String)
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=false; FailIfMissing=false;", arquivoDB)
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS YtUser (username text, playlistId text);"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "SELECT * FROM YtUser"

                    Using reader As DbDataReader = cmd.ExecuteReader()


                        If reader.HasRows Then
                            reader.Close()
                            cmd.CommandText = "UPDATE YtUser SET username=@username , playlistId=@playlistId  "
                        Else
                            reader.Close()
                            cmd.CommandText = "INSERT INTO YtUser (username,playlistId) VALUES(@username,@playlistId)"
                        End If
                    End Using
                    Dim ytCheck As New YoutubeNewVideoCheck
                    Dim p1 = cmd.CreateParameter()
                    p1.ParameterName = "@username"
                    p1.Value = username
                    Dim p2 = cmd.CreateParameter()
                    p2.ParameterName = "@playlistId"
                    p2.Value = ytCheck.getPlaylistId(username)
                    cmd.Parameters.Add(p1)
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
    Public Sub setDateInfoToSqlite(ByVal dt As String)
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & " \ ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=False; FailIfMissing=False;", arquivoDB)
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "CREATE TABLE If Not EXISTS VideoDateInfo (dtVideo text);"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "Select dtVideo FROM VideoDateInfo"

                    Using reader As DbDataReader = cmd.ExecuteReader()


                        If reader.HasRows Then
                            reader.Close()
                            cmd.CommandText = "UPDATE VideoDateInfo Set dtVideo=@dtVideo"
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
    Public Sub setLikeInfoToSqlite(ByVal id As String, ByVal liked As String)
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=False; FailIfMissing=False;", arquivoDB)
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "CREATE TABLE If Not EXISTS VideoLikeInfo (id text, liked Boolean);"
                    cmd.ExecuteNonQuery()
                    cmd.CommandText = "Select * FROM VideoLikeInfo where id='" & id & "'"

                    Using reader As DbDataReader = cmd.ExecuteReader()


                        If reader.HasRows Then
                            reader.Close()
                            cmd.CommandText = "UPDATE VideoLikeInfo SET liked=@liked"
                        Else
                            reader.Close()
                            cmd.CommandText = "INSERT INTO VideoLikeInfo (id,liked) VALUES(@id,@liked)"
                            Dim p1 = cmd.CreateParameter()
                            p1.ParameterName = "@id"
                            p1.Value = id
                            cmd.Parameters.Add(p1)
                        End If
                    End Using
                    Dim p2 = cmd.CreateParameter()
                    p2.ParameterName = "@liked"
                    p2.Value = liked

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
    Public Function isVideoLiked(ByVal id As String)
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=false; FailIfMissing=false;", arquivoDB)
        Dim liked As Boolean = False
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "SELECT liked FROM VideoLikeInfo where id='" & id & "'"

                    Using reader As DbDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            liked = reader.GetBoolean(0)
                        Else
                            liked = False
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

        Return liked
    End Function

    Public Function getCredentials()
        Dim arquivoDB = System.AppDomain.CurrentDomain.BaseDirectory & "\ytCheck.db"
        Dim connString = String.Format("Data Source={0}; Pooling=false; FailIfMissing=false;", arquivoDB)
        Dim liked As Boolean = False
        Dim ytUser As New YtUser
        Using factory = New SQLiteFactory()
            Using dbConn As DbConnection = factory.CreateConnection()
                dbConn.ConnectionString = connString
                dbConn.Open()
                Using cmd As DbCommand = dbConn.CreateCommand()
                    cmd.CommandText = "SELECT * FROM YtUser"

                    Using reader As DbDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            ytUser = New YtUser(reader.GetString(0), reader.GetString(1))
                        Else

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

        Return ytUser
    End Function

    Public Class YtUser
        Public username As String = ""
        Public playlistId As String = ""

        Public Sub New(ByVal username As String, ByVal playlistId As String)
            Me.username = username
            Me.playlistId = playlistId
        End Sub
        Public Sub New()

        End Sub
    End Class
End Class
