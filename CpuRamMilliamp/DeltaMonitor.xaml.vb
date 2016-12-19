Imports System.Drawing
Imports System.Windows.Threading
Imports Microsoft.Win32

Class DeltaMonitor

    Dim dispatcherTimer As New DispatcherTimer
    Dim dispatcherTimerVideoCheck As New DispatcherTimer
    Dim cpu As New System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total")
    Dim pageCounter As New System.Diagnostics.PerformanceCounter("Paging File", "% Usage", "_Total")
    Dim conection As SerialConn
    Private m_ico As Icon
    Dim notifyIcon As New System.Windows.Forms.NotifyIcon()
    Dim percentCPU As String = ""
    Dim percentRam As String = ""
    Dim ytCheck As New YoutubeNewVideoCheck



    Private Sub window_Loaded(sender As Object, e As RoutedEventArgs) Handles window.Loaded
        conection = New SerialConn(lblDeviceStatus)
        Dim regStartUp As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

        Dim value As String

        value = regStartUp.GetValue("CPURAMMetter")

        If value <> My.Application.StartupUri.ToString() Then

            regStartUp.CreateSubKey("CPURAMMetter")
            regStartUp.SetValue("CPURAMMetter", My.Application.StartupUri.ToString())

        End If

        Dim m_ico As Icon = System.Drawing.Icon.FromHandle((My.Resources.MMlogo.GetHicon))
        notifyIcon.Icon = m_ico
        notifyIcon.Visible = True
        AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        AddHandler dispatcherTimerVideoCheck.Tick, AddressOf ytVerification
        AddHandler notifyIcon.DoubleClick, AddressOf notfyClick
        ''AddHandler notifyIcon.Click, AddressOf notifyTip
        dispatcherTimer.Interval = New TimeSpan(0, 0, 0.55)
        dispatcherTimerVideoCheck.Interval = New TimeSpan(0, 0, 3)
        dispatcherTimer.Start()
        dispatcherTimerVideoCheck.Start()
        notifyIcon.BalloonTipText = "CPU: " & lblCPUUsage.Text & "  RAM: " & lblRAMUsage.Text
        notifyIcon.BalloonTipTitle = "DELTA MONITOR"
        Me.Hide()

    End Sub

    Public Sub ytVerification()
        If conection.conectedVerif Then
            If ytCheck.isLatestVideoANewVideo Then
                conection.write("@")
            End If
        End If

    End Sub
    Public Sub notfyClick(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Forms.MouseButtons.Left Then
            If Me.IsVisible Then
                Me.Hide()
            Else
                Me.Show()
            End If
        Else
            notifyTip()
        End If




    End Sub
    Public Sub notifyTip()

        notifyIcon.ShowBalloonTip(0)
    End Sub
    Public Sub dispatcherTimer_Tick()
        Dim doublecpu As Double = (Math.Round(cpu.NextValue, 2, MidpointRounding.AwayFromZero) + 10)
        If doublecpu > 100 Then
            doublecpu -= 10
        End If

        percentCPU = doublecpu.ToString("##.##") & "%"
        percentRam = Math.Round((My.Computer.Info.TotalPhysicalMemory - My.Computer.Info.AvailablePhysicalMemory) / My.Computer.Info.TotalPhysicalMemory * 100, 2, MidpointRounding.AwayFromZero) & "%"
        lblCPUUsage.Text = percentCPU
        lblRAMUsage.Text = percentRam

        If conection.conectedVerif Then
            conection.write("#C" & percentCPU & "#R" & percentRam)
        End If

        notifyIcon.BalloonTipText = "CPU: " & lblCPUUsage.Text & "  RAM: " & lblRAMUsage.Text
        notifyIcon.BalloonTipTitle = "DELTA MONITOR"







    End Sub

    Private Sub canvasTop_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles canvasTop.MouseLeftButtonDown
        Me.DragMove()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Hide()

    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As RoutedEventArgs) Handles btnMinimize.Click
        Me.WindowState = WindowState.Minimized
    End Sub
End Class
