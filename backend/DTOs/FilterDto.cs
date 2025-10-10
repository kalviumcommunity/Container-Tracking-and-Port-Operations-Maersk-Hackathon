using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// Generic pagination request DTO
    /// </summary>
    public class PaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc"; // asc or desc
    }

    /// <summary>
    /// Generic pagination response DTO
    /// </summary>
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// Container filtering options (enhanced for complete field support)
    /// </summary>
    public class ContainerFilterDto : PaginationRequest
    {
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? CargoType { get; set; }
        public string? Condition { get; set; } // Added condition filter
        public string? Size { get; set; } // Added size filter
        public string? CurrentLocation { get; set; }
        public string? Destination { get; set; }
        public int? ShipId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? MinTemperature { get; set; } // Added temperature filters
        public decimal? MaxTemperature { get; set; }
        public string? Coordinates { get; set; } // Added coordinates filter
        public string? SearchTerm { get; set; } // Global search across multiple fields
    }

    /// <summary>
    /// Ship filtering options
    /// </summary>
    public class ShipFilterDto : PaginationRequest
    {
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Flag { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public string? CurrentPort { get; set; }
        public string? SearchTerm { get; set; }
    }

    /// <summary>
    /// Berth filtering options
    /// </summary>
    public class BerthFilterDto : PaginationRequest
    {
        public string? Status { get; set; }
        public string? Type { get; set; }
        public int? PortId { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public bool? HasAvailableCapacity { get; set; }
        public string? SearchTerm { get; set; }
    }

    /// <summary>
    /// User filtering options for admin
    /// </summary>
    public class UserFilterDto : PaginationRequest
    {
        public string? Status { get; set; } // Active, Inactive, Blocked
        public string? Role { get; set; }
        public DateTime? RegisteredAfter { get; set; }
        public DateTime? RegisteredBefore { get; set; }
        public string? SearchTerm { get; set; } // Name, email search
    }

    /// <summary>
    /// Role application filtering
    /// </summary>
    public class RoleApplicationFilterDto : PaginationRequest
    {
        public string? Status { get; set; } // Pending, Approved, Rejected
        public string? RequestedRole { get; set; }
        public int? UserId { get; set; }
        public DateTime? SubmittedAfter { get; set; }
        public DateTime? SubmittedBefore { get; set; }
    }

    /// <summary>
    /// Container statistics DTO
    /// </summary>
    public class ContainerStatsDto
    {
        public int TotalContainers { get; set; }
        public int AvailableContainers { get; set; }
        public int InTransitContainers { get; set; }
        public int AtPortContainers { get; set; }
        public int LoadingContainers { get; set; }
        public int UnloadingContainers { get; set; }
        public Dictionary<string, int> ContainersByType { get; set; } = new();
        public Dictionary<string, int> ContainersByStatus { get; set; } = new();
        public Dictionary<string, int> ContainersByLocation { get; set; } = new();
    }

    /// <summary>
    /// Bulk status update DTO
    /// </summary>
    public class BulkStatusUpdateDto
    {
        public List<string> ContainerIds { get; set; } = new();
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Bulk update result DTO
    /// </summary>
    public class BulkUpdateResultDto
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> FailedContainerIds { get; set; } = new();
        public List<string> ErrorMessages { get; set; } = new();
    }
}