namespace Rise.Shared.Pagination
{
    public class ItemsPageDto<T>
    {
        public IEnumerable<T> Data { get; set; }
    public int? NextId { get; set; }
    public int? PreviousId { get; set; }
    public bool IsFirstPage { get; set; }
    public bool CanGoBack { get; set; } // Add this
    public bool CanGoForward { get; set; } // Add this
    public int? ForwardCursor { get; set; } // Add this
    public int? BackwardCursor { get; set; } // Add this
}

}