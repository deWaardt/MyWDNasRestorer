using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD_File_Recovery
{
    internal class Item
    {
        public string id { get; set; }
        public string contentID { get; set; }
        public string filename { get; set; }
        public string parentID { get; set; }
        public string type { get; set; }
    }
}
