"""
Binance Price Alert Desktop Application
Приложение для отслеживания ценовых уровней на бирже Binance
с отправкой системных уведомлений Windows
"""

import tkinter as tk
from tkinter import ttk, messagebox
import threading
import time
import json
import os
from datetime import datetime
import requests
from plyer import notification

# Файл для сохранения уровней
LEVELS_FILE = "price_levels.json"


class BinanceAPI:
    """Класс для работы с Binance API"""
    
    BASE_URL = "https://api.binance.com/api/v3"
    
    @classmethod
    def get_all_symbols(cls):
        """Получить все торгуемые символы на Binance"""
        try:
            response = requests.get(f"{cls.BASE_URL}/exchangeInfo", timeout=10)
            response.raise_for_status()
            data = response.json()
            # Фильтруем только активные символы с статусом TRADING
            symbols = [
                s['symbol'] for s in data['symbols'] 
                if s['status'] == 'TRADING' and s['quoteAsset'] in ['USDT', 'BUSD', 'BTC', 'ETH', 'BNB']
            ]
            return sorted(symbols)
        except Exception as e:
            print(f"Ошибка получения символов: {e}")
            return []
    
    @classmethod
    def get_price(cls, symbol):
        """Получить текущую цену символа"""
        try:
            response = requests.get(
                f"{cls.BASE_URL}/ticker/price",
                params={"symbol": symbol},
                timeout=5
            )
            response.raise_for_status()
            data = response.json()
            return float(data['price'])
        except Exception as e:
            print(f"Ошибка получения цены для {symbol}: {e}")
            return None
    
    @classmethod
    def get_prices_batch(cls, symbols):
        """Получить цены для нескольких символов"""
        try:
            response = requests.get(
                f"{cls.BASE_URL}/ticker/price",
                timeout=10
            )
            response.raise_for_status()
            all_prices = response.json()
            price_dict = {item['symbol']: float(item['price']) for item in all_prices}
            
            # Возвращаем только нужные символы
            result = {}
            for symbol in symbols:
                if symbol in price_dict:
                    result[symbol] = price_dict[symbol]
            return result
        except Exception as e:
            print(f"Ошибка получения пакета цен: {e}")
            return {}


class PriceLevel:
    """Класс представляющий ценовой уровень"""
    
    def __init__(self, symbol, price, condition="above", active=True):
        self.symbol = symbol
        self.price = float(price)
        self.condition = condition  # "above" или "below"
        self.active = active
        self.created_at = datetime.now()
        self.triggered = False
    
    def check(self, current_price):
        """Проверить, достигнута ли цена уровня"""
        if not self.active or self.triggered:
            return False
        
        if self.condition == "above":
            if current_price >= self.price:
                self.triggered = True
                return True
        elif self.condition == "below":
            if current_price <= self.price:
                self.triggered = True
                return True
        return False
    
    def to_dict(self):
        return {
            'symbol': self.symbol,
            'price': self.price,
            'condition': self.condition,
            'active': self.active,
            'triggered': self.triggered,
            'created_at': self.created_at.isoformat()
        }
    
    @classmethod
    def from_dict(cls, data):
        level = cls(
            data['symbol'],
            data['price'],
            data['condition'],
            data['active']
        )
        level.triggered = data.get('triggered', False)
        level.created_at = datetime.fromisoformat(data['created_at'])
        return level


