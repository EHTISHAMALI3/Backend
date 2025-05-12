using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Hidayah.Application.Interfaces;
using Hidayah.Infrastrcture.AppDbContext;
using Hidayah.Infrastrcture.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public class GenericRepositoryImpl<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<GenericRepositoryImpl<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbSet<T> _dbSet;

        public GenericRepositoryImpl(ApplicationDbContext applicationDbContext,
                                      ILogger<GenericRepositoryImpl<T>> logger,
                                      IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _dbSet = applicationDbContext.Set<T>();
        }

        public async Task<mGeneric.mApiResponse<List<T>>> GetAllAsync()
        {
            try
            {
                IQueryable<T> query = _dbSet;

                // Apply soft delete filter if "Deleted" exists
                if (typeof(T).GetProperty("Deleted") != null)
                {
                    var param = Expression.Parameter(typeof(T), "x");
                    var prop = Expression.Property(param, "Deleted");
                    var condition = Expression.Equal(prop, Expression.Constant(false));
                    var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
                    query = query.Where(lambda);
                }

                var list = await query.ToListAsync();

                // If needed, you can process path formatting (e.g., trim or standardize slashes)
                foreach (var entity in list)
                {
                    var svgProp = typeof(T).GetProperty("SvgPath");
                    var audioProp = typeof(T).GetProperty("AudioPath");

                    if (svgProp != null)
                    {
                        var svgPath = svgProp.GetValue(entity)?.ToString();
                        if (!string.IsNullOrWhiteSpace(svgPath))
                            svgProp.SetValue(entity, svgPath.Trim()); // or Path.GetFileName(svgPath)
                    }

                    if (audioProp != null)
                    {
                        var audioPath = audioProp.GetValue(entity)?.ToString();
                        if (!string.IsNullOrWhiteSpace(audioPath))
                            audioProp.SetValue(entity, audioPath.Trim());
                    }
                }

                return new mGeneric.mApiResponse<List<T>>(200, "Success", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GenericRepo][{typeof(T).Name}] Error in GetAllAsync");
                return new mGeneric.mApiResponse<List<T>>(500, "Internal server error");
            }
        }

        public async Task<mGeneric.mApiResponse<PaginatedResponseDto<T>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
        {
            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();

                var totalRecords = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var paginatedResult = new PaginatedResponseDto<T>(items, totalRecords);

                return new mGeneric.mApiResponse<PaginatedResponseDto<T>>(200, "Success", paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GenericRepo][{typeof(T).Name}] Error in GetAllPaginatedAsync");
                return new mGeneric.mApiResponse<PaginatedResponseDto<T>>(500, "Internal server error");
            }
        }

        public async Task<mGeneric.mApiResponse<T>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return new mGeneric.mApiResponse<T>(404, "Not found");

                var deletedProp = typeof(T).GetProperty("Deleted");
                if (deletedProp != null && (bool)deletedProp.GetValue(entity))
                    return new mGeneric.mApiResponse<T>(404, "Record deleted");

                var svgProp = typeof(T).GetProperty("SvgPath");
                var audioProp = typeof(T).GetProperty("AudioPath");

                if (svgProp != null)
                {
                    var svgPath = svgProp.GetValue(entity)?.ToString();
                    if (!string.IsNullOrWhiteSpace(svgPath))
                        svgProp.SetValue(entity, svgPath.Trim()); // return just the path as-is
                }

                if (audioProp != null)
                {
                    var audioPath = audioProp.GetValue(entity)?.ToString();
                    if (!string.IsNullOrWhiteSpace(audioPath))
                        audioProp.SetValue(entity, audioPath.Trim()); // return just the path as-is
                }

                return new mGeneric.mApiResponse<T>(200, "Success", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Error");
                return new mGeneric.mApiResponse<T>(500, "Server error");
            }
        }


        public async Task<mGeneric.mApiResponse<string>> AddAsync(T entity)
        {
            try
            {
                // Set CreatedAt dynamically
                var createdAtProp = typeof(T).GetProperty("CreatedAt");
                if (createdAtProp != null)
                    createdAtProp.SetValue(entity, DateTime.Now);

                // Sanitize name
                var nameProp = typeof(T).GetProperty("Name");
                var name = nameProp?.GetValue(entity)?.ToString()?.Trim() ?? Guid.NewGuid().ToString("N");

                // Remove all whitespace characters
                name = string.Concat(name.Where(c => !char.IsWhiteSpace(c)));

                // Sanitize for invalid filename chars
                var sanitizedFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));

                // Handle SVG File
                var svgFileProp = typeof(T).GetProperty("SvgFile");
                var svgPathProp = typeof(T).GetProperty("SvgPath");

                if (svgFileProp != null && svgPathProp != null)
                {
                    var svgFile = svgFileProp.GetValue(entity) as IFormFile;
                    if (svgFile != null && svgFile.Length > 0)
                    {
                        var path = await FileHelper.SaveFileAsync(svgFile, "assets/icons", sanitizedFileName);
                        svgPathProp.SetValue(entity, path);
                    }
                }

                // Handle Audio File
                var audioFileProp = typeof(T).GetProperty("AudioFile");
                var audioPathProp = typeof(T).GetProperty("AudioPath");

                if (audioFileProp != null && audioPathProp != null)
                {
                    var audioFile = audioFileProp.GetValue(entity) as IFormFile;
                    if (audioFile != null && audioFile.Length > 0)
                    {
                        var path = await FileHelper.SaveFileAsync(audioFile, "assets/audio", sanitizedFileName);
                        audioPathProp.SetValue(entity, path);
                    }
                }

                _dbSet.Add(entity);
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddAsync Error");
                return new mGeneric.mApiResponse<string>(500, "Server error");
            }
        }

        public async Task<mGeneric.mApiResponse<string>> UpdateAsync(T entity)
        {
            try
            {
                var modifiedAt = typeof(T).GetProperty("ModifiedAt");
                if (modifiedAt != null) modifiedAt.SetValue(entity, DateTime.Now);

                _dbSet.Update(entity);
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Error");
                return new mGeneric.mApiResponse<string>(500, "Server error");
            }
        }

        public async Task<mGeneric.mApiResponse<string>> SoftDeleteAsync(int id, string modifiedBy)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return new mGeneric.mApiResponse<string>(404, "Not found");

                var deletedProp = typeof(T).GetProperty("Deleted");
                var modifiedAtProp = typeof(T).GetProperty("ModifiedAt");
                var modifiedByProp = typeof(T).GetProperty("ModifiedBy");

                if (deletedProp != null)
                {
                    var currentValue = (bool?)deletedProp.GetValue(entity) ?? false;
                    deletedProp.SetValue(entity, !currentValue); // 👈 Toggle
                }

                if (modifiedAtProp != null) modifiedAtProp.SetValue(entity, DateTime.Now);
                if (modifiedByProp != null) modifiedByProp.SetValue(entity, modifiedBy);

                _dbSet.Update(entity);
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Status Updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SoftDeleteAsync Error");
                return new mGeneric.mApiResponse<string>(500, "Server error");
            }
        }

        public static async Task<string> SaveFileAsync(IFormFile file, string subFolder, string fileNameWithoutExt = null)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);
            var sanitizedName = fileNameWithoutExt ?? Guid.NewGuid().ToString("N");
            sanitizedName = Path.GetInvalidFileNameChars().Aggregate(sanitizedName, (current, c) => current.Replace(c, '_'));

            var fileName = $"{sanitizedName}{extension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(subFolder, fileName).Replace("\\", "/"); // relative path
        }
        public async Task<mGeneric.mApiResponse<string>> DeletePermanentAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return new mGeneric.mApiResponse<string>(404, "Entity not found");

                // Delete associated file paths (SvgPath, AudioPath, etc.)
                var filePathProps = typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(string) &&
                                (p.Name.ToLower().Contains("path") || p.Name.ToLower().EndsWith("file")));

                foreach (var prop in filePathProps)
                {
                    var relativePath = prop.GetValue(entity)?.ToString();
                    if (!string.IsNullOrWhiteSpace(relativePath))
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(fullPath))
                            System.IO.File.Delete(fullPath);
                    }
                }

                // Remove the entity without checking if it's marked as deleted
                _dbSet.Remove(entity);
                await _applicationDbContext.SaveChangesAsync();

                return new mGeneric.mApiResponse<string>(200, "Entity permanently deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeletePermanentAsync Error");
                return new mGeneric.mApiResponse<string>(500, "Server error");
            }
        }



    }
}