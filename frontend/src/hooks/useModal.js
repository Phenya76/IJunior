import { useState, useCallback } from 'react';

/**
 * Кастомный хук для управления модальными окнами
 * Предоставляет простые методы для открытия/закрытия модального окна
 * 
 * @param {boolean} initialState - начальное состояние (открыто/закрыто)
 * @returns {Object} методы и состояние модального окна
 */
const useModal = (initialState = false) => {
  // Состояние открыто/закрыто
  const [isOpen, setIsOpen] = useState(initialState);

  /**
   * Открыть модальное окно
   */
  const open = useCallback(() => {
    setIsOpen(true);
  }, []);

  /**
   * Закрыть модальное окно
   */
  const close = useCallback(() => {
    setIsOpen(false);
  }, []);

  /**
   * Переключить состояние (открыть если закрыто, закрыть если открыто)
   */
  const toggle = useCallback(() => {
    setIsOpen((prev) => !prev);
  }, []);

  return {
    isOpen,
    open,
    close,
    toggle,
  };
};

export default useModal;
