using _300963357_rashid__Lab2.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
using Table = Amazon.DynamoDBv2.DocumentModel.Table;

namespace _300963357_rashid__Lab2
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        private User user = new User();
        public BookShelf bookShelf;
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
         private string loggedinUserId;
        private List<BookShelf> booksList;
        public HomePage(BookShelf bookshelf)
        {
            InitializeComponent();
            bookShelf = bookshelf;
            loggedinUserId = bookShelf.UserEmail;
            booksList = new List<BookShelf>();
            this.txtEmail.Text = loggedinUserId;    
            context = new DynamoDBContext();
            btnBookList.IsEnabled = false;
            btnAddSnapshot.IsEnabled = false;
            btnDeleteShelf.IsEnabled = false;
            btnDisplaySbanshot.IsEnabled = false;
          
        }


        public void resetBtns()
        {
            txtAuthor1.Clear();
            txtAuthor2.Clear();
            txtAuthor3.Clear();
            txtISBN.Clear();
            txtTitle.Clear();
        }

        private void btnAddToShelf_Click(object sender, RoutedEventArgs e)
        {
            AddBookToShelf();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.UserEmail = txtEmail.Text;
        }

        private void txtISBN_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.ISBN = txtISBN.Text;
        }

        private void txtAuthor1_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.Author1 = txtAuthor1.Text;
        }

        private void txtAuthor2_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.Author2 = txtAuthor2.Text;
        }

        private void txtAuthor3_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.Author3 = txtAuthor3.Text;
        }

        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            bookShelf.BookTitle = txtTitle.Text;
        }
        private void CreateTable(string tabNam, string hashKey)
        {
            client = new AmazonDynamoDBClient();
            var tableResponse = client.ListTables();
            if (!tableResponse.TableNames.Contains(tabNam))
            {

                MessageBox.Show("Shelf doesn't exist, creating shelf " + tabNam);
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
                MessageBox.Show($"Shelf {tabNam} Created Successfully!");
                SaveBookinShelf();
                enableBtns();
                //resetBtns();
            }
            else
            {
                MessageBox.Show($"{tabNam} exist already!, Adding {txtTitle.Text} book in the shelf");
                SaveBookinShelf();
                //resetBtns();
                enableBtns();
            }
        }
        public void enableBtns()
        {
            this.btnAddSnapshot.IsEnabled = true;
            this.btnAddToShelf.IsEnabled = true;
            this.btnBookList.IsEnabled = true;
            this.btnDisplaySbanshot.IsEnabled = true;
            this.btnDeleteShelf.IsEnabled = true;
            this.btnReset.IsEnabled = true;
        }
        public void disableBtns()
        {
            this.btnAddSnapshot.IsEnabled = false;
            this.btnAddToShelf.IsEnabled = false;
            this.btnBookList.IsEnabled = false;
            this.btnDisplaySbanshot.IsEnabled = false;
            this.btnReset.IsEnabled = false;
            this.btnDeleteShelf.IsEnabled = false;
        }
        private void AddBookToShelf()
        {
         string bucketName = "BookShelf";
        string hashKey = "ISBN";
            try
            {
                if (string.IsNullOrEmpty(txtTitle.Text) || string.IsNullOrEmpty(txtISBN.Text))
                {
                    MessageBox.Show("Fields can't be empty!", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                }
                else
                {
                    CreateTable(bucketName,hashKey);
                    btnBookList.IsEnabled = true;
                }
            }
          
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        public void SaveBookinShelf()
        {
            //Set a local DB context
            context = new DynamoDBContext(client);
            //Create an BookShelf object to save
            bookShelf.IsActive = true;
            bookShelf.BookId = Guid.NewGuid().ToString();
            //Save an Book object
            context.Save<BookShelf>(bookShelf);
            MessageBox.Show("Book added in the shelf Successfully!");
            resetBtns();
        }



        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetBtns();
        }

        private void btnBookList_Click(object sender, RoutedEventArgs e)
        {
            QueryBooksList(context);
        }
        private void QueryBooksList(DynamoDBContext context)
        {
            string tabName = "BookShelf";
            List<string> currentTables = client.ListTables().TableNames;
            if (currentTables.Contains(tabName))
            {
                IEnumerable<BookShelf> ownedBooksResult;
                ownedBooksResult = context.Scan<BookShelf>(
                    new ScanCondition("UserEmail", ScanOperator.Equal, loggedinUserId));
                Console.WriteLine("List retrieved " + ownedBooksResult);
                foreach (var result in ownedBooksResult)
                {
                    if (result != null)
                    {
                        BookShelf newShelf = new BookShelf()
                        {
                            BookId = result.BookId,
                            BookTitle = result.BookTitle,
                            IsActive = result.IsActive,
                            ISBN = result.ISBN,
                            Author1 = result.Author1,
                            Author2 = result.Author2,
                            Author3 = result.Author3,
                            UserEmail = result.UserEmail,
                        };

                        booksList.Add(newShelf);
                    }
                    lstBooks.ItemsSource = null;
                    lstBooks.ItemsSource = booksList;
                }
            }

        }
       
        private void btnAddSnapshot_Click(object sender, RoutedEventArgs e)
        {
           
            Console.WriteLine("Selected index, "+ lstBooks.SelectedIndex);
            BookShelf selectedBook =(BookShelf) lstBooks.SelectedItem;
            if (lstBooks.SelectedIndex > -1)
            {
                AddSnapshot addSnapshot = new AddSnapshot(selectedBook);
                addSnapshot.Show();
               
            } else
            {
                MessageBox.Show("Choose a book to add snapshot!");
            }
          
           
        }

        private void btnDisplaySbanshot_Click(object sender, RoutedEventArgs e)
        {
            DisplaySnapShot snapShots = new DisplaySnapShot(bookShelf);
            snapShots.Show();
        }

        private async void btnDeleteShelf_Click(object sender, RoutedEventArgs e)
        {
            string tableName = "BookShelf";
            await DeletingTable_async(tableName);
            await DeleteSnapshot_async();
            MessageBox.Show("Shelf and snapshots deleted successfully! ");
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
        private async Task<bool> DeletingTable_async(string tableName)
        {

            Task tblDelete = client.DeleteTableAsync(tableName);
            try
            {
                await tblDelete;
            }
            catch (Exception ex)
            {
               Console.WriteLine("  ERROR: Failed to delete the table =>" + ex.Message);

                return (false);
            }
           Console.WriteLine("Shelf Successfully deleted the table!");

            return (true);
        }
        private async Task<bool> DeleteSnapshot_async()
        {
            Task tblSnapshotDelete = client.DeleteTableAsync("Snapshot");
            try
            {
                await tblSnapshotDelete;
            }
            catch (Exception ex)
            {
                Console.WriteLine("  ERROR: Failed to delete the table =>" + ex.Message);

                return (false);
            }
            Console.WriteLine("Shelf Successfully deleted the table!");

            return (true);
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }
    }
}






