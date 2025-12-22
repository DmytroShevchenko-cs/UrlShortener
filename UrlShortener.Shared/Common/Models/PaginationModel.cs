namespace UrlShortener.Shared.Common.Models;

public class PaginationModel
{
    private const int MaxPageSize = 150;
    private int _pageSize = 10;
    private int _offset;

    public int Offset
    {
        get => _offset;
        set
        {
            if (value < 0)
                value = 0;
            _offset = value;
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 0)
                value = 0;
            _pageSize = value > 150 ? 150 : value;
        }
    }
}