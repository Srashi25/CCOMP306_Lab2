using _300963357_rashid__Lab2.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _300963357_rashid__Lab2
{
    /// <summary>
    /// Interaction logic for AddSnapshot.xaml
    /// </summary>
    public partial class AddSnapshot : Window
    {
        BookShelf bookShelf;
        Snapshot snapshot;
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        public AddSnapshot(BookShelf book)
        {
            InitializeComponent();
            snapshot = new Snapshot();
            bookShelf = book;
            context = new DynamoDBContext();
            TxtBookTitle.IsEnabled = false;
            TxtBookTitle.Text = book.BookTitle;
        }

        private void BtnSaveSnapShot_Click(object sender, RoutedEventArgs e)
        {
            string bucketName = "Snapshot";
            string hashKey = "SnapshotId";
           CreateTable(bucketName, hashKey);
            this.Close();
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        private void CreateTable(string tabNam, string hashKey)
        {
            client = new AmazonDynamoDBClient();
            var tableResponse = client.ListTables();
            if (!tableResponse.TableNames.Contains(tabNam))
            {

                MessageBox.Show("Table doesn't exist, creating table " + tabNam);
                client.CreateTable(new CreateTableRequest
                {
                    TableName = tabNam,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = hashKey,
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = hashKey, AttributeType=ScalarAttributeType.S }
                    }
                });

                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    Console.WriteLine("Waiting for table to be active...");
                    Thread.Sleep(5000);
                    var tableStatus = client.DescribeTable(tabNam);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
                MessageBox.Show("DynamoDB Table Created Successfully!");
                SaveSnapShot();
           
            }
            else
            {
                MessageBox.Show($"{tabNam} exist already!, Adding SNAPSHOT");
                SaveSnapShot();
     
            }
        }
        public void SaveSnapShot()
        {
            //Set a local DB context
            context = new DynamoDBContext(client);
            //Create an BookShelf object to save
            string pageNo = txtPageNo.Text;
            if(string.IsNullOrEmpty(pageNo))
            {
                MessageBox.Show("Page no is miising!");
            }else
            {
                snapshot.BookTitle =bookShelf.BookTitle;
                snapshot.Email = bookShelf.UserEmail;
                snapshot.SnapshotId = Guid.NewGuid().ToString();
                snapshot.DateTime = DateTime.Now;
                snapshot.LastPage = Convert.ToInt32(txtPageNo.Text);
                //Save an Book object
                context.Save<Snapshot>(snapshot);
                MessageBox.Show("Snapshot added Successfully!");
            }
            
        }


    }
}
