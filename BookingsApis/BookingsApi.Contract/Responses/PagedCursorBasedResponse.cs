namespace BookingsApi.Contract.Responses
{
    /// <summary>
    /// Base class for paged cursor base pagination responses where each
    /// response represents a next cursor to get data.
    /// </summary>
    public abstract class PagedCursorBasedResponse
    {
        /// <summary>
        /// Gets or sets a unique sequential value to get next set of records. 
        /// value is set to 0 if no records to return.
        /// </summary>
        public string NextCursor { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items returned for the page.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Absolute url to the previous page of items.
        /// Will be null for the first page.
        /// </summary>
        public string PrevPageUrl { get; set; }

        /// <summary>
        /// Absolute url for the next page of items.
        /// Will be null for the last page.
        /// </summary>
        public string NextPageUrl { get; set; }
    }
}