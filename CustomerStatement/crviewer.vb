Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports CrystalDecisions.ReportSource
Imports CrystalDecisions.Windows.Forms
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports acc = ACCPAC.Advantage



Friend Class crviewer
    Private rdoc As New ReportDocument
    Private conrpt As New ConnectionInfo()
    Dim server As String = ""
    Dim uid As String = ""
    Dim pass As String = ""

    Private ccompid As String
    Private crbfun As Boolean
    Private crbsrc As Boolean
    Private cfdate As String
    Private ctdate As String
    Private cfrmsales As String
    Private ctosales As String
    Private cfrmcust As String
    Private ctocust As String
    Private cwsalm As Boolean
    Private cwosalm As Boolean
    Friend Property ObjectHandle As String
    Friend Function createdes(ByVal key As String) As TripleDES
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim des As TripleDES = New TripleDESCryptoServiceProvider()
        des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key))
        des.IV = New Byte(des.BlockSize \ 8 - 1) {}
        Return des
    End Function
    Friend Function Decryption(ByVal cyphertext As String, ByVal key As String) As String
        Dim b As Byte() = Convert.FromBase64String(cyphertext)
        Dim des As TripleDES = createdes(key)
        Dim ct As ICryptoTransform = des.CreateDecryptor()
        Dim output As Byte() = ct.TransformFinalBlock(b, 0, b.Length)
        Return Encoding.Unicode.GetString(output)
    End Function
    Friend Function Readconnectionstring() As String

        Dim secretkey As String = "Fhghqwjehqwlegtoit123mnk12%&4#"
        Dim path As String = ("txtcon\bascon.txt")
        Dim sr As New StreamReader(path)

        server = sr.ReadLine()
        Dim db As String = sr.ReadLine()
        uid = sr.ReadLine()
        pass = sr.ReadLine()


        server = Decryption(server, secretkey)
        uid = Decryption(uid, secretkey)
        pass = Decryption(pass, secretkey)

        Dim cons As String = "" '"Data Source =(Local); DataBase =" & custstatement.compid & "; User Id =" & uid & "; Password =" & pass & ";"

        Return cons
    End Function
    Public Sub New(ByVal _objectHandle As String, ByVal _sess As acc.Session, ByVal rbfun As Boolean, ByVal rbsrc As Boolean, ByVal fdate As String, ByVal tdate As String, ByVal frmcust As String, ByVal tocust As String)
        InitializeComponent()
        ObjectHandle = _objectHandle
        ccompid = _sess.CompanyID
        crbfun = rbfun
        crbsrc = rbsrc

        cfdate = fdate
        ctdate = tdate
        cfrmcust = frmcust
        ctocust = tocust

    End Sub

    Public Sub New(ByVal _objectHandle As String)
        InitializeComponent()
        ObjectHandle = _objectHandle
    End Sub

    Private Sub crviewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

            Dim cwvr As New CrystalReportViewer
            cwvr.Dock = DockStyle.Fill
            cwvr.BorderStyle = BorderStyle.None
            cwvr.ExportReport()
            Me.Controls.Add(cwvr)



            If crbfun = True Then
                rdoc.Load("reports\ARCUSTHOMERPT.rpt")
            ElseIf crbsrc = True Then
                rdoc.Load("reports\ARCUSTSURCRPT.rpt")
            End If

            Dim tabs As Tables = rdoc.Database.Tables
            Dim parv As New ParameterValues
            Dim dis As New ParameterDiscreteValue








            Readconnectionstring()


            For Each TAB As CrystalDecisions.CrystalReports.Engine.Table In tabs
                Dim tablog As TableLogOnInfo = TAB.LogOnInfo
                conrpt.ServerName = server
                conrpt.DatabaseName = ccompid
                conrpt.UserID = uid
                conrpt.Password = pass
                tablog.ConnectionInfo = conrpt
                TAB.ApplyLogOnInfo(tablog)
            Next




            rdoc.SetParameterValue("frmyearper", cfdate)
            rdoc.SetParameterValue("toyearper", ctdate)
            rdoc.SetParameterValue("Frmcus", cfrmcust)
            rdoc.SetParameterValue("TOcus", ctocust)




            cwvr.ReportSource = rdoc


        Catch ex As Exception
            MessageBox.Show(ex.Message)
            If rdoc Is Nothing Then
                rdoc.Close()
                rdoc.Dispose()
            End If
        End Try
    End Sub


End Class



