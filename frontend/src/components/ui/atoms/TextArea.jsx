import React from 'react';

/**
 * Компонент TextArea - переиспользуемое многострочное текстовое поле
 * 
 * @param {string} value - текущее значение
 * @param {string} label - текст метки над полем
 * @param {string} placeholder - текст заглушки
 * @param {boolean} disabled - отключено ли состояние
 * @param {boolean} error - есть ли ошибка валидации
 * @param {string} errorText - текст ошибки
 * @param {number} maxLength - максимальное количество символов
 * @param {boolean} showCount - показывать ли счетчик символов
 * @param {number} rows - количество строк по умолчанию
 * @param {function} onChange - функция обратного вызова при изменении
 * @param {string} className - дополнительные CSS классы
 * @param {string} id - уникальный идентификатор
 */
const TextArea = ({
  value = '',
  label = '',
  placeholder = '',
  disabled = false,
  error = false,
  errorText = '',
  maxLength,
  showCount = false,
  rows = 4,
  onChange,
  className = '',
  id,
}) => {
  // Генерируем уникальный id, если не передан
  const textareaId = id || `textarea-${Math.random().toString(36).substr(2, 9)}`;
  
  // Текущая длина текста
  const currentLength = value?.length || 0;

  return (
    <div className={`flex flex-col gap-2 ${className}`}>
      {/* Метка и счетчик */}
      {(label || (showCount && maxLength)) && (
        <div className="flex items-center justify-between">
          {label && (
            <label
              htmlFor={textareaId}
              className={`
                text-sm font-medium
                ${disabled ? 'text-gray-400' : error ? 'text-red-600' : 'text-gray-700'}
              `}
            >
              {label}
            </label>
          )}
          
          {showCount && maxLength && (
            <span className={`text-xs ${currentLength > maxLength ? 'text-red-600' : 'text-gray-500'}`}>
              {currentLength}/{maxLength}
            </span>
          )}
        </div>
      )}

      {/* Текстовое поле */}
      <textarea
        id={textareaId}
        value={value}
        placeholder={placeholder}
        disabled={disabled}
        maxLength={maxLength}
        rows={rows}
        onChange={(e) => onChange?.(e.target.value)}
        className={`
          w-full px-3 py-2.5 rounded-lg border resize-y
          bg-white transition-all duration-200 ease-in-out
          focus:outline-none focus:ring-2 focus:ring-offset-0
          disabled:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50
          ${
            error
              ? 'border-red-500 focus:border-red-600 focus:ring-red-100'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-100 hover:border-gray-400'
          }
        `}
        aria-invalid={error}
        aria-describedby={error && errorText ? `${textareaId}-error` : undefined}
      />

      {/* Текст ошибки */}
      {error && errorText && (
        <p
          id={`${textareaId}-error`}
          className="text-xs text-red-600"
          role="alert"
        >
          {errorText}
        </p>
      )}
    </div>
  );
};

export default TextArea;
