using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Domain.Services
{
    public interface IMetadataService
    {
        Task<EntityOperationResult<metadata>> CreateMetadata(metadata metadata);
        Task<EntityOperationResult<metadata>> UpdateMetadata(metadata metadata);
        Task<EntityOperationResult<metadata>> DeleteMetadata(int? id);
    }

    public class MetadataService: IMetadataService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        public MetadataService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        public async Task<EntityOperationResult<metadata>> CreateMetadata(metadata metadata)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var entity = await unitOfWork.metadata.InsertAsyncWithoutHistory(metadata);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<metadata>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<metadata>.Failure().AddError(ex.Message);
                }
            }
          
        }

        public async Task<EntityOperationResult<metadata>> UpdateMetadata(metadata metadata)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var metadataFromDb = unitOfWork.metadata.GetByDefId(metadata.defid);
                    if (metadataFromDb==null) 
                        return EntityOperationResult<metadata>.Failure().AddError("Объект с текущим id не существует");
                    metadataFromDb.data = metadata.data;
                    metadataFromDb.type = metadata.type;
                    metadataFromDb.updated_at = DateTime.Now;
                    unitOfWork.metadata.UpdateWithoutHistory(metadataFromDb);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<metadata>.Success(metadata);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<metadata>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<metadata>> DeleteMetadata(int? id)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var metadata = unitOfWork.metadata.Get(id);
                    if (metadata!=null)
                    {
                        
                        unitOfWork.metadata.DeleteWithoutHistory(metadata);
                        await unitOfWork.CompleteAsync();
                        return EntityOperationResult<metadata>.Success(metadata);
                    }
                    else
                        return EntityOperationResult<metadata>.Failure().AddError("Объект с текущим id не существует");
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<metadata>.Failure().AddError(ex.Message);
                }
            }
        }
    }
}
