using Domain.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace Presenter.Json
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public delegate void Excluir(object sender, System.Windows.Input.KeyEventArgs e);

    public partial class MainWindow : Window
    {
        Infra.ADO ado = new Infra.ADO();

        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<Documento> observer = new ObservableCollection<Documento>(ado.Getall() as List<Documento>);
            MegaScannerGrid.ItemsSource = observer;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            Pac doc = new Pac();

            try
            {
                var fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                var dadosJson = new byte[fs.Length];
                fs.Read(dadosJson, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();

                var arquivo = dlg.FileName.ToString();
                var json = System.IO.File.ReadAllText(arquivo);

                dynamic dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                foreach (var item in DeserializarNewtonsoft(arquivo))
                {
                    Pac b = new Pac();
                    switch (item.TipoDocumento.Value)
                    {
                        case "Extratos":
                            //documento.insert(Newtonsoft.Json.JsonConvert.DeserializeObject<List<Extrato>>(json));                            
                            break;
                        case "PAC":
                            var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Pac>(item.ToString());
                            doc.insert(ret);
                            break;
                    };
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro :: " + ex.Message);
            }
            MegaScannerGrid.ItemsSource = new ObservableCollection<Documento>(ado.Getall() as List<Documento>);

        }

        private static IEnumerable<dynamic> DeserializarNewtonsoft(string arquivo)
        {
            var json = System.IO.File.ReadAllText(arquivo);
            dynamic dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            int cont = 0;
            foreach (var item in dynJson)
            {
                JArray jsonArray = JArray.Parse(json);
                dynamic data = JObject.Parse(jsonArray[cont].ToString());
                cont = cont + 1;
                yield return data;
            }
        }

        private static void SerializarNewtonsoft(List<Documento> doc, string arquivo)
        {
            using (var streamWriter = new System.IO.StreamWriter(arquivo))
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
                streamWriter.Write(json);
            }
        }

        private static List<Documento> DeserializarDataContractJsonSerializer(string arquivo)
        {
            using (var stream = new System.IO.FileStream(arquivo, System.IO.FileMode.Open))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<Documento>));
                return (List<Documento>)serializer.ReadObject(stream);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (MegaScannerGrid.SelectedItem != null)
            {
                Delete(MegaScannerGrid.SelectedItem as Documento);
                //    DataGridteste wm = new DataGridteste();
                //    wm.Show();
            }
        }

        private void MegaScannerGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            Documento doc = new Documento();
            Documento documento = e.Row.DataContext as Documento;

            if (documento != null)
            {
                if (documento.ID != 0)
                {
                    Delete(documento);
                }

            }
        }

        private void Delete(Documento doc)
        {
            var result = System.Windows.MessageBox.Show("Vocë deseja excluir esse documento?", "Deletar documento", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                ado.Delete(doc);
                System.Windows.MessageBox.Show("Documento excluido com sucesso.", "Documento Excluido", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            MegaScannerGrid.ItemsSource = new ObservableCollection<Documento>(ado.Getall() as List<Documento>);
        }

        private void ChangeText(object sender, RoutedEventArgs e)
        {
            Documento doc = (sender as System.Windows.Controls.Button).DataContext as Documento;
            DataGridteste wm = new DataGridteste();
            wm.Show();
        }
    }
}
