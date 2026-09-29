namespace ProductSellPurchase.ViewModels
{
    public class RequestPaginationViewModel
    {
        public int Start { get; set; }
        public int Length { get; set; }
        //public string? Search { get; set; }
        public IEnumerable<KColumn>? Columns { get; set; }
        public object Draw { get; set; }
        //public Filters? Filters { get; set; }
    }
    public class KColumn
    {
        public string? Field { get; set; }
        public KSort? Sort { get; set; }
        public bool IsSortable { get; set; }
    }
    public class KSort
    {
        public KSortDirection? Direction { get; set; }
    }
    public enum KSortDirection
    {
        Ascending = 0,
        Descending = 1
    }
    public class ResponseList
    {
        public int? TotalCount { get; set; }
        public int? FilteredCount { get; set; }
        public object? Result { get; set; }
    }
    //public class Filters
    //{
    //    public DateTime? field1 { get; set; }
    //    public DateTime? field2 { get; set; }
    //}
}
