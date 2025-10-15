using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class ContainerStorageFeeService : IContainerStorageFeeService
    {
        private readonly IContainerStorageFeeRepository _repository;

        public ContainerStorageFeeService(IContainerStorageFeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ContainerStorageFeeDto> CreateAsync(ContainerStorageFeeCreateUpdateDto createDto)
        {
            var fee = new ContainerStorageFee
            {
                ContainerId = createDto.ContainerId,
                PortId = createDto.PortId,
                StorageStartDate = createDto.StorageStartDate,
                StorageEndDate = createDto.StorageEndDate,
                DailyStorageRate = createDto.DailyStorageRate,
                FeeStatus = createDto.FeeStatus
            };
            var newFee = await _repository.CreateAsync(fee);
            return MapToDto(newFee);
        }

        public async Task<bool> DeleteAsync(object id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ContainerStorageFeeDto>> GetAsync(Expression<Func<ContainerStorageFee, bool>> filter)
        {
            var fees = await _repository.GetAsync(filter);
            return fees.Select(MapToDto);
        }

        public async Task<IEnumerable<ContainerStorageFeeDto>> GetAllAsync()
        {
            var fees = await _repository.GetAllAsync();
            return fees.Select(MapToDto);
        }

        public async Task<ContainerStorageFeeDto> GetByIdAsync(object id)
        {
            var fee = await _repository.GetByIdAsync(id);
            return fee == null ? null : MapToDto(fee);
        }

        public async Task<ContainerStorageFeeDto> UpdateAsync(object id, ContainerStorageFeeCreateUpdateDto updateDto)
        {
            var existingFee = await _repository.GetByIdAsync(id);
            if (existingFee == null)
            {
                throw new KeyNotFoundException("ContainerStorageFee not found");
            }

            existingFee.ContainerId = updateDto.ContainerId;
            existingFee.PortId = updateDto.PortId;
            existingFee.StorageStartDate = updateDto.StorageStartDate;
            existingFee.StorageEndDate = updateDto.StorageEndDate;
            existingFee.DailyStorageRate = updateDto.DailyStorageRate;
            existingFee.FeeStatus = updateDto.FeeStatus;
            existingFee.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingFee);
            return MapToDto(existingFee);
        }

        private static ContainerStorageFeeDto MapToDto(ContainerStorageFee fee)
        {
            return new ContainerStorageFeeDto
            {
                Id = fee.Id,
                ContainerId = fee.ContainerId,
                PortId = fee.PortId,
                StorageStartDate = fee.StorageStartDate,
                StorageEndDate = fee.StorageEndDate,
                DailyStorageRate = fee.DailyStorageRate,
                TotalDays = fee.TotalDays,
                TotalFees = fee.TotalFees,
                FeeStatus = fee.FeeStatus
            };
        }
    }
}
