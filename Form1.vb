Imports System.IO
Imports System.IO.Compression
Imports System.IO.Packaging
Imports Ionic
Imports Ionic.Zip
Imports VB = Microsoft.VisualBasic
Imports System.Runtime.InteropServices





Public Class Form1

    Public Class DialogHandler
        'API CONSTANTS
        Const WM_GETTEXT As Long = &HD
        Const WM_GETTEXTLENGTH As Long = &HE
        Const GW_ENABLEDPOPUP As Long = 6
        Const BM_CLICK As Long = &HF5&
        Const GW_CHILD As Long = 5
        Const GW_HWNDNEXT As Long = 2

        'FINDS CHILD WINDOWS
        Private Declare Auto Function GetWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal uCmd As Integer) As IntPtr

        'SEND MESSAGES TO THE BUTTON
        Private Declare Auto Function SendMessage Lib "user32.dll" Alias "SendMessage" (ByVal hWnd As IntPtr, ByVal Msg As Integer,
    ByVal wParam As Integer, ByRef lParam As IntPtr) As IntPtr

        'GETS WINDOW TEXT
        Private Declare Auto Function SendMessageA Lib "user32.dll" Alias "SendMessageA" (ByVal hWnd As IntPtr, ByVal Msg As Integer,
    ByVal wParam As IntPtr, ByRef lParam As IntPtr) As IntPtr
        <DllImport("User32.dll", CharSet:=CharSet.Auto, EntryPoint:="SendMessage")> Public Shared Function SendMessageString(ByVal hwnd As IntPtr,
    ByVal wMsg As Integer, ByVal wparam As Integer, ByVal lparam As System.Text.StringBuilder) As IntPtr
        End Function

        Private Function GetChildWindowHandles(ByVal ParentWindowHandle As IntPtr) As ArrayList

            Dim ptrChild As IntPtr
            Dim clsRet As New ArrayList

            'GET FIRST CHILD HANDLE
            ptrChild = GetChildWindowHandle(ParentWindowHandle)

            Do Until ptrChild.Equals(IntPtr.Zero)
                'ADD TO COLLECTION OF HANDLES
                clsRet.Add(ptrChild)
                'GET NEXT CHILD
                ptrChild = GetNextWindowHandle(ptrChild)
            Loop

            Return clsRet

        End Function

        Private Function GetChildWindowHandle(ByVal ParentWindowHandle As IntPtr) As IntPtr
            Return GetWindow(ParentWindowHandle, GW_CHILD)
        End Function

        Private Function GetNextWindowHandle(ByVal CurrentWindowhandle As IntPtr) As IntPtr
            Return GetWindow(CurrentWindowhandle, GW_HWNDNEXT)
        End Function

        'RETURNS TEXT OF THE WINDOW FOR CONFIRMATION OF CORRECT DIALOG
        Private Function GetWindowText(ByVal WindowHandle As IntPtr) As String

            Dim ptrRet As IntPtr
            Dim ptrLength As IntPtr

            'LENGTH OF BUFFER
            ptrLength = SendMessageA(WindowHandle, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero)

            'BUFFER NEEDED FOR RETURN VALUE
            Dim sb As New System.Text.StringBuilder(ptrLength.ToInt32 + 1)

            'WINDOW TEXT
            ptrRet = SendMessageString(WindowHandle, WM_GETTEXT, ptrLength.ToInt32 + 1, sb)

            Return sb.ToString

        End Function

        'SEND A 'CLICK' TO THE BUTTON ("WINDOW")
        Private Sub PerformClick(ByVal WindowHandle As IntPtr)
            SendMessage(WindowHandle, BM_CLICK, 0, IntPtr.Zero)
        End Sub

        Public Sub LookForAndCloseIEPopup(ByVal whichButton As String)

            'GET HANDLE OF ANY POPUP WINDOW ASSOCIATED WITH MAIN FORM
            Dim ptrDialogWindow As IntPtr = GetWindow(Process.GetCurrentProcess.MainWindowHandle, GW_ENABLEDPOPUP)

            'IF IT'S A BROWSER POPUP, HANDLE IT
            If GetWindowText(ptrDialogWindow) = "Microsoft Internet Explorer" Or GetWindowText(ptrDialogWindow) = "Message from webpage" Or GetWindowText(ptrDialogWindow) = "Windows Internet Explorer" Then
                ClosePopup(ptrDialogWindow, whichButton)
            End If

        End Sub

        Private Sub ClosePopup(ByVal WindowHandle As IntPtr, ByVal whichButton As String)

            Dim clsChildHandles As ArrayList = GetChildWindowHandles(WindowHandle)

            For Each ptrHandle As IntPtr In clsChildHandles
                'IF IT FINDS A BUTTON WITH THE TEXT SPECIFIED, CLICK IT
                If GetWindowText(ptrHandle).Contains(whichButton) Then PerformClick(ptrHandle) : Exit For
            Next

        End Sub
    End Class



    ''' <summary>
    '''  Written By Glen Benson may 26th 2016
    '''  Logs on to mycccportal.com  and downloads EMS files
    '''  
    '''
    ''' 
    ''' Connects to external site to get acct no
    ''' retvaltxt = retval.account + "|" + retval.password + "|" + retval.description + "|" + retval.row;
    ''' 
    ''' </summary>

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load





        havefiles = False
        proc_files = False

        app_path = Directory.GetCurrentDirectory



        WebBrowser1.Navigate(googleurl)

    End Sub
    Function wait()
        Try
            While WebBrowser1.ReadyState <> WebBrowserReadyState.Complete
                Application.DoEvents()
            End While
            For i = 1 To 1000
                Application.DoEvents()
            Next

        Catch ex As Exception
            writetolog("In Function wait browser not set")
        End Try

    End Function



    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        Me.Show()

        If (e.Url.AbsolutePath = Me.WebBrowser1.Url.AbsolutePath) Then

            Debug.Print(e.Url.ToString)
            Dim x As String
            x = e.Url.ToString

            Dim logedin As Boolean = False
            Select Case True

                Case x.Contains("google")
                    '-------------------------------------------------------------------------
                    '--  STEP 1 get the google info to process
                    '-------------------------------------------------------------------------
                    Dim i As Integer
                    Dim aryTextFile() As String
                    Dim pagetext As String = WebBrowser1.Document.Body.InnerHtml
                    pagetext = pagetext.Replace("<PRE>", "").Replace("</PRE>", "")
                    aryTextFile = pagetext.Split("|")
                    If aryTextFile.Length <> 3 Then
                        writetolog("value fields from google malformed (not 3 long)")
                        Exit Sub
                    End If
                    acctno = aryTextFile(0)
                    password = aryTextFile(1)
                    row = aryTextFile(2)

                    If acctno = 0 Then
                        'go get the zip file
                        acctno = "34138" 'change to real acctno
                        password = "dakabi04" 'same here

                        WebBrowser1.Navigate("https://mycccportal.com/")
                    Else

                    End If


                Case x = "https://mycccportal.com/" And Not logedin

                    '-------------------------------------------------------------------------
                    '--  STEP 2 Log In
                    '-------------------------------------------------------------------------
                    'delete any existing files in the download folder
                    If Directory.Exists(downloadpath) Then
                        For Each _file As String In Directory.GetFiles(downloadpath)
                            File.Delete(_file)
                        Next
                    End If

                    '--- Login ---
                    Dim strActNo As String = "av@" & acctno
                    'password and acctno are defined from gpoogle data

                    WebBrowser1.Document.GetElementById("USERNAME").InnerText = strActNo
                    WebBrowser1.Document.GetElementById("PASSWORD").InnerText = password
                    WebBrowser1.Document.GetElementById("login").InvokeMember("Click")

                Case x.Contains("https://mycccportal.com/dlPortalWebApp/")
                    '-------------------------------------------------------------------------
                    '--  STEP 3 current claim folder (first page after login)
                    '-------------------------------------------------------------------------
                    WebBrowser1.Navigate("https://www.mycccportal.com/dlPortalWebApp/portlets/CurrentClaimFolders/currentClaimFolders.jsp")


                Case x = "https://www.mycccportal.com/dlPortalWebApp/portlets/CurrentClaimFolders/currentClaimFolders.jsp"
                    '-------------------------------------------------------------------------
                    '--  STEP 4 MAXIMIZED claim folder Page
                    '-------------------------------------------------------------------------
                    Dim cnt As Integer
                    cnt = 0

                    'see if any new claims exist
                    Dim pagetext As String = WebBrowser1.Document.Body.InnerHtml
                    If pagetext.Contains("fa-star") Then
                        MsgBox("there is a New one")
                        Application.Exit()
                    End If

                    ' click the first claim
                    For Each oLink In WebBrowser1.Document.Links
                        If oLink.InnerText IsNot Nothing Then
                            If cnt = 0 Then
                                oLink.InvokeMember("click")
                                claim_no = oLink.InnerText.ToString()
                                Form2.lblClaimNo.Text = claim_no
                                finalfilename = Trim(acctno) & "~" & Trim(claim_no)
                            End If
                            'ListBox1.Items.Add(oLink.InnerText.ToString())
                            cnt = cnt + 1
                        End If
                    Next

                Case x.Contains("ApplicationServlet")
                    '-------------------------------------------------------------------------
                    '--  STEP 5 GO TO MANAGE Tab
                    '-------------------------------------------------------------------------
                    WebBrowser1.Navigate("https://www.mycccportal.com/r/Manage.do")


                Case x = "https://www.mycccportal.com/r/Manage.do"
                    '-------------------------------------------------------------------------
                    '--  STEP 6 load the download box in the current window
                    '-------------------------------------------------------------------------
                    WebBrowser1.Navigate("https://www.mycccportal.com/r/AssignDownload.do?hdn_step=1&source=claimfolder")


                Case x = "https://www.mycccportal.com/r/AssignDownload.do?hdn_step=1&source=claimfolder"
                    '-------------------------------------------------------------------------
                    '--  STEP 7 
                    '-- download dialog is loaded now
                    '-- there is a java element that we need to manually wait for
                    '-------------------------------------------------------------------------

                    proc_files = True
                    ' sitting  on the java download page and have to do some tricky stuff
                    ' download happens next page Step 8
                    If proc_files = True Then

                        SendKeys.Send("{ENTER}") 'this is needed to make the java downloader work 
                        waitseconds(2)           'this is needed to make the java downloader work 
                        SendKeys.Send("{ENTER}") 'this is needed to make the java downloader work 

                        click_download()         'click the download button on the web page
                        Dim dh As New DialogHandler
                        dh.LookForAndCloseIEPopup("OK")
                        proc_files = False
                        javatimer.Enabled = True
                        
                    End If

                Case Me.WebBrowser1.Url.AbsolutePath = "/r/RedirectApplet.do"
                    '-------------------------------------------------------------------------
                    '--  STEP 8
                    ' -- the files should be there now so zip them up and delete after
                    '-------------------------------------------------------------------------
                    ' javatimer.Enabled = False 'shut the timer off because we made it here

                    zipandSendFiles()

                    waitseconds(1)
                    '- We have the zip file so delete the dbf files
                    If Directory.Exists(downloadpath) Then
                        For Each _file As String In Directory.GetFiles(downloadpath)
                            If _file.Contains(acctno) Then
                            Else
                                File.Delete(_file)
                            End If
                        Next
                    End If
                    WebBrowser1.Refresh(WebBrowserRefreshOption.Completely)
                    WebBrowser1.Stop()

            End Select

        End If ' urls match

    End Sub

    Public Sub getzipfile()


    End Sub

    Public Sub waitseconds(ByVal seconds As Single)
        Static start As Single
        start = VB.Timer()
        Do While VB.Timer() < start + seconds

            Application.DoEvents()
        Loop
    End Sub


    Private Sub FILEToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs)
        getzipfile()

    End Sub

    Private Sub renameFilesInFolderChronologically()
        Dim sourcePath As String = "c:\ADP\EMSIN"
        Dim searchPattern As String = "*.*"

        Dim curDir As New DirectoryInfo(sourcePath)

        Dim i As Integer = 0
        For Each fi As FileInfo In curDir.GetFiles(searchPattern).OrderBy(Function(num) num.CreationTime)
            File.Move(fi.FullName, Path.Combine(fi.Directory.FullName, "txtFile_" & i & ".txt"))
            'MsgBox(fi.FullName)
            i += 1
        Next
    End Sub






    Private Sub MenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)

    End Sub


    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

    Sub zipandSendFiles()
        'MsgBox(finalfilename)
        Dim safetozip As Boolean = False
        waitseconds(1)

        Using zip As ZipFile = New ZipFile()

            If Directory.Exists(downloadpath) Then
                For Each _file As String In Directory.GetFiles(downloadpath)
                    zip.AddFile(_file, "")
                Next
            End If

            zip.Save(downloadpath & "\" & finalfilename & ".zip")

        End Using
    End Sub





    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles main_loop_timer.Tick


        'finalfilename
        'If My.Computer.FileSystem.FileExists(downloadpath & "\" & finalfilename & ".zip") Then
        '    writetolog(finalfilename & ".zip was created")
        'Else
        '    writetolog("ERROR ->" & finalfilename & ".zip was NOT created")
        'End If
    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles javatimer.Tick


        If Me.WebBrowser1.Url.AbsolutePath = "/r/RedirectApplet.do" Then
            'succesful so the zip should be there
            attempt = 0
            writetolog("Successful ->" & attempt.ToString)

            Me.Close()

        Else
            Dim dh As New DialogHandler
            dh.LookForAndCloseIEPopup("OK")
            attempt = attempt + 1
            If attempt >= 3 Then
                writetolog("UNABLE to Download ZIP timed out after 3 attempts")


                Me.Close()




            Else
                click_download()
            End If

        End If

        ''see if the zip file is there
        ''finalfilename
        'If My.Computer.FileSystem.FileExists(downloadpath & "\" & finalfilename & ".zip") Then
        '    writetolog(finalfilename & ".zip was created")
        'Else
        '    writetolog("ERROR ->" & finalfilename & ".zip was NOT created")
        'End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)


    End Sub


    Sub click_download()
        Try
            WebBrowser1.Document.Window.Frames("RFPopupButtons").Document.GetElementById("download").InvokeMember("click")
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs)

    End Sub







End Class
