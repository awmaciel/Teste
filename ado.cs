using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Domain.Json;
using System.Data;

namespace Infra
{
    public class ADO
    {
        SqlConnection sqlCon = new SqlConnection("Data Source = DESKTOP-M2JEDS0; " + "Initial Catalog=teste;" + "User id=sa;" + "Password=Alxndr@12;");
        public void insert(Domain.Json.Documento doc)
        {            
            try
            {
                
                sqlCon.Open();
                string sqlCmd = "INSERT INTO Documento(ID,TipoDocumento,LocalDeGuarda,NumeroCaixa) values(@ID,@TipoDocumento,@LocalDeGuarda,@NumeroCaixa)";
                SqlCommand sc = new SqlCommand(sqlCmd, sqlCon);
                sc.Parameters.AddWithValue("@ID", doc.ID);
                sc.Parameters.AddWithValue("@TipoDocumento", doc.TipoDocumento);
                sc.Parameters.AddWithValue("@LocalDeGuarda", doc.LocalDeGuarda);
                sc.Parameters.AddWithValue("@NumeroCaixa", doc.NumeroCaixa);
                sc.ExecuteNonQuery();
                sc.Dispose();
                sqlCon.Close();
                //MessageBox.Show("Imagem Salva ");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erro :: " + ex.Message);
            }

        }
        public bool ValidaDocumento(string id)
        {
            bool ret = true;
            try
            {

                sqlCon.Open();
                string sqlCmd = "SELECT ID FROM DOCUMENTO WHERE ID = @ID";
                SqlCommand sc = new SqlCommand(sqlCmd, sqlCon);
                sc.Parameters.AddWithValue("@ID", id);
                var reader = sc.ExecuteReader();

                if (reader.HasRows)
                    ret = false;

                sc.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erro :: " + ex.Message);
            }
            return ret;
        }
        public List<Documento> Getall()
        {
            DataTable dataTable = new DataTable();
            List<Documento> lstdoc = new List<Documento>();
            try
            {
                
                sqlCon.Open();
                SqlCommand cmd = new SqlCommand("SELECT *FROM DOCUMENTO",sqlCon);                
                var reader = cmd.ExecuteReader();               
                
                while (reader.Read())
                {
                    Documento doc = new Documento()
                    {
                        ID = reader["ID"].ToString(),
                        LocalDeGuarda = reader["LocalDeGuarda"].ToString(),
                        TipoDocumento = reader["TipoDocumento"].ToString(),
                        NumeroCaixa =  reader["NumeroCaixa"].ToString()
                    };
                    lstdoc.Add(doc);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erro :: " + ex.Message);
            }
            sqlCon.Close();
            return lstdoc;
        }
        public void Delete(Documento doc)
        {
            try
            {

                sqlCon.Open();
                string sqlCmd = "DELETE FROM DOCUMENTO WHERE ID = @ID";
                SqlCommand sc = new SqlCommand(sqlCmd, sqlCon);
                sc.Parameters.AddWithValue("@ID", doc.ID);
                sc.ExecuteNonQuery();
                sc.Dispose();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erro :: " + ex.Message);
            }
        }
    }
}
