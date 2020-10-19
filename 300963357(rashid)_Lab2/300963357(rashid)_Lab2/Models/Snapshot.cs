using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300963357_rashid__Lab2.Models
{
    [DynamoDBTable("Snapshot")]
    public class Snapshot
    {
        public string SnapshotId { get; set ; }
        public string BookTitle { get; set; }
        public string Email { get; set; }
        public int LastPage { get; set; }
        public DateTime DateTime { get; set; }
    }
}
