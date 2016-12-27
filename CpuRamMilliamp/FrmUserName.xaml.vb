Public Class FrmUserName
    Private Sub button_Click(sender As Object, e As RoutedEventArgs) Handles button.Click
        Dim dbHelper As New DbHelper
        dbHelper.setUserInfo(txtUsername.Text)
        MsgBox(DirectCast(dbHelper.getCredentials, DbHelper.YtUser).playlistId)
    End Sub
End Class
