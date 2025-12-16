using System;


namespace RepairRequests
{
    public class RequestValidator
    {
        public static ValidationResult ValidateResponsiblePerson(string Responsible)
        {
            if (string.IsNullOrWhiteSpace(Responsible))
            {
                return ValidationResult.Error("Поле 'Ответственный' обязательно для заполнения!");
            }

            // Проверяем на наличие цифр
            if (ContainsDigits(Responsible))
            {
                return ValidationResult.Error("В поле 'Ответственный' нельзя вводить цифры!");
            }

            return ValidationResult.Success();
        }

        // Валидация статуса при изменении
        public static ValidationResult ValidateStatusChange(string Status, string newStatus)
        {
            if (Status == "Выполнено" && newStatus != "Выполнено")
            {
                return ValidationResult.Error("Нельзя изменить статус 'Выполнено' на другой!");
            }

            if (Status == "Не выполнено" && newStatus != "Не выполнено")
            {
                return ValidationResult.Error("Нельзя изменить статус 'Не выполнено' на другой!");
            }

            if (Status == "Выполнено" && newStatus == "Не выполнено")
            {
                return ValidationResult.Error("Нельзя отменить уже выполненную работу!");
            }

            return ValidationResult.Success();
        }
        // Проверка: можно ли поставить статус "В процессе" при просроченной дате
        public static ValidationResult ValidateDeadlineForInProgress(DateTime deadline, string newStatus)
        {
            if (newStatus == "В процессе" && deadline.Date < DateTime.Now.Date)
            {
                return ValidationResult.Error("Нельзя установить статус 'В процессе' при просроченной дате!");
            }

            return ValidationResult.Success();
        }

        private static bool ContainsDigits(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
    }   
}

