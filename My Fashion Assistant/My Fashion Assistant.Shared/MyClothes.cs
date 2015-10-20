using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My_Fashion_Assistant
{
    [Table("MyClothes")]
    public class MyClothes
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string nameClothes { get; set; }

        public string typeClothes { get; set; }

       public string imgPath { get; set; }
       public string season { get; set; }
    
       
    }
}
