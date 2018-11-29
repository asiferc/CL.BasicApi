using System.Collections.Generic;

namespace Cardlytics.BasicApi.Models
{
    public class BaseFilter
    {
        public BaseFilter()
        {
            Include = new List<string>();
            PageNumber = 1;
            PageSize = 25;
        }

        public List<string> Include { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}