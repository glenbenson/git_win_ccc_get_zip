Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' establish file paths
        txtDownloadPath.Text = My.Settings.download_path

        downloadpath = My.Settings.download_path
        backuppath = My.Settings.download_path & "\sentzips"


        'Make the needed folders if they dont exist
        If (Not System.IO.Directory.Exists(downloadpath)) Then
            System.IO.Directory.CreateDirectory(downloadpath)
        End If

        If (Not System.IO.Directory.Exists(backuppath)) Then
            System.IO.Directory.CreateDirectory(backuppath)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Form1.Show()
    End Sub
    Private Sub Label2_Click(sender As Object, e As EventArgs)
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            txtDownloadPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub txtDownloadPath_TextChanged(sender As Object, e As EventArgs)
        My.Settings.download_path = Trim(txtDownloadPath.Text)
        downloadpath = txtDownloadPath.Text
        My.Settings.Save()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnPause.Click
        If btnPause.Text = "Pause" Then
            btnPause.Text = "Start"
            Timer1.Enabled = False
        Else
            Timer1.Enabled = True
            btnPause.Text = "Pause"
        End If
    End Sub

    Private Sub MenuStrip2_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuStrip2.ItemClicked

    End Sub

    Private Sub FilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilesToolStripMenuItem.Click

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()

    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles lblAcctNo.Click

    End Sub

    Private Sub lblClaimNo_Click(sender As Object, e As EventArgs) Handles lblClaimNo.Click

    End Sub
End Class