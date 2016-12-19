Imports Microsoft.Win32
Imports System.IO.Ports
Imports System.Windows.Threading

Public Class SerialConn
    Dim comPORT As String
    Dim receivedData As String = ""
    Dim SeP As String
    Dim timeNow As Integer = 0
    Dim incomingData As String
    Dim porta As New SerialPort
    Dim verif As Boolean = False
    Dim timer As New DispatcherTimer
    Dim conected As New Label
    Public conectedVerif As Boolean = False

    Public Sub New(ByRef conectedLabel As Label)
        conected = conectedLabel
        AddHandler timer.Tick, AddressOf timerTick
        timer.Interval = New TimeSpan(0, 0, 3)
        searchDevice()
    End Sub

    Public Sub timerTick(sender As Object, e As EventArgs)
        Try
            If porta.IsOpen Then
            Else
                porta.Open()
            End If
            porta.Write("$")
            receivedData = ReceiveSerialData()


            incomingData += receivedData

            If incomingData.Contains("CPURAMDEVICE") Then
                conected.Content = ""
                conectedVerif = True


            Else
                conected.Content = ""

                porta.Close()

            End If


            timer.Stop()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub


    Public Sub searchDevice()
        Try

            incomingData = ""
            comPORT = ""

            For Each sp As String In My.Computer.Ports.SerialPortNames

                Try
                    Try
                        porta.Close()
                    Catch ex As Exception

                    End Try
                    Try
                        porta.PortName = sp
                        porta.BaudRate = 115200
                        porta.DataBits = 8
                        porta.Parity = Parity.None
                        porta.StopBits = StopBits.One
                        porta.Handshake = Handshake.None
                        porta.Encoding = System.Text.Encoding.Default
                        porta.ReadTimeout = 10000

                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                    SeP = sp


                    porta.Open()

                    timer.Start()

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            Next
            If SeP = "" Then

            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Function ReceiveSerialData() As String
        Try
            Dim Incoming As String
            System.Threading.Thread.Sleep(200)
            Try
                Incoming = porta.ReadExisting()
                If Incoming Is Nothing Then
                    Return "nothing" & vbCrLf
                Else
                    Return Incoming
                End If
            Catch ex As TimeoutException

            End Try
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try



    End Function

    Public Sub write(ByVal str As String)
        Try
            porta.Write(str)
        Catch ex As Exception

        End Try

    End Sub

End Class
