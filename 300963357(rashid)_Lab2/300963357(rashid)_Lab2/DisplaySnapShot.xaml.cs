using _300963357_rashid__Lab2.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for DisplaySnapShot.xaml
    /// </summary>
    public partial class DisplaySnapShot : Window
    {
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private List<Snapshot> snapshotList;
        private BookShelf bshelf;
        private string userEmail;
        public DisplaySnapShot(BookShelf bookShelf)
        {
            InitializeComponent();
            snapshotList = new List<Snapshot>();
            bshelf = bookShelf;
            userEmail = bshelf.UserEmail;
            context = new DynamoDBContext();
            client = new AmazonDynamoDBClient();
            GetSnapshots(context);
           
        }

        private void GetSnapshots(DynamoDBContext context)
        {
            List<string> currentTables = client.ListTables().TableNames;
            if (currentTables.Contains("Snapshot"))
            {
                IEnumerable<Snapshot> ownedBooksResult;
                Console.WriteLine("Running scan to get owned books");
                ownedBooksResult = context.Scan<Snapshot>(
                    new ScanCondition("Email", ScanOperator.Equal, userEmail));
                Console.WriteLine("List retrieved " + ownedBooksResult);
                foreach (var result in ownedBooksResult)
                {
                    if (result != null)
                    {
                        Snapshot snap = new Snapshot()
                        {
                            SnapshotId = result.SnapshotId,
                            BookTitle = result.BookTitle,
                            Email = result.Email,
                            DateTime = result.DateTime,
                            LastPage= result.LastPage,
                        };

                       snapshotList.Add(snap);
                    }

                    lstSnapshot.ItemsSource = snapshotList;
                }

            }
        }
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
 
    }
}
