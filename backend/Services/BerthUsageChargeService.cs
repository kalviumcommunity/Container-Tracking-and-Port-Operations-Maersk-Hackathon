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
    public class BerthUsageChargeService : IBerthUsageChargeService
    {
        private readonly IBerthUsageChargeRepository _repository;

        public BerthUsageChargeService(IBerthUsageChargeRepository repository)
        {
            _repository = repository;
        }

        public async Task<BerthUsageChargeDto> CreateAsync(BerthUsageChargeCreateUpdateDto createDto)
        {
            var charge = new BerthUsageCharge
            {
                BerthAssignmentId = createDto.BerthAssignmentId,
                HourlyRate = createDto.HourlyRate,
                TotalHours = createDto.TotalHours,
                BaseCharges = createDto.HourlyRate * createDto.TotalHours,
                ServiceCharges = createDto.ServiceCharges,
                TotalCharges = (createDto.HourlyRate * createDto.TotalHours) + createDto.ServiceCharges,
                PaymentStatus = createDto.PaymentStatus
            };
            var newCharge = await _repository.CreateAsync(charge);
            return MapToDto(newCharge);
        }

        public async Task<bool> DeleteAsync(object id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BerthUsageChargeDto>> GetAsync(Expression<Func<BerthUsageCharge, bool>> filter)
        {
            var charges = await _repository.GetAsync(filter);
            return charges.Select(MapToDto);
        }

        public async Task<IEnumerable<BerthUsageChargeDto>> GetAllAsync()
        {
            var charges = await _repository.GetAllAsync();
            return charges.Select(MapToDto);
        }

        public async Task<BerthUsageChargeDto> GetByIdAsync(object id)
        {
            var charge = await _repository.GetByIdAsync(id);
            return charge == null ? null : MapToDto(charge);
        }

        public async Task<BerthUsageChargeDto> UpdateAsync(object id, BerthUsageChargeCreateUpdateDto updateDto)
        {
            var existingCharge = await _repository.GetByIdAsync(id);
            if (existingCharge == null)
            {
                throw new KeyNotFoundException("BerthUsageCharge not found");
            }

            existingCharge.BerthAssignmentId = updateDto.BerthAssignmentId;
            existingCharge.HourlyRate = updateDto.HourlyRate;
            existingCharge.TotalHours = updateDto.TotalHours;
            existingCharge.BaseCharges = updateDto.HourlyRate * updateDto.TotalHours;
            existingCharge.ServiceCharges = updateDto.ServiceCharges;
            existingCharge.TotalCharges = (updateDto.HourlyRate * updateDto.TotalHours) + updateDto.ServiceCharges;
            existingCharge.PaymentStatus = updateDto.PaymentStatus;
            existingCharge.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingCharge);
            return MapToDto(existingCharge);
        }

        private static BerthUsageChargeDto MapToDto(BerthUsageCharge charge)
        {
            return new BerthUsageChargeDto
            {
                Id = charge.Id,
                BerthAssignmentId = charge.BerthAssignmentId,
                HourlyRate = charge.HourlyRate,
                TotalHours = charge.TotalHours,
                BaseCharges = charge.BaseCharges,
                ServiceCharges = charge.ServiceCharges,
                TotalCharges = charge.TotalCharges,
                ChargedAt = charge.ChargedAt,
                PaymentStatus = charge.PaymentStatus
            };
        }
    }
}
