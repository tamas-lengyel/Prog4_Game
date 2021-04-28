// <copyright file="IStorageRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

[assembly: System.CLSCompliant(false)]

namespace Repository
{
    /// <summary>
    /// Interface for all repositories.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IStorageRepository<T>
        where T : class
    {
        /// <summary>
        /// Insert method.
        /// </summary>
        /// <param name="entity">Generic type entity.</param>
        void Insert(T entity);
    }
}