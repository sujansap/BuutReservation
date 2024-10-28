namespace Rise.Shared.Pagination
{
    public class ItemsPageDto<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int? NextId { get; set; }
        public int? PreviousId { get; set; }
        public bool IsFirstPage { get; set; }

    }

}