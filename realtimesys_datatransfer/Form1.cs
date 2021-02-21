using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace realtimesys_datatransfer
{
    public partial class Form1 : Form
    {
        private string data;
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
           
            listView1.View = View.List;
            listView1.FullRowSelect = true;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
                comboBox1.Items.Add(port);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);


        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data = serialPort1.ReadLine();
            this.Invoke(new EventHandler(displayData_event));
        }

        private void displayData_event(object sender, EventArgs e)

        {
            textBox1.Text += DateTime.Now.ToShortDateString()+ "\t\t" + DateTime.Now.ToLongTimeString()+"\t\t"+ data + "\n"; 
            listView1.Items.Add(data);
        }

        private void baslat_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.Open();
                durdur.Enabled = true;
                baslat.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "HATA");
            }
        }

        private void durdur_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            durdur.Enabled = false;
            baslat.Enabled = true;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }
        private void sifirla_Click(object sender, EventArgs e)
        {
           
            listView1.Clear();
            textBox1.Clear();
           
        }
        private void kaydet_Click(object sender, EventArgs e)
        {
            //txt dosyasına kaydetme işlemi
             try
             {
                 for(int i=0; i < listView1.Items.Count; i++)
                {
                    string path = @"D:\\";
                    string filename = "data.txt";
                    System.IO.File.WriteAllText(path + filename, "Tarih\t\tZaman\t\tSensor\t\tDeger\n"+textBox1.Text);
                }
                 
                 MessageBox.Show("Dosya txt olarak kaydedildi", "Mesaj");
             }
             catch (Exception ex2)
             {
                 MessageBox.Show(ex2.Message, "txt dosyası");
             }   
          //db kayıt
            int etkilenen = 0;

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                try
                {
                    SqlConnection conn = new SqlConnection
                    ("Data Source=uemyodata.database.windows.net;Initial Catalog=realtimeSys;User ID=ueadmin;Password=Merve1234");
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    string sensor_adı = listView1.Items[i].SubItems[0].Text;
                    string sensor_deger = listView1.Items[i].SubItems[0].Text;
                    string tarih = DateTime.Now.ToShortDateString();
                    string saat = DateTime.Now.ToLongTimeString();
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.CommandText = "insert into sensor_bilgi (tarih,saat,sensor_adı,sensor_deger) values (@tarih, @saat,@sensor_adı,@sensor_deger)";

                    cmd.Parameters.AddWithValue("@tarih", tarih);
                    cmd.Parameters.AddWithValue("@saat", saat);
                    cmd.Parameters.AddWithValue("@sensor_adı", sensor_adı);
                    cmd.Parameters.AddWithValue("@sensor_deger", sensor_deger);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    
                    conn.Close();

                }
                catch (Exception hata)
                {
                    MessageBox.Show( hata.Message);
                }
            }
            if (etkilenen != 0) 
            {
                MessageBox.Show("DB kayıt edilmedi");
            }
            else
            {
                MessageBox.Show("DB kayıt edildi");

            }
        }

        
    }
}
