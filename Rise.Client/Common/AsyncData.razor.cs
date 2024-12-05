using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Common
{
    public partial class AsyncData<T> : ComponentBase
    {
        [Inject]
        public required ISnackbar SnackbarService { get; set; }

        [Parameter, EditorRequired]
        public required string TestIdPrefix { get; set; }

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

        /// <summary>
        /// Current loading state
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Raises the <see cref="LoadingChanged"/> event upon change.  When bound via <c>@bind-Loading</c>, this property is updated when the loading is changed.
        /// </remarks>
        protected bool IsLoading { get; set; } = false;

        [Parameter]
        public bool DisableLoader { get; set; } = false;

        protected bool HasError { get; set; } = false;
        protected string? ErrorMessage { get; set; }

        [Parameter]
        public AsyncErrorDisplayMethod ErrorDisplayMethod { get; set; } = AsyncErrorDisplayMethod.StaticAlert;

        [Parameter]
        public bool ShowContentWhenError { get; set; } = true;

        protected virtual Task AssignData(T? data)
        {
            _cachedData = Data;
            Data = data;
            return DataChanged.InvokeAsync(Data);
        }

        private bool IsCachedDataEqual()
        {
            return _cachedData?.Equals(Data) ?? false;
        }

        protected override async Task OnInitializedAsync()
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
                    await AssignData(newData);
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

        protected bool ShowStaticAlert()
        {
            return ErrorDisplayMethod == AsyncErrorDisplayMethod.StaticAlert || ErrorDisplayMethod == AsyncErrorDisplayMethod.Both;
        }

        protected bool ShowSnackBarAlert()
        {
            return ErrorDisplayMethod == AsyncErrorDisplayMethod.SnackBarAlert || ErrorDisplayMethod == AsyncErrorDisplayMethod.Both;
        }

        protected bool ShowContent()
        {
            return ShowContentWhenError || !HasError;
        }

        protected bool ShowLoader()
        {
            return !DisableLoader && IsLoading;
        }

        protected string PrefixTestId(string testId)
        {
            StringBuilder sb = new(TestIdPrefix);
            if (sb.Length > 0)
            {
                char lastChar = sb[^1];
                char joinChar = '-';
                if (lastChar != joinChar)
                    sb.Append(joinChar);
            }
            sb.Append(testId);

            return sb.ToString();
        }

        private string TestIdLoading()
        {
            return PrefixTestId("loading-progress");
        }

        private string TestIdError()
        {
            return PrefixTestId("fetch-error");
        }
    }

    public enum AsyncErrorDisplayMethod
    {
        StaticAlert,
        SnackBarAlert,
        Both,
    }

}

