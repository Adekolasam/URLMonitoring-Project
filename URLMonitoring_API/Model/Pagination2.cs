//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;

//namespace URLMonitoring_API.Model
//{
//    public class PaginationFilter
//    {
//        private readonly int maxPageSize;
//        public int PageNumber { get; set; }
//        public int PageSize { get; set; }
//        public PaginationFilter(IConfiguration config)
//        {
//            maxPageSize = int.Parse(config["Pagination:MaxPageSize"]);
//            PageNumber = int.Parse(config["Pagination:PageNumber"]);
//            PageSize = int.Parse(config["Pagination:PageSize"]);
//        }
//        public PaginationFilter(IConfiguration config, int pageNumber, int pageSize)
//        {
//            maxPageSize = int.Parse(config["Pagination:MaxPageSize"]);
//            this.PageNumber = pageNumber < int.Parse(config["Pagination:PageNumber"]) ? int.Parse(config["Pagination:PageNumber"]) : pageNumber;
//            this.PageSize = pageSize > maxPageSize ? maxPageSize : pageSize;
//        }
//    }

//    public class PagedResponse<T>
//    {
//        public int PageNumber { get; set; }
//        public int PageSize { get; set; }
//        public Uri FirstPage { get; set; }
//        public Uri LastPage { get; set; }
//        public int TotalPages { get; set; }
//        public int TotalRecords { get; set; }
//        public Uri NextPage { get; set; }
//        public Uri PreviousPage { get; set; }
//        public T Data { get; set; }

//        public PagedResponse(T data, int pageNumber, int pageSize)
//        {
//            Data = data;
//            PageNumber = pageNumber;
//            PageSize = pageSize;
//        }
//    }


//    public interface IUriService
//    {
//        public Uri GetPageUri(PaginationFilter filter, string route);
//    }

//    public class UriService : IUriService
//    {
//        private readonly string _baseUri;
//        public UriService(string baseUri)
//        {
//            _baseUri = baseUri;
//        }
//        public Uri GetPageUri(PaginationFilter filter, string route)
//        {
//            var _enpointUri = new Uri(string.Concat(_baseUri, route));
//            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
//            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
//            return new Uri(modifiedUri);
//        }
//    }

//    public class PaginationHelper
//    {
//        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
//        {
//            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
//            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
//            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
//            respose.NextPage =
//                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
//                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
//                : null;
//            respose.PreviousPage =
//                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
//                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
//                : null;
//            respose.FirstPage = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
//            respose.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
//            respose.TotalPages = roundedTotalPages;
//            respose.TotalRecords = totalRecords;
//            return respose;
//        }
//    }
//}


////return FindAll()
////        .OrderBy(on => on.Name)
////        .Skip((ownerParameters.PageNumber - 1) * ownerParameters.PageSize)
////        .Take(ownerParameters.PageSize)
////        .ToList();