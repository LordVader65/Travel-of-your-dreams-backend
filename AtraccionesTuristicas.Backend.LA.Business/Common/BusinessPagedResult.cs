namespace AtraccionesTuristicas.Backend.LA.Business.Common;

public sealed class BusinessPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
    }

