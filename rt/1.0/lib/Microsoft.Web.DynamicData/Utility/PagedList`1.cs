using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
            : this(source.Count(), index, pageSize) {
            AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList());
        }

        public PagedList(IEnumerable<T> source, int index, int pageSize)
            : this(source.Count(), index, pageSize) {
            AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList());
        }

        protected PagedList(int count, int index, int pageSize) {
            TotalCount = count;
            PageSize = Math.Max(pageSize, 1);
            CurrentPage = Math.Max(index, 1);

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;
        }

        public int CurrentPage { get; set; }

        public bool HasPreviousPage {
            get { return CurrentPage > 1; }
        }

        public bool HasNextPage {
            get { return CurrentPage < TotalPages; }
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages {
            get { return Math.Max((TotalCount + PageSize - 1) / PageSize, 1); }
        }
    }
}