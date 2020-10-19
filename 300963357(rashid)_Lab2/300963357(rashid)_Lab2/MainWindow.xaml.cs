using _300963357_rashid__Lab2.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Amazon;
    using Amazon.Runtime;
using System.Data;
using System.Drawing;
using Color = System.Drawing.Color;
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace _300963357_rashid__Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User user;
        public BookShelf bookShelf;
        private string tableName = "Users";
        private string hashKey = "UserEmail";
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private bool userExist = false;


        public MainWindow()
        {
            InitializeComponent();
            user = new User();
            this.BtnLogin.IsEnabled = false;

        }


        private void TxtUserEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            user.UserEmail = TxtUserEmail.Text;
        }

        private void BtnSignup_Click(object sender, RoutedEventArgs e)
        {
            creatingTable();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private async void creatingTable()
        {
            if (string.IsNullOrEmpty(TxtUserEmail.Text) || string.IsNullOrEmpty(TxtPassword.Password))
            {
                MessageBox.Show("Fields can't be empty!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                client = new AmazonDynamoDBClient();
                context = new DynamoDBContext(client);
                List<string> currentTables = client.ListTables().TableNames;


                if (!currentTables.Contains(tableName))
                {
                    await CreateUserTable(client, tableName);
                    await saveUser(context);
                }
                else
                {
                    await saveUser(context);
                }
                BtnLogin.IsEnabled = true;
            }
        }
        public static async Task CreateUserTable(AmazonDynamoDBClient client, string tableName)
        {
            var tableResponse = client.ListTables();
            if (!tableResponse.TableNames.Contains(tableName))
            {
                await Task.Run(() =>
            {

                client.CreateTable(new CreateTableRequest
                {
                    TableName = "Users",
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 },
                    KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName="UserEmail",
                        KeyType=KeyType.HASH
                    }
                },
                    AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition{AttributeName="UserEmail", AttributeType=ScalarAttributeType.S}
                }
                });
                Thread.Sleep(5000);
            });
            }
         
        }
        private async Task saveUser(DynamoDBContext context)
        {
            bool userExisted = userExists();
            user.Password = TxtPassword.Password;
                if(userExist)
            {
                MessageBox.Show("Account exists already!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                await Task.Run(() =>
                {
                    context.Save<User>(user);
                    MessageBox.Show("Account Created Successfully!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
           
        }
        public bool userExists()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast2);
            Table table = Table.LoadTable(client, "Users");
            string email = TxtUserEmail.Text;
            Document doc = table.GetItem(email);
            if(doc == null)
            {
                userExist = false;
            } else
            {
                userExist = true;
            }
            return userExist;
        }

        public void loadData()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast2);
            Table table = Table.LoadTable(client, "Users");
            string email = TxtUserEmail.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Fields can't be empty!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Document doc = table.GetItem(email);
                string emailInput = doc.Values.ElementAt(1);
                string userPasword = doc.Values.ElementAt(0);
                string result = emailInput;
                string pass = userPasword;
                if (email == result & password == pass)
                {

                    bookShelf = new BookShelf();
                    this.bookShelf.UserEmail = emailInput;

                    HomePage homePage = new HomePage(this.bookShelf);
                    homePage.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Incorrect Email or Password entered!");
                }
            }
        }
    }

}

