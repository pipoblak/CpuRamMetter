Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq


Public Class YoutubeNewVideoCheck

    Public Function isLatestVideoANewVideo()
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Dim json As JObject
            Dim dbHelper As New DbHelper
            request = DirectCast(WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=UCm2CE2YfpmobBmF8ARLPzAw&order=date&key=AIzaSyAh1Bb31Y8kIjQcrZ_BzB27HvHB0JcGQBU"), HttpWebRequest)
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())

            Dim rawresp As String
            rawresp = reader.ReadToEnd()
            ' MsgBox(rawresp)
            json = JObject.Parse(rawresp)
            Dim videoId As String = json.SelectToken("").SelectToken("items[0]").SelectToken("").SelectToken("id").SelectToken("videoId")
            ' getLikeIfo("uk7gaHQ7lGM", getPlaylistId(DirectCast(dbHelper.getCredentials(), DbHelper.YtUser).username))
            Dim dateOfVideo As Date = Date.Parse(json.SelectToken("").SelectToken("items[0]").SelectToken("snippet").SelectToken("publishedAt").ToString)
            Dim difference As Integer = DateDiff(DateInterval.Minute, dateOfVideo, dbHelper.getLastDateInfoFromSqlite)
            If difference < 0 Then
                dbHelper.setDateInfoToSqlite(Date.Parse(json.SelectToken("").SelectToken("items[0]").SelectToken("snippet").SelectToken("publishedAt").ToString))


                Return True
            Else
                Return False
            End If




        Catch ex As Exception

        End Try

    End Function

    Public Function getLikeIfo(ByVal vId As String, ByVal playlistId As String)
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Dim json As JObject
            Dim dbHelper As New DbHelper
            request = DirectCast(WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet%2CcontentDetails&maxResults=50&playlistId=" & playlistId & "&videoId=" & vId & "&key=AIzaSyAh1Bb31Y8kIjQcrZ_BzB27HvHB0JcGQBU"), HttpWebRequest)
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())

            Dim rawresp As String
            rawresp = reader.ReadToEnd()
            ' MsgBox(rawresp)
            json = JObject.Parse(rawresp)
            Try
                Dim videoId As String = json.SelectToken("").SelectToken("items[0]").SelectToken("").SelectToken("contentDetails").SelectToken("videoId")
                Return True
            Catch ex As Exception
                Return False
            End Try




        Catch ex As Exception

        End Try

    End Function

    Public Function getPlaylistId(ByVal username As String)
        Try
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Dim json As JObject
            Dim dbHelper As New DbHelper
            request = DirectCast(WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=contentDetails&forUsername=" & username & "&key=AIzaSyAh1Bb31Y8kIjQcrZ_BzB27HvHB0JcGQBU"), HttpWebRequest)
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())

            Dim rawresp As String
            rawresp = reader.ReadToEnd()
            ' MsgBox(rawresp)
            json = JObject.Parse(rawresp)
            Try
                Dim playlistId As String = json.SelectToken("").SelectToken("items[0]").SelectToken("").SelectToken("contentDetails").SelectToken("relatedPlaylists").SelectToken("likes").ToString
                Return playlistId
            Catch ex As Exception
                Return ""
            End Try




        Catch ex As Exception

        End Try

    End Function



End Class
