using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairRequests;

namespace RepairRequests
{
    public class RepairRequestService
    {
        private readonly RepairRequestRepository _repository;

        public RepairRequestService(string connectionString)
        {
            _repository = new RepairRequestRepository(connectionString);
        }

        // Получить все заявки
        public List<Model> GetAllRequests()
        {
            return _repository.GetAllRequests();
        }

        // Добавить новую заявку с валидацией
        public OperationResult AddNewRequest(Model request)
        {
            var validationResult = RequestValidator.ValidateResponsiblePerson(request.Responsible);
            if (!validationResult.IsValid)
            {
                return OperationResult.Error(validationResult.ErrorMessage);
            }
            var deadlineValidation = RequestValidator.ValidateDeadlineForInProgress(request.Deadline, request.Status);
            if (!deadlineValidation.IsValid)
            {
                return OperationResult.Error(deadlineValidation.ErrorMessage);
            }

            try
            {
                int newId = _repository.AddRequest(request);
                request.Id = newId;
                return OperationResult.Success("Заявка успешно добавлена!", request);
            }
            catch (Exception ex)
            {
                return OperationResult.Error($"Ошибка при добавлении заявки: {ex.Message}");
            }
        }

        // Изменить статус заявки с валидацией
        public OperationResult ChangeStatus(int requestId, string newStatus)
        {
            try
            {
                var request = _repository.GetById(requestId);
                if (request == null)
                    return OperationResult.Error("Заявка не найдена!");

                var validation = RequestValidator.ValidateStatusChange(request.Status, newStatus);
                if (!validation.IsValid)
                    return OperationResult.Error(validation.ErrorMessage);

                bool success = _repository.UpdateStatus(requestId, newStatus);

                if (success)
                    return OperationResult.Success($"Статус изменен на '{newStatus}'!");

                return OperationResult.Error("Ошибка обновления статуса!");
            }
            catch (Exception ex)
            {
                return OperationResult.Error($"Ошибка: {ex.Message}");
            }

        }

        // Метод фильтрации по статусу
        public List<Model> GetRequestsByStatus(string status)
        {
            var all = _repository.GetAllRequests();

            if (string.IsNullOrEmpty(status) || status == "Все")
                return all;

            return all.Where(r => r.Status == status).ToList();
        }

        public OperationResult ChangeStatusWithValidation(int id, string newStatus)
        {
            var request = _repository.GetById(id);
            if (request == null) return OperationResult.Error("Заявка не найдена");

            var validation = RequestValidator.ValidateStatusChange(request.Status, newStatus);
            if (!validation.IsValid) return OperationResult.Error(validation.ErrorMessage);

            return _repository.UpdateStatus(id, newStatus)
                ? OperationResult.Success($"Статус изменен на '{newStatus}'")
                : OperationResult.Error("Не удалось изменить статус");
        }       
    }
}
