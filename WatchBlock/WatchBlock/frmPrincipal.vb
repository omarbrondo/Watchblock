Imports System.IO
Imports System.Data.SqlClient
Public Class frmPrincipal

    Private Database1DataSet As DataSet

    Dim RutaCSV As String = "C:\Watchblock\fichero.csv"

    Public Property Database1DataSet1 As DataSet
        Get
            Return Database1DataSet
        End Get
        Set(value As DataSet)
            Database1DataSet = value
        End Set
    End Property

    Dim Servidor As String 'variable global para guardar el nombre del servidor a usar

    Private Sub btnCerrarProceso_Click(sender As Object, e As EventArgs) Handles btnCerrarProceso.Click

        Dim Respuesta As DialogResult

        If lblUsuarioBloqueando.Text = "NINGUNO" And lblServer.Text = "NINGUNO" And lblPID.Text = "NINGUNO" Then

            MessageBox.Show("NO HAY USUARIOS BLOQUEANDO", "ATENCION", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        Else

            Respuesta = MessageBox.Show("DESEA CERRAR LA SESION DEL USUARIO " + lblUsuarioBloqueando.Text + " DEL TERMINAL " + lblServer.Text + "?", "CONFIRMAR CIERRE", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

            If Respuesta = DialogResult.Yes Then

                Dim comandoTaskill As String

                comandoTaskill = " /s " + lblIPServer.Text + " /u bosisiosa\" + Sesion.Usuario + " /p " + Sesion.Password + " /pid " + lblPID.Text

                Dim strArgumentos = comandoTaskill

                Dim strExe As String = "taskkill"

                Dim startInfo As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo(strExe, strArgumentos)

                startInfo.UseShellExecute = False

                startInfo.ErrorDialog = False

                startInfo.CreateNoWindow = True

                startInfo.RedirectStandardOutput = True

                Dim p As Diagnostics.Process = System.Diagnostics.Process.Start(startInfo)


                MessageBox.Show("SE CERRO EL PID " + lblPID.Text + " DEL SERVER " + Servidor, "FINALIZADO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                'System.IO.File.Delete(ruta)

                lblPID.Text = "NINGUNO"

                Grabar()

                ConsultarBloqueo()

            Else

                btnCerrarProceso.Focus()

            End If

        End If

    End Sub
    Private Sub btnBuscarUsuario_Click(sender As Object, e As EventArgs)

        ComandoTasklist()

    End Sub

    Private Sub Blanqueo()


        DataGridView1.Columns.Clear()

        lblServer.Text = "NINGUNO"

        lblPID.Text = "NINGUNO"

        lblUsuarioBloqueando.Text = "NINGUNO"

        lblEstado.Text = "EN ESPERA"

        lblIPServer.Visible = False

    End Sub


    Private Sub Grabar()

        Using conexion = ConectorBase.CreateConnection()

            conexion.Open()

            Dim RegistrosTableAdapter = New SqlDataAdapter()

            RegistrosTableAdapter.InsertCommand = New SqlCommand With {
                .CommandText = "INSERT INTO dbo.WS_SISTEMAS_CALIPSO_LOG (userlog, hostlog) VALUES (@userlog, @hostlog)",
                .CommandTimeout = 3600,
                .Connection = conexion
            }

            RegistrosTableAdapter.InsertCommand.Parameters.Add("@userlog", SqlDbType.VarChar, 50).Value = lblUsuarioBloqueando.Text

            RegistrosTableAdapter.InsertCommand.Parameters.Add("@hostlog", SqlDbType.VarChar, 150).Value = lblServer.Text

            RegistrosTableAdapter.InsertCommand.ExecuteNonQuery()

            conexion.Close()


        End Using

    End Sub

    Private Sub ComandoTasklist()

        'CapturarServidor()

        Dim comandoTasklist As String

        comandoTasklist = " /s " + lblIPServer.Text + " /u bosisiosa.com\" + Sesion.Usuario + " /p " + Sesion.Password + " /v /FI ""IMAGENAME eq CExplorer.exe"" /FI ""PID eq " + lblPID.Text + """ /FO csv"

        Dim strArgumentos = comandoTasklist

        Dim strExe As String = "tasklist"

        Dim startInfo As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo(strExe, strArgumentos)

        ProgressBar1.Value = 6

        lblEstado.Text = "EJECUTANDO TASKLIST"

        startInfo.UseShellExecute = False

        startInfo.ErrorDialog = False

        startInfo.CreateNoWindow = True

        startInfo.RedirectStandardOutput = True

        Dim p As Diagnostics.Process = System.Diagnostics.Process.Start(startInfo)

        ProgressBar1.Value = 7

        lblEstado.Text = "CREANDO INFORME DE TASKLIST"

        Try
            'Dim p As Diagnostics.Process = System.Diagnostics.Process.Start(startInfo)
            Dim sr As System.IO.StreamReader = p.StandardOutput
            Dim cadenaSalida As String = sr.ReadToEnd()
            sr.Close()

            'Dim ruta As String = "C:\Watchblock\fichero.csv"

            Dim escritor As StreamWriter

            escritor = File.CreateText(RutaCSV)

            escritor.Write(cadenaSalida)

            escritor.Flush()

            escritor.Close()

            ProgressBar1.Value = 8

            lblEstado.Text = "IMPORTANDO GRILLA DE USUARIOS"

            DataGridView1.Columns.Clear() 'LIMPIA DE RESULTADOS ANTERIORES


            'CABECERA
            Dim CABECERA As String = IO.File.ReadLines(RutaCSV)(0) 'PRIMERA LINEA DEL ARCHIVO COMO CABECERA
            Dim COLUMNAS As String() = CABECERA.Split(",")
            DataGridView1.ColumnCount = COLUMNAS.Count
            For I = 0 To COLUMNAS.Count - 1
                DataGridView1.Columns(I).Name = COLUMNAS(I)
            Next

            'RESTO DE FILAS
            For I = 1 To IO.File.ReadLines(RutaCSV).Count - 1
                Dim FILA As String() = IO.File.ReadLines(RutaCSV)(I).Split(",")
                DataGridView1.Rows.Add(FILA)
            Next

            ProgressBar1.Value = 10

            lblEstado.Text = "FINALIZADO"

        Catch ex As Exception

        End Try

        lblUsuarioBloqueando.Text = Me.DataGridView1.Rows(0).Cells(5).Value


    End Sub

    Private Sub ConsultarBloqueo()

        Blanqueo()

        Using conexion = ConectorBase.CreateConnection

            conexion.Open()

            Dim ConsultaTableAdapter As New SqlDataAdapter

            ProgressBar1.Value = 1

            ConsultaTableAdapter = New SqlDataAdapter With {
                .SelectCommand = New SqlCommand("SELECT TOP 1 sysSession.session_id, sysSession.host_name, sysSession.host_process_id, sysBlocking.wait_duration_ms
                                                FROM sys.dm_exec_sessions sysSession
                                                JOIN sys.dm_os_waiting_tasks sysBlocking on sysBlocking.blocking_session_id = sysSession.session_id 
                                                AND sysBlocking.blocking_session_id <> 0
                                                AND sysBlocking.wait_type = 'LCK_M_S'
                                                ORDER BY sysBlocking.wait_duration_ms desc", conexion)
            }

            lblEstado.Text = "EJECUTANDO QUERY"

            ProgressBar1.Value = 2



            Database1DataSet = New DataSet

            lblEstado.Text = "LLENANDO GRILLA DE DATOS"

            ProgressBar1.Value = 3

            Database1DataSet.Tables.Add("ConsultaCalipso")

            ProgressBar1.Value = 4

            ConsultaTableAdapter.Fill(Database1DataSet.Tables("ConsultaCalipso"))

            dgv1.DataSource = Database1DataSet.Tables("ConsultaCalipso")

            lblEstado.Text = "FIN DE LA CONSULTA QUERY"

            ProgressBar1.Value = 5

            conexion.Close()

        End Using

        Try
            lblPID.Text = Me.dgv1.Rows(0).Cells(2).Value

            lblServer.Text = Me.dgv1.Rows(0).Cells(1).Value

            Servidor = lblServer.Text

            CapturarServidor()

            ComandoTasklist()

        Catch ex As Exception

            ProgressBar1.Value = 10

            Blanqueo()

        End Try

    End Sub
    Private Sub btnConsultar_Click(sender As Object, e As EventArgs) Handles btnConsultar.Click

        ConsultarBloqueo()

    End Sub

    Private Sub CapturarServidor()


        Select Case lblServer.Text

            Case "CALIPSO-TSADM"

                lblIPServer.Visible = True

                lblIPServer.Text = "\\172.16.0.209"

            Case "CALIPSO-TSPROD"

                lblIPServer.Visible = True

                lblIPServer.Text = "\\172.16.0.208"

            Case "CALIPSO-TSFAC9"

                lblIPServer.Visible = True

                lblIPServer.Text = "\\172.16.0.213"

            Case "CALIPSO-TSPREP"

                lblIPServer.Visible = True

                lblIPServer.Text = "\\172.16.0.215"

            Case "CALIPSO-TSSOP"

                lblIPServer.Visible = True

                lblIPServer.Text = "\\172.16.0.212"

            Case Else

                lblIPServer.Visible = False

        End Select

    End Sub

    Private Sub frmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ProgressBar1.Minimum = 0

        ProgressBar1.Maximum = 10

        ProgressBar1.Value = 0


        lblVersionSistema.Text = My.Application.Info.Version.ToString


        btnConsultar.Focus()

    End Sub



    Private Sub TicketsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TicketsToolStripMenuItem.Click

        frmTickets.Show()

    End Sub

    Private Sub ORDENESDETRABNOGENERADASToolStripMenuItem_Click(sender As Object, e As EventArgs)
        frmOrdTrabGen.Show()
        Me.Close()
    End Sub

    Private Sub CierresHistoricosDeCalipsoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CierresHistoricosDeCalipsoToolStripMenuItem.Click
        frmConsultas.Show()
    End Sub

    Private Sub OrdenesDeTrabajoGeneradasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OrdenesDeTrabajoGeneradasToolStripMenuItem.Click
        frmOrdTrabGen.Show()
        Me.Close()
    End Sub
End Class
