using MachineLearn.XmlType;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace MachineLearn.Views
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var soap = new Soap<GetUserResponse>()
            {
                Envesoap = "string",
                body = new GetUserResponse()
                {
                    Name = "string",
                    Email = "string",
                }
            };
            string path = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "xml789.json");

            var serializer = new XmlSerializer(typeof(Soap<GetUserResponse>));
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            using (XmlWriter writer = XmlWriter.Create(fs))
            {
                serializer.Serialize(writer, soap);
            }
            var xmldocument = new XmlDocument();
            xmldocument.Load(path);
            foreach(XmlNode i in xmldocument.SelectNodes("name"))
            {
                File.WriteAllText(path, i.InnerText);
            }
        }
    }
}
