using System;

namespace Rise.Domain.Common;

public interface IEntity
{
    /// <summary>
    /// Primary Key of the <see cref="Entity"/>
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Date of the initial creation.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date of the last update.
    /// </summary>
    DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Soft Delete indicator, instead of deleting rows, we flag them as deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}
