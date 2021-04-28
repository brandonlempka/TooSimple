using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class PagerVM
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string FullUrlWithoutPageNumber { get; set; }
        public bool IsAjaxPager { get; set; }

        public string ReplaceWithSelector { get; set; }

        public List<int?> PageNumbers { get; set; }

        public PagerVM(int currentPage, int totalPages, string fullUrlWithoutPageNumber)
        {
            this.CurrentPage = currentPage;
            this.FullUrlWithoutPageNumber = fullUrlWithoutPageNumber;
            this.TotalPages = totalPages;

            this.PageNumbers = new List<int?>();
            this.PageNumbers.Add(1);

            if (currentPage <= 4 || totalPages <= 7)
            {
                // if is beginning of pages
                // 1  2  3 4 5 ... 10
                if (totalPages >= 2)
                    this.PageNumbers.Add(2);
                if (totalPages >= 3)
                    this.PageNumbers.Add(3);
                if (totalPages >= 4)
                    this.PageNumbers.Add(4);
                if (totalPages >= 5)
                    this.PageNumbers.Add(5);

                if (totalPages > 7)
                    this.PageNumbers.Add(null);
                else if (totalPages >= 6)
                    this.PageNumbers.Add(6);

                if (totalPages >= 7)
                    this.PageNumbers.Add(totalPages);
            }
            else if (currentPage > 4 && totalPages > currentPage + 3)
            {
                // if is mid of pages
                // 1 ... 4 5 6 ... 10
                this.PageNumbers.Add(null);
                this.PageNumbers.Add(currentPage - 1);
                this.PageNumbers.Add(currentPage);
                this.PageNumbers.Add(currentPage + 1);
                this.PageNumbers.Add(null);
                this.PageNumbers.Add(totalPages);
            }
            else
            {
                // if is end of pages
                // 1 ... 6 7 8  9  10
                this.PageNumbers.Add(null);
                this.PageNumbers.Add(totalPages - 4);
                this.PageNumbers.Add(totalPages - 3);
                this.PageNumbers.Add(totalPages - 2);
                this.PageNumbers.Add(totalPages - 1);
                this.PageNumbers.Add(totalPages);
            }
        }
    }
}
