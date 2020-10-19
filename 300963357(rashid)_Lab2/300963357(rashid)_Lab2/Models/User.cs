using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300963357_rashid__Lab2.Models
{
    [DynamoDBTable("Users")]
    public class User
    {
        public string UserEmail { get ; set; }
        public string Password { get; set; }
    }
}
