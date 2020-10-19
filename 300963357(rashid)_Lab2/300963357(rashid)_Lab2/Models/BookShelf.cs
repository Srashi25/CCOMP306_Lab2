using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300963357_rashid__Lab2.Models
{
    [DynamoDBTable("BookShelf")]
    public class BookShelf
    {
      
        public string BookId { get; set; }
        public string UserEmail { get; set; }
        public bool IsActive { get; set; }
        public string BookTitle { get; set; }
        public string ISBN { get; set; }
        public string Author1 { get; set; }
        public string Author2 { get; set; }
        public string Author3 { get; set; }
    }
   
}

