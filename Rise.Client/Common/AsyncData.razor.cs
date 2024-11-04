using Microsoft.AspNetCore.Components;

namespace Rise.Client.Common
{
    public partial class AsyncData<T> : ComponentBase
    {
        private bool shouldRender;

        private Func<Task<T>> _previousDataFetcher = default!;

        [Parameter, EditorRequired]
        public required Func<Task<T>> DataFetcher
        {
            get; set;
        }

        private T? _cachedData;

        /// <summary>
        /// The fetched data
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Raises the <see cref="DataChanged"/> event upon change.  When bound via <c>@bind-Data</c>, this property is updated when the data is changed.
        /// </remarks>
        [Parameter]
        public T? Data
        {
            get; set;
        }
        /// <summary>
        /// Occurs when the <see cref="Data"/> value has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> DataChanged { get; set; }

        protected bool IsLoading { get; private set; } = false;
        [Parameter]
        public bool DisableLoader { get; set; } = false;

        protected bool HasError { get; private set; } = false;
        protected string? ErrorMessage { get; set; }

        [Parameter]
        public AsyncErrorDisplayMethod ErrorDisplayMethod { get; set; } = AsyncErrorDisplayMethod.StaticAlert;

        [Parameter]
        public bool ShowContentWhenError { get; set; } = true;

        [Parameter, EditorRequired]
        public required RenderFragment ChildContent { get; set; }

        private bool IsCachedDataEqual()
        {
            return _cachedData?.Equals(Data) ?? false;
        }

        protected override async Task OnParametersSetAsync()
        {
            bool isDifferentFetcher = _previousDataFetcher != DataFetcher;
            bool isCachedDataEqual = IsCachedDataEqual();
            shouldRender = isDifferentFetcher || !isCachedDataEqual;

            _previousDataFetcher = DataFetcher;
            if (isDifferentFetcher || !isCachedDataEqual)
            {
                await FetchData();
            }
        }
        public async Task FetchData()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                HasError = false;

                try
                {
                    T? newData = await DataFetcher();
                    _cachedData = Data;
                    Data = newData;
                    await DataChanged.InvokeAsync(Data);
                }
                catch (Exception ex)
                {
                    HasError = true;
                    // TODO fix localisation of error message
                    ErrorMessage = ex.Message ?? "Oops something went wrong";
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        protected override bool ShouldRender() => shouldRender;

        private bool ShowStaticAlert()
        {
            return ErrorDisplayMethod == AsyncErrorDisplayMethod.StaticAlert || ErrorDisplayMethod == AsyncErrorDisplayMethod.Both;
        }

        private bool ShowSnackBarAlert()
        {
            return ErrorDisplayMethod == AsyncErrorDisplayMethod.SnackBarAlert || ErrorDisplayMethod == AsyncErrorDisplayMethod.Both;
        }
    }

    public enum AsyncErrorDisplayMethod
    {
        StaticAlert,
        SnackBarAlert,
        Both,
    }

}

