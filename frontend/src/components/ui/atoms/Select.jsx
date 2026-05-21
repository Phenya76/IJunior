import React from 'react';

/**
 * Компонент Select - переиспользуемый выпадающий список с поддержкой различных состояний
 * 
 * @param {Array} options - массив опций [{value, label}]
 * @param {string} value - текущее выбранное значение
 * @param {string} label - текст метки над select
 * @param {boolean} disabled - отключено ли состояние
 * @param {boolean} error - есть ли ошибка валидации
 * @param {string} errorText - текст ошибки
 * @param {string} placeholder - текст заглушки
 * @param {function} onChange - функция обратного вызова при изменении
 * @param {string} className - дополнительные CSS классы
 * @param {string} id - уникальный идентификатор
 */
const Select = ({
  options = [],
  value = '',
  label = '',
  disabled = false,
  error = false,
  errorText = '',
  placeholder = 'Выберите значение',
  onChange,
  className = '',
  id,
}) => {
  // Генерируем уникальный id, если не передан
  const selectId = id || `select-${Math.random().toString(36).substr(2, 9)}`;

  return (
    <div className={`flex flex-col gap-2 ${className}`}>
      {/* Метка */}
      {label && (
        <label
          htmlFor={selectId}
          className={`
            text-sm font-medium
            ${disabled ? 'text-gray-400' : error ? 'text-red-600' : 'text-gray-700'}
          `}
        >
          {label}
        </label>
      )}

      {/* Контейнер select */}
      <div className="relative">
        <select
          id={selectId}
          value={value}
          disabled={disabled}
          onChange={(e) => onChange?.(e.target.value)}
          className={`
            w-full px-3 py-2.5 rounded-lg border appearance-none
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
          aria-describedby={error && errorText ? `${selectId}-error` : undefined}
        >
          {/* Опция placeholder */}
          <option value="" disabled>
            {placeholder}
          </option>

          {/* Список опций */}
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>

        {/* Иконка стрелки */}
        <div className="absolute inset-y-0 right-0 flex items-center px-3 pointer-events-none">
          <svg
            className={`w-4 h-4 ${disabled ? 'text-gray-400' : 'text-gray-500'}`}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M19 9l-7 7-7-7"
            />
          </svg>
        </div>
      </div>

      {/* Текст ошибки */}
      {error && errorText && (
        <p
          id={`${selectId}-error`}
          className="text-xs text-red-600"
          role="alert"
        >
          {errorText}
        </p>
      )}
    </div>
  );
};

export default Select;
