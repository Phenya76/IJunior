import React from 'react';

/**
 * Компонент Modal - базовое переиспользуемое модальное окно
 * 
 * @param {boolean} isOpen - открыто ли модальное окно
 * @param {string} title - заголовок модального окна
 * @param {React.ReactNode} children - содержимое модального окна
 * @param {boolean} showCloseButton - показывать ли кнопку закрытия
 * @param {boolean} closeOnBackdropClick - закрывать ли при клике на фон
 * @param {function} onClose - функция обратного вызова при закрытии
 * @param {string} className - дополнительные CSS классы для контейнера
 * @param {string} contentClassName - дополнительные CSS классы для контента
 */
const Modal = ({
  isOpen = false,
  title = '',
  children,
  showCloseButton = true,
  closeOnBackdropClick = true,
  onClose,
  className = '',
  contentClassName = '',
}) => {
  // Если модальное окно закрыто, не рендерим ничего
  if (!isOpen) return null;

  // Обработчик клика на фон
  const handleBackdropClick = (e) => {
    if (closeOnBackdropClick && e.target === e.currentTarget) {
      onClose?.();
    }
  };

  // Обработчик нажатия клавиши Escape
  React.useEffect(() => {
    const handleEscape = (e) => {
      if (e.key === 'Escape') {
        onClose?.();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
      // Блокируем прокрутку фона
      document.body.style.overflow = 'hidden';
    }

    return () => {
      document.removeEventListener('keydown', handleEscape);
      document.body.style.overflow = 'unset';
    };
  }, [isOpen, onClose]);

  return (
    // Затемненный фон (backdrop)
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm animate-fade-in"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
      aria-labelledby={title ? 'modal-title' : undefined}
    >
      {/* Контейнер модального окна */}
      <div
        className={`
          relative w-full max-w-md mx-4 bg-white rounded-xl shadow-2xl
          animate-slide-up max-h-[90vh] overflow-hidden
          ${className}
        `}
      >
        {/* Заголовок с кнопкой закрытия */}
        {(title || showCloseButton) && (
          <div className="flex items-center justify-between px-6 py-4 border-b border-gray-200">
            {title && (
              <h2
                id="modal-title"
                className="text-lg font-semibold text-gray-900"
              >
                {title}
              </h2>
            )}

            {showCloseButton && (
              <button
                onClick={onClose}
                className="p-2 text-gray-400 hover:text-gray-600 transition-colors rounded-lg hover:bg-gray-100"
                aria-label="Закрыть"
              >
                <svg
                  className="w-5 h-5"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            )}
          </div>
        )}

        {/* Контент модального окна */}
        <div
          className={`px-6 py-4 overflow-y-auto ${contentClassName}`}
          style={{ maxHeight: 'calc(90vh - 80px)' }}
        >
          {children}
        </div>
      </div>
    </div>
  );
};

export default Modal;
