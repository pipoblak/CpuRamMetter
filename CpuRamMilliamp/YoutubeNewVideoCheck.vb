Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports Google.Apis.YouTube.v3
Imports Google.Apis.Auth.OAuth2
Imports System.Threading
Imports Google.Apis.Util.Store
Imports Google.Apis.Services


Public Class YoutubeNewVideoCheck
    Dim ytService As YouTubeService
    Dim credential As UserCredential
    Dim scopes As IList(Of String) = New List(Of String)()
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

    Public Function isLatestVideoLiked()
        Try
            scopes.Add(YouTubeService.Scope.Youtube)
            Using stream As New FileStream(System.AppDomain.CurrentDomain.BaseDirectory & "\client_id.json", FileMode.Open, FileAccess.Read)
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets, scopes, "user", CancellationToken.None,
                    New FileDataStore("YtCheckasdas")).Result
            End Using
            Dim initializer As New BaseClientService.Initializer()
            initializer.HttpClientInitializer = credential
            initializer.ApplicationName = "YtCheck"
            ytService = New YouTubeService(initializer)

            'RETRIEVE PLAYLISTID LIKE
            Dim channelPlaylistRequest = ytService.Channels.List("contentDetails")
            channelPlaylistRequest.Mine = True
            Dim listChannel As IList(Of Data.Channel) = channelPlaylistRequest.Execute().Items()

            Dim playlistId As String = DirectCast(listChannel.Item(0), Data.Channel).ContentDetails.RelatedPlaylists.Likes


            'RETRIEVE LATEST VIDEOS
            Dim searchRequest = ytService.Search.List("snippet")
            searchRequest.ChannelId = "UCm2CE2YfpmobBmF8ARLPzAw"
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date
            Dim listSearch As IList(Of Data.SearchResult) = searchRequest.Execute().Items()
            Dim videoId As String = DirectCast(listSearch.Item(0), Data.SearchResult).Id.VideoId


            'RETRIEVE is LIKED VIDEO
            Dim playlistRequest = ytService.PlaylistItems.List("snippet,contentDetails")
            playlistRequest.VideoId = videoId
            playlistRequest.PlaylistId = playlistId
            Dim listPlaylist As IList(Of Data.PlaylistItem) = playlistRequest.Execute.Items()
            Dim isVideoLiked As Boolean
            Try
                Dim haveLiked As String = DirectCast(listPlaylist.Item(0), Data.PlaylistItem).ContentDetails.VideoId
                isVideoLiked = True

            Catch ex As Exception
                isVideoLiked = False
            End Try


            Return isVideoLiked

        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
