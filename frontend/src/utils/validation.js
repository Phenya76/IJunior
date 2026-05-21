/**
 * Утилиты для валидации форм
 * Набор функций для проверки корректности ввода данных
 */

/**
 * Проверка email на корректность
 * @param {string} email - email для проверки
 * @returns {boolean} true если email корректен
 */
export const validateEmail = (email) => {
  if (!email) return false;
  
  // Регулярное выражение для проверки email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

/**
 * Проверка пароля на надежность
 * @param {string} password - пароль для проверки
 * @returns {Object} объект с результатами проверки
 */
export const validatePassword = (password) => {
  const errors = [];
  
  if (!password || password.length < 8) {
    errors.push('Пароль должен содержать минимум 8 символов');
  }
  
  if (!/[A-Z]/.test(password)) {
    errors.push('Пароль должен содержать хотя бы одну заглавную букву');
  }
  
  if (!/[a-z]/.test(password)) {
    errors.push('Пароль должен содержать хотя бы одну строчную букву');
  }
  
  if (!/[0-9]/.test(password)) {
    errors.push('Пароль должен содержать хотя бы одну цифру');
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
};

/**
 * Проверка телефона на корректность
 * @param {string} phone - телефон для проверки
 * @returns {boolean} true если телефон корректен
 */
export const validatePhone = (phone) => {
  if (!phone) return false;
  
  // Удаляем все нецифровые символы
  const cleaned = phone.replace(/\D/g, '');
  
  // Проверяем длину (от 10 до 15 цифр)
  return cleaned.length >= 10 && cleaned.length <= 15;
};

/**
 * Проверка обязательного поля
 * @param {any} value - значение для проверки
 * @returns {boolean} true если поле заполнено
 */
export const validateRequired = (value) => {
  if (typeof value === 'string') {
    return value.trim().length > 0;
  }
  return value !== null && value !== undefined;
};

/**
 * Проверка длины строки
 * @param {string} value - строка для проверки
 * @param {number} min - минимальная длина
 * @param {number} max - максимальная длина
 * @returns {Object} объект с результатами проверки
 */
export const validateLength = (value, min, max) => {
  if (!value) {
    return {
      isValid: min === 0,
      error: min > 0 ? `Минимальная длина ${min} символов` : null,
    };
  }
  
  const length = value.length;
  
  if (length < min) {
    return {
      isValid: false,
      error: `Минимальная длина ${min} символов`,
    };
  }
  
  if (max && length > max) {
    return {
      isValid: false,
      error: `Максимальная длина ${max} символов`,
    };
  }
  
  return {
    isValid: true,
    error: null,
  };
};

/**
 * Валидация формы по схеме
 * @param {Object} values - значения полей формы
 * @param {Object} schema - схема валидации
 * @returns {Object} объект с ошибками валидации
 */
export const validateForm = (values, schema) => {
  const errors = {};
  
  for (const [field, rules] of Object.entries(schema)) {
    const value = values[field];
    
    for (const rule of rules) {
      let error = null;
      
      if (rule.required && !validateRequired(value)) {
        error = rule.message || 'Это поле обязательно для заполнения';
      } else if (rule.email && !validateEmail(value)) {
        error = rule.message || 'Введите корректный email';
      } else if (rule.phone && !validatePhone(value)) {
        error = rule.message || 'Введите корректный номер телефона';
      } else if (rule.minLength || rule.maxLength) {
        const result = validateLength(value, rule.minLength || 0, rule.maxLength);
        error = result.error;
      } else if (rule.validate && typeof rule.validate === 'function') {
        error = rule.validate(value);
      }
      
      if (error) {
        errors[field] = error;
        break; // Прерываем после первой ошибки
      }
    }
  }
  
  return errors;
};

export default {
  validateEmail,
  validatePassword,
  validatePhone,
  validateRequired,
  validateLength,
  validateForm,
};
