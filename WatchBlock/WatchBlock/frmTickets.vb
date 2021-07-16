Imports System.Data.SqlClient

Public Class frmTickets

    Private Database1DataSet As DataSet
    Private Sub btnConsultas_Click(sender As Object, e As EventArgs)
        Consulta()
    End Sub

    Private Sub Consulta()

        Using conexion = ConectorBase.CreateConnection

            conexion.Open()

            Dim ConsultaTableAdapter As New SqlDataAdapter

            ConsultaTableAdapter = New SqlDataAdapter With {
                .SelectCommand = New SqlCommand(
                    "SELECT  ALIAS_0.NOMBRE Ticket, 
                    ALIAS_1.NOMBRE Solicitante, ALIAS_3.NOMBRE Prioridad, 
                    ALIAS_4.DESCRIPCION Flag, ALIAS_0.DETALLE Detalle,
                    ALIAS_6.FECHAPROMETIDA ALIAS_6_FECHAPROMETIDA
                    FROM   V_TRORDENVENTA_ ALIAS_0  
                    LEFT OUTER JOIN V_UD_TICKETSOPORTE_ ALIAS_6 ON ALIAS_0.BOEXTENSION_ID = ALIAS_6.ID   
                    LEFT OUTER JOIN V_EMPLEADO_ ALIAS_7 ON ALIAS_6.SOLICITANTE_ID = ALIAS_7.ID 
                    LEFT OUTER JOIN V_PERSONA_ ALIAS_1 ON ALIAS_7.ENTEASOCIADO_ID = ALIAS_1.ID   
                    LEFT OUTER JOIN V_ITEMTIPOCLASIFICADOR_ ALIAS_3 ON ALIAS_6.PRIORIDAD_ID = ALIAS_3.ID  
                    LEFT OUTER JOIN V_FLAG_ ALIAS_4 ON ALIAS_0.FLAG_ID = ALIAS_4.ID
                    WHERE ALIAS_0.BO_PLACE_ID = '{EE1B5C36-774B-40A2-8CAD-6F4505295117}'   
                    AND  ALIAS_0.TIPOTRANSACCION_ID = '{FBE616F7-BE93-4823-A9E9-B530F23FEE63}' 
                    AND   ( ALIAS_0.UNIDADOPERATIVA_ID = '{7D3319C0-F01D-4D1B-AC50-F00F26CB8A8A}' )   
                    AND  ALIAS_0.ESTADO = 'A' 
                    AND  MAILTECNICO LIKE '%" + Sesion.Usuario + "@%'  
                    ORDER BY ALIAS_0.FECHAACTUAL DESC, ALIAS_0.NUMERODOCUMENTO DESC
", conexion)
            }

            Database1DataSet = New DataSet

            Database1DataSet.Tables.Add("ConsultaCalipso")

            ConsultaTableAdapter.Fill(Database1DataSet.Tables("ConsultaCalipso"))

            dgvTickets.DataSource = Database1DataSet.Tables("ConsultaCalipso")

            conexion.Close()

        End Using


    End Sub

    Private Sub frmTickets_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Consulta()
    End Sub

    Private Sub dgvTickets_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTickets.RowEnter
        lblTicket.Text = Me.dgvTickets.Rows(e.RowIndex).Cells(0).Value

        lblSolicitante.Text = Me.dgvTickets.Rows(e.RowIndex).Cells(1).Value

        Try
            lblPrioridad.Text = Me.dgvTickets.Rows(e.RowIndex).Cells(2).Value

        Catch ex As Exception

            lblPrioridad.Text = "SIN ESPECIFICAR"

        End Try

        lblFlag.Text = Me.dgvTickets.Rows(e.RowIndex).Cells(3).Value

        txtDetalle.Text = Me.dgvTickets.Rows(e.RowIndex).Cells(4).Value
    End Sub
End Class