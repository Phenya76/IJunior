import React, { createContext, useContext, useState, useCallback } from 'react';

/**
 * Контекст для управления модальными окнами
 * Предоставляет функции для открытия/закрытия модальных окон
 */
const ModalContext = createContext(null);

/**
 * Провайдер модальных окон - оборачивает приложение и предоставляет доступ к управлению модалками
 */
export const ModalProvider = ({ children }) => {
  // Состояние для хранения информации об открытом модальном окне
  const [modalState, setModalState] = useState({
    isOpen: false,
    modalType: null, // тип модального окна (например, 'login', 'register')
    modalProps: {}, // дополнительные props для модального окна
  });

  /**
   * Открыть модальное окно
   * @param {string} modalType - тип модального окна
   * @param {Object} props - дополнительные props
   */
  const openModal = useCallback((modalType, props = {}) => {
    setModalState({
      isOpen: true,
      modalType,
      modalProps: props,
    });
  }, []);

  /**
   * Закрыть модальное окно
   */
  const closeModal = useCallback(() => {
    setModalState((prev) => ({
      ...prev,
      isOpen: false,
    }));
  }, []);

  /**
   * Значение контекста
   */
  const value = {
    isOpen: modalState.isOpen,
    modalType: modalState.modalType,
    modalProps: modalState.modalProps,
    openModal,
    closeModal,
  };

  return (
    <ModalContext.Provider value={value}>
      {children}
    </ModalContext.Provider>
  );
};

/**
 * Хук для использования контекста модальных окон
 * @returns {Object} методы и состояние модальных окон
 */
export const useModal = () => {
  const context = useContext(ModalContext);
  
  if (!context) {
    throw new Error('useModal должен использоваться внутри ModalProvider');
  }
  
  return context;
};

export default ModalContext;
