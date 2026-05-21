import React from 'react';
import { Check } from 'lucide-react';

/**
 * Компонент Checkbox - переиспользуемый чекбокс с поддержкой различных состояний
 * 
 * @param {boolean} checked - состояние чекбокса (отмечен/не отмечен)
 * @param {string} label - текст метки рядом с чекбоксом
 * @param {boolean} disabled - отключено ли состояние
 * @param {boolean} error - есть ли ошибка валидации
 * @param {string} errorText - текст ошибки
 * @param {function} onChange - функция обратного вызова при изменении
 * @param {string} className - дополнительные CSS классы
 * @param {string} id - уникальный идентификатор для связи label и input
 */
const Checkbox = ({
  checked = false,
  label = '',
  disabled = false,
  error = false,
  errorText = '',
  onChange,
  className = '',
  id,
}) => {
  // Генерируем уникальный id, если не передан
  const checkboxId = id || `checkbox-${Math.random().toString(36).substr(2, 9)}`;

  return (
    <div className={`flex flex-col gap-2 ${className}`}>
      <div className="flex items-center gap-3">
        {/* Скрытый стандартный input для доступности */}
        <input
          type="checkbox"
          id={checkboxId}
          checked={checked}
          disabled={disabled}
          onChange={(e) => onChange?.(e.target.checked)}
          className="sr-only" // sr-only скрывает визуально, но оставляет доступным для скринридеров
          aria-invalid={error}
          aria-describedby={error && errorText ? `${checkboxId}-error` : undefined}
        />

        {/* Кастомная визуализация чекбокса */}
        <label
          htmlFor={checkboxId}
          className={`
            relative flex items-center justify-center
            w-5 h-5 border-2 rounded cursor-pointer
            transition-all duration-200 ease-in-out
            select-none
            ${
              disabled
                ? 'bg-gray-100 border-gray-300 cursor-not-allowed opacity-50'
                : error
                ? 'bg-red-50 border-red-500 hover:border-red-600'
                : checked
                ? 'bg-blue-600 border-blue-600 hover:bg-blue-700'
                : 'bg-white border-gray-300 hover:border-gray-400'
            }
          `}
          role="checkbox"
          aria-checked={checked}
          aria-disabled={disabled}
        >
          {/* Иконка галочки, показывается только когда чекбокс отмечен */}
          {checked && (
            <Check
              className="w-3.5 h-3.5 text-white"
              strokeWidth={3}
              aria-hidden="true"
            />
          )}
        </label>

        {/* Текстовая метка */}
        {label && (
          <label
            htmlFor={checkboxId}
            className={`
              text-sm font-medium
              ${
                disabled
                  ? 'text-gray-400 cursor-not-allowed'
                  : error
                  ? 'text-red-600'
                  : 'text-gray-700'
              }
              cursor-pointer
            `}
          >
            {label}
          </label>
        )}
      </div>

      {/* Текст ошибки */}
      {error && errorText && (
        <p
          id={`${checkboxId}-error`}
          className="text-xs text-red-600 ml-8"
          role="alert"
        >
          {errorText}
        </p>
      )}
    </div>
  );
};

export default Checkbox;