class LevelManager:
    """Менеджер для управления ценовыми уровнями"""
    
    def __init__(self, filename=LEVELS_FILE):
        self.filename = filename
        self.levels = []
        self.load()
    
    def add_level(self, symbol, price, condition="above"):
        """Добавить новый уровень"""
        level = PriceLevel(symbol, price, condition)
        self.levels.append(level)
        self.save()
        return level
    
    def remove_level(self, index):
        """Удалить уровень по индексу"""
        if 0 <= index < len(self.levels):
            self.levels.pop(index)
            self.save()
            return True
        return False
    
    def deactivate_level(self, index):
        """Деактивировать уровень"""
        if 0 <= index < len(self.levels):
            self.levels[index].active = False
            self.save()
            return True
        return False
    
    def reactivate_level(self, index):
        """Реактивировать уровень"""
        if 0 <= index < len(self.levels):
            self.levels[index].active = True
            self.levels[index].triggered = False
            self.save()
            return True
        return False
    
    def get_active_levels(self):
        """Получить все активные уровни"""
        return [l for l in self.levels if l.active and not l.triggered]
    
    def save(self):
        """Сохранить уровни в файл"""
        try:
            data = [level.to_dict() for level in self.levels]
            with open(self.filename, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=2, ensure_ascii=False)
        except Exception as e:
            print(f"Ошибка сохранения: {e}")
    
    def load(self):
        """Загрузить уровни из файла"""
        try:
            if os.path.exists(self.filename):
                with open(self.filename, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                self.levels = [PriceLevel.from_dict(d) for d in data]
        except Exception as e:
            print(f"Ошибка загрузки: {e}")
            self.levels = []


class NotificationService:
    """Сервис для отправки уведомлений"""
    
    @staticmethod
    def send(title, message, app_name="Binance Alert"):
        """Отправить системное уведомление"""
        try:
            notification.notify(
                title=title,
                message=message,
                app_name=app_name,
                timeout=10  # Уведомление исчезнет через 10 секунд
            )
            print(f"[{datetime.now()}] Уведомление: {title} - {message}")
        except Exception as e:
            print(f"Ошибка отправки уведомления: {e}")
            # Fallback - вывод в консоль
            print(f"[ALERT] {title}: {message}")


class MonitorThread(threading.Thread):
    """Поток для мониторинга цен"""
    
    def __init__(self, level_manager, callback=None):
        super().__init__(daemon=True)
        self.level_manager = level_manager
        self.callback = callback
        self.running = False
        self.check_interval = 5  # Проверка каждые 5 секунд
    
    def run(self):
        """Основной цикл мониторинга"""
        self.running = True
        print("Мониторинг запущен...")
        
        while self.running:
            try:
                # Получаем все активные уровни
                active_levels = self.level_manager.get_active_levels()
                
                if active_levels:
                    # Собираем уникальные символы
                    symbols = list(set(level.symbol for level in active_levels))
                    
                    # Получаем цены для всех символов
                    prices = BinanceAPI.get_prices_batch(symbols)
                    
                    # Проверяем каждый уровень
                    triggered_levels = []
                    for level in active_levels:
                        if level.symbol in prices:
                            current_price = prices[level.symbol]
                            if level.check(current_price):
                                triggered_levels.append((level, current_price))
                    
                    # Отправляем уведомления о сработавших уровнях
                    for level, current_price in triggered_levels:
                        direction = "выше" if level.condition == "above" else "ниже"
                        title = f"🚨 Binance Alert: {level.symbol}"
                        message = (
                            f"Цена {current_price:.6f} {direction} уровня {level.price:.6f}\n"
                            f"Время: {datetime.now().strftime('%H:%M:%S')}"
                        )
                        NotificationService.send(title, message)
                        
                        # Сохраняем изменения
                        self.level_manager.save()
                        
                        # Вызываем callback для обновления UI
                        if self.callback:
                            self.callback()
                
                time.sleep(self.check_interval)
                
            except Exception as e:
                print(f"Ошибка в цикле мониторинга: {e}")
                time.sleep(self.check_interval)
    
    def stop(self):
        """Остановить мониторинг"""
        self.running = False


class BinanceAlertApp:
    """Основное приложение"""
    
    def __init__(self, root):
        self.root = root
        self.root.title("Binance Price Alerts")
        self.root.geometry("900x700")
        self.root.minsize(800, 600)
        
        # Менеджер уровней
        self.level_manager = LevelManager()
        
        # Поток мониторинга
        self.monitor_thread = None
        
        # Кэш символов
        self.symbols_cache = []
        
        # Настройка UI
        self.setup_ui()
        
        # Загрузка символов
        self.load_symbols_async()
        
        # Запуск мониторинга
        self.start_monitoring()
        
        # Обработка закрытия окна
        self.root.protocol("WM_DELETE_WINDOW", self.on_closing)
    
    def setup_ui(self):
        """Настройка пользовательского интерфейса"""
        # Основной фрейм
        main_frame = ttk.Frame(self.root, padding="10")
        main_frame.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        # Настройка весов для растягивания
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        main_frame.columnconfigure(0, weight=1)
        main_frame.rowconfigure(3, weight=1)
        
        # === Секция добавления уровня ===
        add_frame = ttk.LabelFrame(main_frame, text="Добавить новый уровень", padding="10")
        add_frame.grid(row=0, column=0, sticky=(tk.W, tk.E), pady=(0, 10))
        add_frame.columnconfigure(1, weight=1)
        
        # Символ
        ttk.Label(add_frame, text="Символ:").grid(row=0, column=0, sticky=tk.W, padx=(0, 5))
        self.symbol_var = tk.StringVar()
        self.symbol_combo = ttk.Combobox(
            add_frame, 
            textvariable=self.symbol_var,
            width=15,
            state="readonly"
        )
        self.symbol_combo.grid(row=0, column=1, sticky=(tk.W, tk.E), padx=(0, 5))
        self.symbol_combo.bind('<KeyRelease>', self.filter_symbols)
        
        # Цена
        ttk.Label(add_frame, text="Цена:").grid(row=0, column=2, sticky=tk.W, padx=(0, 5))
        self.price_var = tk.StringVar()
        price_entry = ttk.Entry(add_frame, textvariable=self.price_var, width=15)
        price_entry.grid(row=0, column=3, sticky=(tk.W, tk.E), padx=(0, 5))
        
        # Условие
        ttk.Label(add_frame, text="Условие:").grid(row=0, column=4, sticky=tk.W, padx=(0, 5))
        self.condition_var = tk.StringVar(value="above")
        condition_combo = ttk.Combobox(
            add_frame,
            textvariable=self.condition_var,
            values=["above", "below"],
            state="readonly",
            width=10
        )
        condition_combo.grid(row=0, column=5, sticky=(tk.W, tk.E), padx=(0, 5))
        
        # Кнопка добавления
        add_btn = ttk.Button(add_frame, text="➕ Добавить", command=self.add_level)
        add_btn.grid(row=0, column=6, padx=(10, 0))
        
        # Подсказка
        hint_label = ttk.Label(
            add_frame, 
            text="Начните вводить символ для поиска (например: BTCUSDT)",
            font=('TkDefaultFont', 8),
            foreground='gray'
        )
        hint_label.grid(row=1, column=0, columnspan=7, sticky=tk.W, pady=(5, 0))
        
        # === Статус бар ===
        status_frame = ttk.Frame(main_frame)
        status_frame.grid(row=1, column=0, sticky=(tk.W, tk.E), pady=(0, 10))
        
        self.status_label = ttk.Label(status_frame, text="● Мониторинг активен", foreground="green")
        self.status_label.pack(side=tk.LEFT)
        
        self.active_count_label = ttk.Label(status_frame, text="Активных уровней: 0")
        self.active_count_label.pack(side=tk.RIGHT)
        
        # === Список уровней ===
        list_frame = ttk.LabelFrame(main_frame, text="Ваши уровни", padding="10")
        list_frame.grid(row=2, column=0, sticky=(tk.W, tk.E, tk.N, tk.S), pady=(0, 10))
        list_frame.columnconfigure(0, weight=1)
        list_frame.rowconfigure(0, weight=1)
        
        # Treeview для отображения уровней
        columns = ('symbol', 'price', 'condition', 'status', 'created')
        self.levels_tree = ttk.Treeview(list_frame, columns=columns, show='headings', height=10)
        
        # Настройка колонок
        self.levels_tree.heading('symbol', text='Символ')
        self.levels_tree.heading('price', text='Цена уровня')
        self.levels_tree.heading('condition', text='Условие')
        self.levels_tree.heading('status', text='Статус')
        self.levels_tree.heading('created', text='Создан')
        
        self.levels_tree.column('symbol', width=120)
        self.levels_tree.column('price', width=100)
        self.levels_tree.column('condition', width=80)
        self.levels_tree.column('status', width=80)
        self.levels_tree.column('created', width=150)
        
        # Scrollbar
        scrollbar = ttk.Scrollbar(list_frame, orient=tk.VERTICAL, command=self.levels_tree.yview)
        self.levels_tree.configure(yscrollcommand=scrollbar.set)
        
        self.levels_tree.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        scrollbar.grid(row=0, column=1, sticky=(tk.N, tk.S))
        
        # === Кнопки управления ===
        btn_frame = ttk.Frame(main_frame)
        btn_frame.grid(row=3, column=0, sticky=(tk.W, tk.E))
        
        delete_btn = ttk.Button(btn_frame, text="🗑️ Удалить", command=self.delete_level)
        delete_btn.pack(side=tk.LEFT, padx=(0, 5))
        
        deactivate_btn = ttk.Button(btn_frame, text="⏸️ Деактивировать", command=self.deactivate_level)
        deactivate_btn.pack(side=tk.LEFT, padx=(0, 5))
        
        reactivate_btn = ttk.Button(btn_frame, text="▶️ Активировать", command=self.reactivate_level)
        reactivate_btn.pack(side=tk.LEFT, padx=(0, 5))
        
        clear_triggered_btn = ttk.Button(btn_frame, text="🧹 Очистить сработавшие", command=self.clear_triggered)
        clear_triggered_btn.pack(side=tk.LEFT, padx=(0, 5))
        
        refresh_btn = ttk.Button(btn_frame, text="🔄 Обновить список", command=self.refresh_levels)
        refresh_btn.pack(side=tk.RIGHT)
        
        # === Лог событий ===
        log_frame = ttk.LabelFrame(main_frame, text="Лог событий", padding="10")
        log_frame.grid(row=4, column=0, sticky=(tk.W, tk.E, tk.N, tk.S), pady=(10, 0))
        log_frame.columnconfigure(0, weight=1)
        log_frame.rowconfigure(0, weight=1)
        
        self.log_text = tk.Text(log_frame, height=6, width=80, state='disabled', wrap=tk.WORD)
        self.log_text.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        log_scrollbar = ttk.Scrollbar(log_frame, orient=tk.VERTICAL, command=self.log_text.yview)
        log_scrollbar.grid(row=0, column=1, sticky=(tk.N, tk.S))
        self.log_text.configure(yscrollcommand=log_scrollbar.set)
        
        # Начальное сообщение в лог
        self.log_event("Приложение запущено. Мониторинг активен.")
    
    def filter_symbols(self, event=None):
        """Фильтрация символов при вводе"""
        query = self.symbol_var.get().upper()
        if not query:
            self.symbol_combo['values'] = self.symbols_cache[:100]  # Показываем первые 100
        else:
            filtered = [s for s in self.symbols_cache if query in s]
            self.symbol_combo['values'] = filtered[:100]  # Ограничиваем до 100 результатов
    
    def load_symbols_async(self):
        """Асинхронная загрузка символов"""
        def load():
            self.log_event("Загрузка списка символов...")
            symbols = BinanceAPI.get_all_symbols()
            self.symbols_cache = symbols
            self.root.after(0, lambda: self.symbol_combo.configure(values=symbols[:100]))
            self.log_event(f"Загружено {len(symbols)} символов")
        
        thread = threading.Thread(target=load, daemon=True)
        thread.start()
    
    def add_level(self):
        """Добавить новый уровень"""
        symbol = self.symbol_var.get().strip().upper()
        price_str = self.price_var.get().strip()
        condition = self.condition_var.get()
        
        if not symbol:
            messagebox.showerror("Ошибка", "Выберите символ")
            return
        
        if not price_str:
            messagebox.showerror("Ошибка", "Введите цену")
            return
        
        try:
            price = float(price_str)
            if price <= 0:
                raise ValueError("Цена должна быть положительной")
        except ValueError as e:
            messagebox.showerror("Ошибка", f"Некорректная цена: {e}")
            return
        
        # Добавляем уровень
        self.level_manager.add_level(symbol, price, condition)
        
        # Очищаем поля
        self.symbol_var.set("")
        self.price_var.set("")
        
        # Обновляем список
        self.refresh_levels()
        
        self.log_event(f"Добавлен уровень: {symbol} {condition} {price}")
    
    def delete_level(self):
        """Удалить выбранный уровень"""
        selection = self.levels_tree.selection()
        if not selection:
            messagebox.showwarning("Предупреждение", "Выберите уровень для удаления")
            return
        
        if messagebox.askyesno("Подтверждение", "Удалить выбранный уровень?"):
            index = self.levels_tree.index(selection[0])
            self.level_manager.remove_level(index)
            self.refresh_levels()
            self.log_event("Уровень удален")
    
    def deactivate_level(self):
        """Деактивировать выбранный уровень"""
        selection = self.levels_tree.selection()
        if not selection:
            messagebox.showwarning("Предупреждение", "Выберите уровень для деактивации")
            return
        
        index = self.levels_tree.index(selection[0])
        self.level_manager.deactivate_level(index)
        self.refresh_levels()
        self.log_event("Уровень деактивирован")
    
    def reactivate_level(self):
        """Активировать выбранный уровень"""
        selection = self.levels_tree.selection()
        if not selection:
            messagebox.showwarning("Предупреждение", "Выберите уровень для активации")
            return
        
        index = self.levels_tree.index(selection[0])
        self.level_manager.reactivate_level(index)
        self.refresh_levels()
        self.log_event("Уровень активирован")
    
    def clear_triggered(self):
        """Очистить сработавшие уровни"""
        if messagebox.askyesno("Подтверждение", "Удалить все сработавшие уровни?"):
            self.level_manager.levels = [
                l for l in self.level_manager.levels 
                if not l.triggered
            ]
            self.level_manager.save()
            self.refresh_levels()
            self.log_event("Сработавшие уровни очищены")
    
    def refresh_levels(self):
        """Обновить список уровней в UI"""
        # Очищаем дерево
        for item in self.levels_tree.get_children():
            self.levels_tree.delete(item)
        
        # Добавляем уровни
        for level in self.level_manager.levels:
            condition_text = "≥" if level.condition == "above" else "≤"
            
            if level.triggered:
                status = "✅ Сработал"
            elif not level.active:
                status = "⏸️ Пауза"
            else:
                status = "🟢 Активен"
            
            created_str = level.created_at.strftime("%d.%m %H:%M")
            
            self.levels_tree.insert('', tk.END, values=(
                level.symbol,
                f"{level.price:.6f}",
                condition_text,
                status,
                created_str
            ))
        
        # Обновляем счетчик
        active_count = len(self.level_manager.get_active_levels())
        self.active_count_label.configure(text=f"Активных уровней: {active_count}")
    
    def start_monitoring(self):
        """Запустить мониторинг"""
        def on_level_triggered():
            self.root.after(0, self.refresh_levels)
        
        self.monitor_thread = MonitorThread(self.level_manager, callback=on_level_triggered)
        self.monitor_thread.start()
    
    def log_event(self, message):
        """Добавить событие в лог"""
        timestamp = datetime.now().strftime("%H:%M:%S")
        log_entry = f"[{timestamp}] {message}\n"
        
        self.log_text.configure(state='normal')
        self.log_text.insert(tk.END, log_entry)
        self.log_text.see(tk.END)
        self.log_text.configure(state='disabled')
    
    def on_closing(self):
        """Обработка закрытия окна"""
        if messagebox.askokcancel("Выход", "Закрыть приложение?\nМониторинг будет остановлен."):
            if self.monitor_thread:
                self.monitor_thread.stop()
            self.root.destroy()


def main():
    """Точка входа приложения"""
    root = tk.Tk()
    
    # Установка иконки (если есть)
    try:
        root.iconbitmap("icon.ico")
    except:
        pass
    
    # Создание приложения
    app = BinanceAlertApp(root)
    
    # Запуск главного цикла
    root.mainloop()


if __name__ == "__main__":
    main()
