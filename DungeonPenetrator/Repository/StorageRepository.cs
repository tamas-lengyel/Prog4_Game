// <copyright file="StorageRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    /// <summary>
    /// Storage repo.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public abstract class StorageRepository<T> : IStorageRepository<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageRepository{T}"/> class.
        /// </summary>
        protected StorageRepository()
        {
        }

        /// <summary>
        /// Insert a Generic type as json into a file.
        /// </summary>
        /// <param name="entity">Generic type.</param>
        public abstract void Insert(T entity);
    }
}